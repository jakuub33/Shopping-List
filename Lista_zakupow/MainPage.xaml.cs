using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Lista_zakupow
{
    /// <summary>
    /// Główne menu aplikacji.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Służy do utworzenia nowej listy zakupów.
        /// </summary>
        private async void btnNewList_Click(object sender, RoutedEventArgs e)
        {            
            //Jeśli jeszcze nie ma utworzonych 3 list to utwórz nową, a jeśli są 3, wyświetl błąd.
            if (!ListboxData.list1Created || !ListboxData.list2Created || !ListboxData.list3Created)
            {
                this.Frame.Navigate(typeof(NewList));
            }            
            else
            {
                //MessageDialog pochodzi z biblioteki Windows.UI.Popups;
                var msg = new MessageDialog("Zostały utworzone już 3 listy zakupów! Zmodyfikuj jedną z nich lub usuń.", "Błąd! ");
                await msg.ShowAsync();   //Jeśli jest await, metoda musi być async - asynchroniczna.                
            }
        }

        /// <summary>
        /// Służy do wyświetlenia utworzonych list oraz operacji na nich.
        /// </summary>
        private void btnSavedList_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SavedLists));
        }

        /// <summary>
        /// Kończy działanie aplikacji.
        /// </summary>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
