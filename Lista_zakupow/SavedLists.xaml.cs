using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Lista_zakupow
{
    /// <summary>
    /// Strona odpowiadająca za wyświetlanie listy zakupów i operacje na niej.
    /// </summary>
    public sealed partial class SavedLists : Page
    {      
        public SavedLists()
        {
            this.InitializeComponent();
        }       

        /// <summary>
        /// Odpowiada za załadowanie interfejsu, gdy chociaż jedna lista jest utworzona.
        /// </summary>
        private async void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Najpierw sprawdzam czy lista jest już utworzona.
            if (await isFileExists("List1.txt"))
            {
                ListboxData.list1Created = true;
            }
            if (await isFileExists("List2.txt"))
            {
                ListboxData.list2Created = true;
            }
            if (await isFileExists("List3.txt"))
            {
                ListboxData.list3Created = true;
            }

            //Jeśli, któraś z list jest utworzona wyświetl jej menu.
            if (ListboxData.list1Created || ListboxData.list2Created || ListboxData.list3Created)
            {
                tbInfo.Visibility = Visibility.Collapsed;
                pivotMain.Visibility = Visibility.Visible;
                if (ListboxData.list1Created)
                {
                    pivot1.Visibility = Visibility.Visible;
                }
                if (ListboxData.list2Created)
                {
                    pivot2.Visibility = Visibility.Visible;
                }
                if (ListboxData.list3Created)
                {
                    pivot3.Visibility = Visibility.Visible;
                }
            }
            else
            {
                tbInfo.Visibility = Visibility.Visible;
                pivotMain.Visibility = Visibility.Collapsed;
            }
                
        }

        /// <summary>
        /// Sprawdza czy dana lista istnieje i zwraca wartość bool.
        /// </summary>
        public async Task<bool> isFileExists(string fileName)
        {
            bool fileExists = false;
            var allfiles = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var storageFile in allfiles)
            {
                if (storageFile.Name == fileName)
                {
                    fileExists = true;
                }
            }
            return fileExists;
        }

        /// <summary>
        /// Wyświetla pierwszą listę zakupów.
        /// </summary>
        private void Pivot1_Loaded(object sender, RoutedEventArgs e)
        {
            if (ListboxData.list1Created)
                ViewList(ListboxData.listboxValues1, pivot1, listboxItems1, "List1.txt", "NameList1.txt");

        }

        /// <summary>
        /// Wyświetla drugą listę zakupów.
        /// </summary>
        private void Pivot2_Loaded(object sender, RoutedEventArgs e)
        {
            if (ListboxData.list2Created)
                ViewList(ListboxData.listboxValues2, pivot2, listboxItems2, "List2.txt", "NameList2.txt");
        }

        /// <summary>
        /// Wyświetla trzecią listę zakupów.
        /// </summary>
        private void Pivot3_Loaded(object sender, RoutedEventArgs e)
        {
            if (ListboxData.list3Created)
                ViewList(ListboxData.listboxValues3, pivot3, listboxItems3, "List3.txt", "NameList3.txt");
        }

        /// <summary>
        /// Służy do edytwowania wybranej listy zakupów.
        /// </summary>
        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewList));
            //Ustawiamy na true, żeby strona NewList wiedziała, że nie tworzymy nowej listy tylko edytujemy już istniejącą.
            ListboxData.list1Edited = true; 
        }

        /// <summary>
        /// Służy do edytwowania wybranej listy zakupów.
        /// </summary>
        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewList));
            ListboxData.list2Edited = true;
        }

        /// <summary>
        /// Służy do edytwowania wybranej listy zakupów.
        /// </summary>
        private void btnEdit3_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewList));
            ListboxData.list3Edited = true;
        }

        /// <summary>
        /// Wyświetla nazwę i przedmioty, które są aktualnie na liście.
        /// </summary>
        /// <param name="list">Lista, do której są odczytywane wartości z pliku.</param>
        /// <param name="pivotItem">PivotItem, do którego chcemy się odwołać.</param>
        /// <param name="listBox">ListBox, do którego przekazywane są wartości z listy.</param>
        /// <param name="nameFileList">Nazwa pliku, w którym znajdują się przedmioty z listy zakupów.</param>
        /// <param name="nameFileName">Nazwa pliku, w którym znajduje się nazwa listy.</param>
        public async void ViewList(List<string> list, PivotItem pivotItem, ListBox listBox, string nameFileList, string nameFileName)
        {           
            //Odczytanie nazwy z pliku i wyświetlenie jej.
            StorageFile sampleFile = await ApplicationData.Current.LocalFolder.GetFileAsync(nameFileName);
            pivotItem.Header = await FileIO.ReadTextAsync(sampleFile);

            //Odczytanie elementów listy zakupów z pliku do listy.
            var serializer = new DataContractSerializer(typeof(List<string>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(nameFileList))
            {                
                list = (List<string>)serializer.ReadObject(stream);
            }

            //Przekazanie odczytanych wartości z listy do listBoxa.
            for (int i = 0; i < list.Count; i++)
            {
                listBox.Items.Add(list[i]);
            }
        }

        /// <summary>
        /// Służy do powrotu do głównego menu.
        /// </summary>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Służy do usuwania wybranej listy.
        /// </summary>
        private async void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            await DeleteList(pivot1, "List1.txt", "NameList1.txt");
            ListboxData.list1Created = false; //Dzięki temu, możemy stworzyć nową listę w miejsce pierwszej.
        }

        /// <summary>
        /// Służy do usuwania wybranej listy.
        /// </summary>
        private async void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            await DeleteList(pivot2, "List2.txt", "NameList2.txt");
            ListboxData.list2Created = false;
        }

        /// <summary>
        /// Służy do usuwania wybranej listy.
        /// </summary>
        private async void btnDelete3_Click(object sender, RoutedEventArgs e)
        {
            await DeleteList(pivot3, "List3.txt", "NameList3.txt");
            ListboxData.list3Created = false;        }

        /// <summary>
        /// Odpowiada za usunięcie wybranej listy zakupów i powraca do głównego menu.
        /// </summary>
        /// <param name="pivotItem">PivotItem, do którego chcemy się odwołać.</param>
        /// <param name="nameFileList">Nazwa pliku, w której przechowywane są dane o liście.</param>
        /// <param name="nameFileName">Nazwa pliku, w którym przechowywana jest nazwa listy.</param>
        private async Task DeleteList(PivotItem pivotItem, string nameFileList, string nameFileName)
        {
            var yes = new UICommand("Tak");
            var no = new UICommand("Nie");
            var msg = new MessageDialog("Czy na pewno chcesz usunąć wybraną listę zakupów?", "Uwaga!");
            msg.Commands.Add(yes);
            msg.Commands.Add(no);
            var command = await msg.ShowAsync();
            if (command == yes)
            {
                //Usuwam pliki z danymi.
                StorageFile listData = await ApplicationData.Current.LocalFolder.GetFileAsync(nameFileList);
                StorageFile nameData = await ApplicationData.Current.LocalFolder.GetFileAsync(nameFileName);
                await listData.DeleteAsync(StorageDeleteOption.PermanentDelete);
                await nameData.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                
                pivotItem.Visibility = Visibility.Collapsed;
                this.Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
