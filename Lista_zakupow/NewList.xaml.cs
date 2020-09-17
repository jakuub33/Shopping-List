using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.IO.IsolatedStorage;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.Threading.Tasks;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Lista_zakupow
{
    /// <summary>
    /// Strona odpowiadająca za utworzenie listy i operacje na niej.
    /// </summary>
    public sealed partial class NewList : Page
    {      
        public NewList()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled; //pamiec podreczna, po zmianie strony, stan listy jest pamietany
        }

        /// <summary>
        /// Dodaje przedmiot do listy zakupów.
        /// </summary>
        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Sprawdzam czy lista nie jest pełna, przy pomocy ProgressBar.
            if (pbProgress.Value < pbProgress.Maximum)  //Domyślnie można dodać 5 przedmiotów. (Oczywiście można to zwiększyć)
            {
                if (txtItem.Text.Length > 0)  //Czy została podana nazwa przedmiotu?
                {
                    if (listBoxPurchases.Items.Contains(txtItem.Text)) //Sprawdzam, czy na liście znajduje się taki sam przedmiot.
                    {
                        //Jeśli tak, dostaniemy informacje czy na pewno chcemy dodać ten sam przedmiot.
                        var yes = new UICommand("Tak");
                        var no = new UICommand("Nie");
                        var msg = new MessageDialog("Czy chcesz dodać ten sam przedmiot?", "Uwaga! Ten przedmiot znajduje się już na liście!");                        
                        msg.Commands.Add(yes);
                        msg.Commands.Add(no);                        
                        var command = await msg.ShowAsync();
                        if (command == no)
                        {
                            txtItem.Text = "";
                            return;
                        }                                             
                    }
                    listBoxPurchases.Items.Add(txtItem.Text);
                    ChangeProgress();
                    txtItem.Text = "";
                }
                else
                {                    
                    var msg = new MessageDialog("Nic nie zostało wpisane!", "Błąd!");
                    await msg.ShowAsync();
                }
            }
            else
            {
                var yes = new UICommand("Tak");
                var no = new UICommand("Nie");
                var msg = new MessageDialog("Czy chcesz powiększyć listę o 5 rzeczy więcej?", "Błąd! Lista jest pełna!");
                msg.Commands.Add(yes);
                msg.Commands.Add(no);
                var command = await msg.ShowAsync();
                if (command == yes)
                {
                    txtItem.Text = "";
                    pbProgress.Maximum += 50; //Uwaga! Zwiększamy tak na prawdę maksymalną ilość przedmiotów na liście o kolejne 5.
                }
            }
        }

        /// <summary>
        /// Po każdym dodaniu lub usunięciu przedmiotu aktualizowana jest ilość przedmiotów na liście widoczna na ProgressBar.
        /// </summary>
        private void ChangeProgress()
        {
            int i = listBoxPurchases.Items.Count;
            pbProgress.Value = i * 10; //Wartości ProgressBaru mnożę przez 10, aby lepiej był widoczny efekt po dodaniu/usunięciu czegoś.
            //Domyślnie maximum ProgressBaru ustawione jest na 50, czyli możemy dodać 5 przedmiotów.
        }

        /// <summary>
        /// Usuwa zaznaczony przedmiot z listy zakupów.
        /// </summary>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (pbProgress.Value > 0)
            {
                int i = listBoxPurchases.SelectedIndex;
                if (i != -1) //-1 oznacze, że coś nie jest zaznaczone
                {
                    listBoxPurchases.Items.RemoveAt(i);
                    ChangeProgress();
                }
                else
                {
                    var msg = new MessageDialog("Najpierw musisz coś zaznaczyć!", "Błąd!");
                    await msg.ShowAsync();
                }
            }
            else
            {
                var msg = new MessageDialog("Najpierw musisz coś dodać!", "Błąd!"); //Jeśli nie ma nic na liscie.
                await msg.ShowAsync();
            }
        }

        /// <summary>
        /// Usuwa wszystkie przedmioty z listy zakupów.
        /// </summary>
        private async void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (pbProgress.Value > 0)
            {
                var yes = new UICommand("Tak");
                var no = new UICommand("Nie");
                var msg = new MessageDialog("Czy na pewno chcesz wyczyścić listę?", "Uwaga!");
                msg.Commands.Add(yes);
                msg.Commands.Add(no);
                var command = await msg.ShowAsync();
                if (command == no)
                {
                    return;
                }
                else if (command == yes)
                {
                    listBoxPurchases.Items.Clear();
                    ChangeProgress();
                }
            }
            else
            {
                var msg = new MessageDialog("Lista jest już pusta!", "Błąd");
                await msg.ShowAsync();
            }
                   
        }        

        /// <summary>
        /// Zapisuję listę zakupów i wraca do poprzedniej strony.
        /// </summary>
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {            
            //To jest na przykład moja ścieżka, gdzie znajduję się folder LocalState z naszymi utworzonymi plikami.
            //C:\Users\Kubson\AppData\Local\Packages\19aaaf20-ae1d-4f63-b37e-4d7a2db20b78_psxb0myrhy436\LocalState    

            if (txtName.Text.Length > 0)
            {
                //Najpierw sprawdzam czy lista jest już utworzona.
                if (await isFileExists("List1.txt")) //Sprawdzam, czy w naszym folderze znajdują się pliki z danymi o liście.
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

                //Jeśli dana lista jest już utworzona to listCreated jest true, analogicznie listEdited.
                if (ListboxData.list1Edited)
                {                    
                    SaveList(ListboxData.listboxValues1, "List1.txt", "NameList1.txt", txtName.Text);                                      
                }
                else if (ListboxData.list2Edited)
                {
                    SaveList(ListboxData.listboxValues2, "List2.txt", "NameList2.txt", txtName.Text);                    
                }
                else if (ListboxData.list3Edited)
                {
                    SaveList(ListboxData.listboxValues3, "List3.txt", "NameList3.txt", txtName.Text);                    
                }
                //Do tego momentu sprawdzam czy któraś z list jest edytowana, a niżej czy dopiero tworzona.
                //-----------------------------------------------------------
                else if (!ListboxData.list1Created)
                //Dzięki temu nie będą utworzone więcej niż 3 listy. A jeśli, któraś zostanie usunięta to nowa lista wskoczy na te miejsce.
                {
                    SaveList(ListboxData.listboxValues1, "List1.txt", "NameList1.txt", txtName.Text);                    
                    ListboxData.list1Created = true;     
                }
                else if (!ListboxData.list2Created)
                {
                    SaveList(ListboxData.listboxValues2, "List2.txt", "NameList2.txt", txtName.Text);
                    ListboxData.list2Created = true;
                }
                else if (!ListboxData.list3Created)
                {
                    SaveList(ListboxData.listboxValues3, "List3.txt", "NameList3.txt", txtName.Text);
                    ListboxData.list3Created = true;
                } 
                else
                {
                    var msg = new MessageDialog("Zostały utworzone już 3 listy zakupów! Zmodyfikuj jedną z nich lub usuń.", "Błąd!");
                    await msg.ShowAsync();
                }
            }
            else
            {
                var msg = new MessageDialog("Najpierw u góry podaj nazwę listy!", "Błąd!");
                await msg.ShowAsync();
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
        /// Elementy z listBoxa przekazywane są do odpowiedniej Listy (Array), dzięki czemu możemy te dane przechowywać w osobnej klasie.
        /// Dodatkowo te dane z Listy zapisujemy do pliku.
        /// </summary>
        /// <param name="list">Lista, do której chcesz się odwołać.</param>
        /// <param name="nameFileList">Nazwa pliku, w której przechowywane są dane o liście.</param>
        /// <param name="nameList">Nazwa listy nadana podczas tworzenia.</param>        
        /// <param name="nameFileName">Nazwa pliku, w którym przechowywana jest nazwa listy.</param>
        private async void SaveList(List<string> list, string nameFileList, string nameFileName, string nameList)
        {            
            list.Clear();
            //Przekazywanie danych z listBoxa do listy.
            for (int i = 0; i < listBoxPurchases.Items.Count; i++)
            {
                list.Add((string)listBoxPurchases.Items[i]);                             
            }

            //Przekazywanie nazwy do pliku .txt
            StorageFile nameData = await ApplicationData.Current.LocalFolder.CreateFileAsync
                (nameFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(nameData, nameList);
            //Dzięki drugiemu parametrowi, jeśli edytujemy dane, to aktualny plik zostanie nadpisany, a nie utworzony nowy.

            //Przekazywanie danych z listy do pliku .txt
            StorageFile listData = await ApplicationData.Current.LocalFolder.CreateFileAsync
                (nameFileList, CreationCollisionOption.ReplaceExisting);             

            IRandomAccessStream randStream = await listData.OpenAsync(FileAccessMode.ReadWrite);
            using (IOutputStream outStream = randStream.GetOutputStreamAt(0))
            {
                //Stan sesji. 

                //Przekazujemy dane w struemieniu określonego typu, u nas jest to List<string>
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<string>));                

                //Zapisujemy dane do strumienia/pliku
                serializer.WriteObject(outStream.AsStreamForWrite(), list);                            

                //Zwolnienie zasobów
                await outStream.FlushAsync();
                outStream.Dispose(); 
                randStream.Dispose();
            }

            ListboxData.list1Edited = false; ListboxData.list2Edited = false; ListboxData.list3Edited = false; 
            //Ponieważ konczymy edytowanie wybranej listy.
            this.Frame.GoBack();
        }

        /// <summary>
        /// Wraca do poprzedniej strony, bez zapisywania.
        /// </summary>
        private async void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ListboxData.list1Edited = false; ListboxData.list2Edited = false; ListboxData.list3Edited = false;
            var yes = new UICommand("Tak");
            var no = new UICommand("Nie");
            var msg = new MessageDialog("Czy zapisałeś listę i na pewno chcesz wyjść?", "Uwaga!");
            msg.Commands.Add(yes);
            msg.Commands.Add(no);
            var command = await msg.ShowAsync();
            if (command == yes)
            {
                this.Frame.GoBack();
            }            
        }

        /// <summary>
        /// Odpowiada za załadowanie listy przechowywanymi danymi jeśli edytujemy listę.
        /// </summary>
        private void ListBoxPurchases_Loaded(object sender, RoutedEventArgs e)
        {
            //Jeśli lista zakupów jest edytowana, a nie tworzona to wyswietl jej zawartość.
            if (ListboxData.list1Edited)
            {
                ViewList(ListboxData.listboxValues1, listBoxPurchases, "NameList1.txt", "List1.txt");                
            }
            else if (ListboxData.list2Edited)
            {
                ViewList(ListboxData.listboxValues2, listBoxPurchases, "NameList2.txt", "List2.txt");                
            }
            else if (ListboxData.list3Edited)
            {
                ViewList(ListboxData.listboxValues3, listBoxPurchases, "NameList3.txt", "List3.txt");                
            }            
        }

        /// <summary>
        /// Wyświetla nazwę edytowanej listy oraz wpisuję do listBoxa dane, które zostały podane wcześniej.
        /// </summary>
        /// <param name="list">Lista, z której przekazywane są wartości do ListBoxa.</param>
        /// <param name="listBox">ListBox, w którym znajdują się przedmioty z listy zakupów.</param>
        /// <param name="nameFileName">Nazwa pliku, w którym znajduje się nazwa listy.</param>
        /// <param name="nameFileList">Nazwa pliku, w którym znajdują się przedmioty z listy zakupów.</param>
        public async void ViewList(List<string> list, ListBox listBox, string nameFileName, string nameFileList)
        {
            //Odczytanie nazwy z pliku i wyświetlenie jej.
            StorageFile sampleFile = await ApplicationData.Current.LocalFolder.GetFileAsync(nameFileName);
            txtName.Text = await FileIO.ReadTextAsync(sampleFile);

            //Odczytanie elementów listy zakupów z pliku do listy.
            var serializer = new DataContractSerializer(typeof(List<string>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(nameFileList))
            {
                list = (List<string>)serializer.ReadObject(stream);
            }

            //Przekazanie elementów z listy do listboxa.
            for (int i = 0; i < list.Count; i++)
            {
                listBox.Items.Add(list[i]);
            }
            ChangeProgress();
        }
    }
}
