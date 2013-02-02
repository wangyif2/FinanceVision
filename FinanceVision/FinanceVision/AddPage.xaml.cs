using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using FinanceVision.Resources;

namespace FinanceVision
{
    public partial class AddPage : PhoneApplicationPage
    {
        //private CameraCaptureTask cam;
        private PhotoChooserTask photoChooser; 

        public AddPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            //cam = new CameraCaptureTask();
            //cam.Completed += cam_Completed;

            photoChooser = new PhotoChooserTask();
            photoChooser.ShowCamera = true;
            photoChooser.Completed += photoChooser_Completed;
        }

        // Code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton_Confirm = new ApplicationBarIconButton(new Uri("/Images/check.png", UriKind.Relative));
            appBarButton_Confirm.Text = AppResources.AppBarButton_Confirm;
            appBarButton_Confirm.Click += ConfirmButton_Click;
            ApplicationBar.Buttons.Add(appBarButton_Confirm);

            ApplicationBarIconButton appBarButton_Cancel = new ApplicationBarIconButton(new Uri("/Images/cancel.png", UriKind.Relative));
            appBarButton_Cancel.Text = AppResources.AppBarButton_Confirm;
            appBarButton_Cancel.Click += CancelButton_Click;
            ApplicationBar.Buttons.Add(appBarButton_Cancel);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ivan Please Add Stuff Here");
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ivan Please Add Stuff Here");
        }

        void photoChooser_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                myImage.Source = bmp;

                OcrClient ocrClient = new OcrClient();
                ocrClient.StartOcrConversion(e.ChosenPhoto);
            }
        }

        void cam_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                myImage.Source = bmp;
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                photoChooser.Show();
                //cam.Show();
            }
        }

    }
}