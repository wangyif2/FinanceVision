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

namespace FinanceVision
{
    public partial class MainPage : PhoneApplicationPage
    {
<<<<<<< HEAD
        private ReceiptViewModel viewModel;
=======
        MediaLibrary library;
>>>>>>> 061563f98d74ecc61736a08f9ca242277db24b6e
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
<<<<<<< HEAD

            string DBConnectionString = "Data Source=isostore:/ReceiptDatabase.sdf";
            viewModel = new ReceiptViewModel(DBConnectionString);
            viewModel.LoadEntriesFromDatabase();
            DataContext = viewModel;
=======
            library = new MediaLibrary();
>>>>>>> 061563f98d74ecc61736a08f9ca242277db24b6e
        }

        // Code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton_add = new ApplicationBarIconButton(new Uri("/Images/add.png", UriKind.Relative));
            appBarButton_add.Text = AppResources.AppBarButton_Add;
            appBarButton_add.Click += AddButton_Click;
            ApplicationBar.Buttons.Add(appBarButton_add);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem AppBarMenu_DevSetup = new ApplicationBarMenuItem(AppResources.AppBarMenu_DevSetup);
            AppBarMenu_DevSetup.Click += AppBarMenu_DevSetup_Click;
            ApplicationBar.MenuItems.Add(AppBarMenu_DevSetup);
        }

        // This is setup code that loads sample images into the image library 
        // This should be removed before publishing 
        void AppBarMenu_DevSetup_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < 9; i++)
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
    }
}