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
    public partial class lijst : ContentPage
    {
        public lijst()
        {
            InitializeComponent();
            List<String> list = new List<String>
            { 
                "gewoon",
                "een",
                "lijst",
                "met",
                "woorden"
            };
            resultaatLijst.ItemsSource = list;
        }

        private void resultaat(object sender, EventArgs e)
        {
            Navigation.PushAsync(new resultaatscherm());
        }
    }
}