using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xam.Forms.VideoPlayer;
using Xamarin.CommunityToolkit.Core;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
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

            kiesVideo();
        }

        async void kiesVideo()
        {

            

            videoElement.Source = globals.videoSource;
        }

        async void upload_clicked(object sender, EventArgs e)
        {
            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            string soort = "Vehicle";
            string beschrijving = editorBeschrijving.Text;

            switch (globals.soort)
            {
                case "wagen":
                    soort = "Vehicle";
                    break;
                case "klant":
                    soort = "Customer";
                    break;
                case "artikel":
                    soort = "Part";
                    break;
                case "werkorder":
                    soort = "Workorder";
                    break;
            }

            string filename = Guid.NewGuid().ToString();

            if (beschrijving == "")
            {
                await DisplayAlert("", "Beschrijving mag niet leeg zijn.", "ok");
            }
            else
            {

                var loginContract = new
                {
                    Type = soort,
                    Id = globals.id,
                    FileName = filename,
                    FileDescription = beschrijving,
                    WebImage = false,
                    WebVisible = false,
                    IsInvoice = false,
                    FileType = "mp4",
                    File = globals.imageBase64
                };


                var content = new StringContent(JsonSerializer.Serialize(loginContract), System.Text.Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/File/PostFile", content);
                responseTask.Wait();
                HttpResponseMessage response = responseTask.Result;
                Task<string> resultTask = response.Content.ReadAsStringAsync();
                resultTask.Wait();
                string result = resultTask.Result;


                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("", "Upload geslaagd.", "ok");
                }
            }
        }

        public void bewerk_clicked(object sender, EventArgs e)
        {
            uploadbutton.IsVisible = true;
        }

        public async void save_clicked(object sender, EventArgs e)
        {

        }

        public async void share_clicked(object sender, EventArgs e)
        {

        }

        public async void verwijder_clicked(object sender, EventArgs e)
        {

        }

    }
}