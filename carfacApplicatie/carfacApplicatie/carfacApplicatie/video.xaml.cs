using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class video : ContentPage
    {
        public video()
        {
            InitializeComponent();
            videoLijst.ItemsSource = globals.videoHelperLijst;
        }
    }
}