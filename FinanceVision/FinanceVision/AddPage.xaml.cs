using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using FinanceVision.Resources;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

namespace FinanceVision
{
    public partial class AddPage : PhoneApplicationPage
    {
        private PhotoChooserTask photoChooser;
        private SpeechRecognizerUI recoWithUI;
        private SpeechSynthesizer speechSynthesizer;

        public AddPage()
        {
            //See photoChooser_Completed for reason why this is commented out
            InitializeComponent();
            BuildLocalizedApplicationBar();

            //cam = new CameraCaptureTask();
            //cam.Completed += cam_Completed;

            photoChooser = new PhotoChooserTask();
            photoChooser.ShowCamera = true;
            photoChooser.Completed += photoChooser_Completed;

            speechSynthesizer = new SpeechSynthesizer();
        }

        // Code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton_Save = new ApplicationBarIconButton(new Uri("/Images/save.png", UriKind.Relative));
            appBarButton_Save.Text = AppResources.AppBarButton_Save;
            appBarButton_Save.Click += SaveButton_Click;
            ApplicationBar.Buttons.Add(appBarButton_Save);

            ApplicationBarIconButton appBarButton_Cancel = new ApplicationBarIconButton(new Uri("/Images/cancel.png", UriKind.Relative));
            appBarButton_Cancel.Text = AppResources.AppBarButton_Cancel;
            appBarButton_Cancel.Click += CancelButton_Click;
            ApplicationBar.Buttons.Add(appBarButton_Cancel);

            ApplicationBarIconButton appBarButton_Speak = new ApplicationBarIconButton(new Uri("/Images/microphone.png", UriKind.Relative));
            appBarButton_Speak.Text = AppResources.AppBarButton_Speak;
            appBarButton_Speak.Click += SpeakButton_Click;
            ApplicationBar.Buttons.Add(appBarButton_Speak);
        }

        private async void SpeakButton_Click(object sender, EventArgs e)
        {
            try
            {
                await speechSynthesizer.SpeakTextAsync("Say the item name");
                this.recoWithUI = new SpeechRecognizerUI();
                recoWithUI.Recognizer.Grammars.AddGrammarFromPredefinedType("webSearch", SpeechPredefinedGrammar.WebSearch);
                SpeechRecognitionUIResult recoResultName = await recoWithUI.RecognizeWithUIAsync();
                Name.Text = recoResultName.ResultStatus == SpeechRecognitionUIStatus.Succeeded ? recoResultName.RecognitionResult.Text : "Unknown";

                if (recoResultName.ResultStatus != SpeechRecognitionUIStatus.Cancelled)
                {
                    await speechSynthesizer.SpeakTextAsync("Say the item price");
                    this.recoWithUI = new SpeechRecognizerUI();
                    SpeechRecognitionUIResult recoResultPrice = await recoWithUI.RecognizeWithUIAsync();
                    Amount.Text = GetOnlyNumberFromSpeech(recoResultPrice);
                }
            }
            catch
            {
            }
        }

        private String GetOnlyNumberFromSpeech(SpeechRecognitionUIResult recoResultPrice)
        {
            String resultString = recoResultPrice.ResultStatus == SpeechRecognitionUIStatus.Succeeded ? recoResultPrice.RecognitionResult.Text : "0";
            try
            {
                Regex regexObj = new Regex(@"[^\d]");
                resultString = regexObj.Replace(resultString, "");
            }
            catch (ArgumentException ex)
            {
                // Syntax error in the regular expression
            }
            return resultString;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Name.Text) || !string.IsNullOrEmpty(Amount.Text))
            {
                MessageBoxResult result = MessageBox.Show("Are you sure...?", "", MessageBoxButton.OKCancel);
                if(result == MessageBoxResult.OK && NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Name.Text) || string.IsNullOrWhiteSpace(Name.Text) ||
                string.IsNullOrEmpty(Amount.Text) || string.IsNullOrWhiteSpace(Amount.Text))
            {
                MessageBox.Show("Something is missing...", "", MessageBoxButton.OK);
            }
            else
            {
                string DBConnectionString = "Data Source=isostore:/ReceiptDatabase.sdf";

                // Add entry to database with user input
                using (ReceiptDatabase db = new ReceiptDatabase(DBConnectionString))
                {

                    // Prepopulate the categories.

                    db.entries.InsertOnSubmit(new ReceiptEntry
                        {
                            EntryName = Name.Text,
                            EntryPrice = float.Parse(Amount.Text),
                            EntryCategory = new ActivityCategory { Name = CategoryPicker.SelectedItem.ToString() },
                            EntryDate = DateTime.Parse(Date.Value.ToString())
                        });

                    // Save categories to the database.
                    db.SubmitChanges();
                }

                // Load database update
                MainPage.viewModel.LoadEntriesFromDatabase();

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        void photoChooser_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                AddImageButton.Visibility = System.Windows.Visibility.Collapsed;
                ImageButton.Visibility = System.Windows.Visibility.Visible;

                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                myImage.Source = bmp;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string selectedCategory;
            if (NavigationContext.QueryString.TryGetValue("category", out selectedCategory))
            {
                CategoryPicker.SelectedItem = selectedCategory;
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            photoChooser.Show();
        }

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            string datetime = e.NewDateTime.Value.ToShortDateString();
        }

    }
}