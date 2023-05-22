using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.CommunityToolkit.Core;
using Xamarin.Forms;

namespace carfacApplicatie
{
    internal class ItemVideo
    {
        INavigation navigation = Application.Current.MainPage.Navigation;
        public string beschrijving { get; set; }
        public FileMediaSource fileMediaSource { get; set; }
        public byte[] bytes { get; set; }
        public string id { get; set; }
        public string datum { get; set; }
    }
}
