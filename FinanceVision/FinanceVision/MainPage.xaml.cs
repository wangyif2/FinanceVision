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
using System.IO.IsolatedStorage;

namespace FinanceVision
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// Provides easy key-value pair storage for app settings
        private IsolatedStorageSettings AppSettings = IsolatedStorageSettings.ApplicationSettings;
        public static ReceiptViewModel viewModel;
        SpeechRecognizerUI recoWithUI;
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
            LoadDatabase();
            LoadGoals();
        }

        private void LoadGoals()
        {
            string mGoal = string.Empty;
            string wGoal = string.Empty;
            try
            {
                mGoal = (string)AppSettings["MonthGoal"];
                wGoal = (string)AppSettings["WeekGoal"];
            }
            catch (KeyNotFoundException e)
            {
                AppSettings.Add("MonthGoal", mGoal);
                AppSettings.Add("WeekGoal", wGoal);
            }
            if (!String.IsNullOrEmpty(mGoal))
            {
                EditMonthGoal_TextBlock.Text = "my goal is to stay under $" + mGoal;
                EditMonthGoal.Visibility = System.Windows.Visibility.Visible;
                AddMonthGoal.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (!String.IsNullOrEmpty(wGoal))
            {
                EditWeekGoal_TextBlock.Text = "my goal is to stay under $" + wGoal;
                EditWeekGoal.Visibility = System.Windows.Visibility.Visible;
                AddWeekGoal.Visibility = System.Windows.Visibility.Collapsed;
            }
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

        internal void LoadDatabase()
        {
            // Load database and display
            string DBConnectionString = "Data Source=isostore:/ReceiptDatabase.sdf";
            viewModel = new ReceiptViewModel(DBConnectionString);
            viewModel.LoadEntriesFromDatabase();
            DataContext = viewModel;
        }

        private void AddGoal_Tapped(object sender, EventArgs e)
        {
            PhoneTextBox phonetb = (PhoneTextBox)sender;
            Button bt = (Button)FindName(phonetb.Name.Replace("Add", "Edit"));
            string amount = phonetb.Text;
            if (!String.IsNullOrEmpty(amount))
            {
                // update
                if (phonetb.Name.Contains("Month"))
                    AppSettings["MonthGoal"] = amount;
                else
                    AppSettings["WeekGoal"] = amount;
            }
            else
            {
                if (phonetb.Name.Contains("Month"))
                    amount = (string)AppSettings["MonthGoal"];
                else
                    amount = (string)AppSettings["WeekGoal"];
            }

            if (!String.IsNullOrEmpty(amount))
            {
                ((TextBlock)bt.FindName(bt.Name + "_TextBlock")).Text = "my goal is to stay under $" + amount;
                phonetb.Visibility = System.Windows.Visibility.Collapsed;
                bt.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void EditGoal_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            bt.Visibility = System.Windows.Visibility.Collapsed;
            PhoneTextBox ptb = (PhoneTextBox)FindName(bt.Name.Replace("Edit", "Add"));
            ptb.Visibility = System.Windows.Visibility.Visible;
            ptb.Focus();
        }

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