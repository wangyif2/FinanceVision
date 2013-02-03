using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FinanceVision.Resources;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Windows.Media.Imaging;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace FinanceVision
{
    public partial class MainPage : PhoneApplicationPage
    {
<<<<<<< HEAD
        public static ReceiptViewModel viewModel;
        MediaLibrary library;
=======
>>>>>>> a620ef4d390874fb223ab0731441166eded242b4
        SpeechRecognizerUI recoWithUI;
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
            LoadDatabase();
        }

        internal void LoadDatabase()
        {
            ReceiptViewModel viewModel;

            // Load database and display
            string DBConnectionString = "Data Source=isostore:/ReceiptDatabase.sdf";
            viewModel = new ReceiptViewModel(DBConnectionString);
            viewModel.LoadEntriesFromDatabase();
            DataContext = viewModel;
        }

        // Code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem AppBarMenu_Settings = new ApplicationBarMenuItem(AppResources.AppBarMenu_Settings);
            AppBarMenu_Settings.Click += AppBarMenu_Settings_Click;
            ApplicationBar.MenuItems.Add(AppBarMenu_Settings);
        }

        private void AppBarMenu_Settings_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
<<<<<<< HEAD
        
        internal void LoadDatabase()
        {
            // Load database and display
            string DBConnectionString = "Data Source=isostore:/ReceiptDatabase.sdf";
            viewModel = new ReceiptViewModel(DBConnectionString);
            viewModel.LoadEntriesFromDatabase();
            DataContext = viewModel;
        }
=======

>>>>>>> a620ef4d390874fb223ab0731441166eded242b4

        private void HubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HubTile ht = (HubTile)sender;
            NavigationService.Navigate(new Uri("/AddPage.xaml?category=" + ht.Title, UriKind.Relative));
        }

        private void HubTile_Hold(object sender, GestureEventArgs e)
        {
            CreateLiveTile((HubTile)sender);
        }

        private void CreateLiveTile(HubTile hubtile)
        {

            StandardTileData LiveTile = new StandardTileData
                {
                    BackgroundImage = ((System.Windows.Media.Imaging.BitmapImage)hubtile.Source).UriSource,
                    Title = hubtile.Title,
                    BackTitle = hubtile.Title,
                    BackContent = hubtile.Message
                };
            ShellTile Tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=" + LiveTile.Title));
            if (Tile == null)
            {
                try
                {
                    ShellTile.Create(new Uri("/AddPage.xaml?category=" + hubtile.Title, UriKind.Relative), LiveTile);
                }
                catch (Exception)
                {
                    MessageBox.Show("I prefer not to be pinned");
                }
            }
            else MessageBox.Show("The tile is already pinned");
        }
    }
}