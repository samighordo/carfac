using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("LilitaOne-Regular.ttf", Alias = "lilita")]
[assembly: ExportFont("TiltWarp-Regular-VariableFont_XROT,YROT.ttf", Alias = "tiltwarp")]


namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class homepage : ContentPage
    {
        public homepage()
        {
            InitializeComponent();
            
        }



        private void wagen_clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new search_wagen());
        }
        private void artikel_clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new search_artikel());
        }
        private void klant_clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new search_klant());
        }
        private void werkorder_clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new search_werkorder());
        }
    }
}