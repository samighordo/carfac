using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace carfacApplicatie
{
    internal class ItemFoto
    {
        public string beschrijving { get; set; }
        public ImageSource ImageSource { get; set; }
        public byte[] bytes { get; set; }
        public string id { get; set; }
        public string datum { get; set; }
    }
}
