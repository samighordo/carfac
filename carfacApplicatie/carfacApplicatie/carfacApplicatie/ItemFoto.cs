using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace carfacApplicatie
{
    internal class ItemFoto
    {
        INavigation navigation = Application.Current.MainPage.Navigation;
        public string beschrijving { get; set; }
        public ImageSource ImageSource { get; set; }
        public byte[] bytes { get; set; }
        public string id { get; set; }
        public string datum { get; set; }

        
    }
}
