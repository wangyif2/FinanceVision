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

namespace FinanceVision
{
    public partial class MainPage : PhoneApplicationPage
    {

        MediaLibrary library;
        SpeechRecognizerUI recoWithUI;
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
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
            ApplicationBarMenuItem AppBarMenu_Speech = new ApplicationBarMenuItem(AppResources.AppBarMenu_Speech);
            AppBarMenu_Speech.Click += AppBarMenu_Speech_Click;
            ApplicationBar.MenuItems.Add(AppBarMenu_Speech);
        }

        private async void AppBarMenu_Speech_Click(object sender, EventArgs e)
        {
            // Create an instance of SpeechRecognizerUI.
            this.recoWithUI = new SpeechRecognizerUI();

            // Start recognition (load the dictation grammar by default).
//            await speechSynthesizer.SpeakTextAsync("");

            SpeechRecognitionUIResult recoResult = await recoWithUI.RecognizeWithUIAsync();

            // Do something with the recognition result.
            MessageBox.Show(string.Format("You said {0}.", recoResult.RecognitionResult.Text));
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