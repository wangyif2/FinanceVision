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

namespace FinanceVision
{
    public partial class MainPage : PhoneApplicationPage
    {

        MediaLibrary library;
        SpeechRecognizerUI recoWithUI;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();

            LoadDatabase();
            library = new MediaLibrary();
        }

        // Code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem AppBarMenu_DevSetup = new ApplicationBarMenuItem(AppResources.AppBarMenu_DevSetup);
            AppBarMenu_DevSetup.Click += AppBarMenu_DevSetup_Click;
            ApplicationBar.MenuItems.Add(AppBarMenu_DevSetup);

            ApplicationBarMenuItem AppBarMenu_Speech = new ApplicationBarMenuItem(AppResources.AppBarMenu_Speech);
            AppBarMenu_Speech.Click += AppBarMenu_Speech_Click;
            ApplicationBar.MenuItems.Add(AppBarMenu_Speech);
        }

        private async void AppBarMenu_Speech_Click(object sender, EventArgs e)
        {
            // Create an instance of SpeechRecognizerUI.
            this.recoWithUI = new SpeechRecognizerUI();

            // Start recognition (load the dictation grammar by default).
            SpeechRecognitionUIResult recoResult = await recoWithUI.RecognizeWithUIAsync();

            // Do something with the recognition result.
            MessageBox.Show(string.Format("You said {0}.", recoResult.RecognitionResult.Text));
        }

        // This is setup code that loads sample images into the image library 
        // This should be removed before publishing 
        void AppBarMenu_DevSetup_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < 10; i++)
            {
                string filename = "Sample" + i + ".jpg";
                string filelocation = "Images/sample" + i + ".jpg";
                library.SavePictureToCameraRoll(filename, Application.GetResourceStream(new Uri(@filelocation, UriKind.Relative)).Stream);
            }
            
            MessageBox.Show("Dev Setup Complated. Sample images have been loaded");
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddPage.xaml", UriKind.Relative));
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

        private void HubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HubTile ht = (HubTile)sender;
            NavigationService.Navigate(new Uri("/AddPage.xaml?category=" + ht.Title, UriKind.Relative));

        }
    }
}