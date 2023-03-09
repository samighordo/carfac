using Android.Media;
using Android.OS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Stream = System.IO.Stream;
using System.Text.Json;
using Android.Text.Format;
using Android.Content;
using Android.Graphics;
using NativeMedia;
using System.Collections;
using System.Security.Cryptography;

namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class foto : ContentPage
    {
        public foto()
        {
            InitializeComponent();
            photo.Source = globals.source;

            if(globals.fotodoel == "upload")
            {
                uploadbutton.IsVisible = true;
                editorBeschrijving.Placeholder = "beschrijving";
            }
            else
            {
                editorBeschrijving.Text = globals.fotoBeschrijving;
            }
        }

        async void upload_clicked(object sender, EventArgs e)
        {
            if(globals.fotodoel == "upload")
            {
                var baseClient = new HttpClient();
                baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

                String soort = "Vehicle";
                String beschrijving = editorBeschrijving.Text;

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

                String filename = soort + "foto" + globals.lijstlengte.ToString();

                var loginContract = new
                {
                    Type = soort,
                    Id = globals.id,
                    FileName = filename,
                    FileDescription = beschrijving,
                    WebImage = false,
                    WebVisible = false,
                    IsInvoice = false,
                    FileType = "jpg",
                    File = globals.imageBase64
                };



                var content = new StringContent(JsonSerializer.Serialize(loginContract), System.Text.Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/File/PostFile", content);
                responseTask.Wait();
                HttpResponseMessage response = responseTask.Result;
                Task<string> resultTask = response.Content.ReadAsStringAsync();
                resultTask.Wait();
                string result = resultTask.Result;

                Navigation.PushAsync(new resultaatscherm());

                System.Diagnostics.Debug.WriteLine(result);
            }
        }

        public void bewerk_clicked(object sender, EventArgs e)
        {
            uploadbutton.IsVisible= true;
        }

        public async void save_clicked(object sender, EventArgs e)
        {
            string s = System.Text.Encoding.UTF8.GetString(globals.bytes);
            System.Diagnostics.Debug.Write("bytes = " + s);

            
        }

    }
}