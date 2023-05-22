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
            videoElement.Source = globals.videoSource;

            if(globals.fotodoel == "upload")
            {
                uploadbutton.IsVisible = true;
                datum.Text = "";

            }
            else
                datum.Text = globals.fotoDatum;


            if (globals.fotoBeschrijving != null)
            {
                editorBeschrijving.Text = globals.fotoBeschrijving;
            }
        }

        async void kiesVideo()
        {
            videoElement.Source = globals.videoSource;
            videoElement.IsVisible = true;
        }

        async void upload_clicked(object sender, EventArgs e)
        {
            indicator.IsVisible = true;
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
                    var loginContract2 = new
                    {
                        Type = soort,
                        Id = int.Parse(globals.id),
                        Paging = new Paging { StartAtRecord = 1, NumberOfRecords = 100 }
                    };

                    var content2 = new StringContent(JsonSerializer.Serialize(loginContract2), System.Text.Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTask2 = baseClient.PostAsync($"https://dev.carfac.com/standard/api/File/GetFileList", content2);
                    responseTask2.Wait();
                    HttpResponseMessage response2 = responseTask2.Result;
                    Task<string> resultTask2 = response2.Content.ReadAsStringAsync();
                    resultTask2.Wait();
                    string result2 = resultTask2.Result;

                    List<string> list = new List<string>();

                    if (result2 != null && result2 != "\"There was an internal server error: .\"")
                    {
                        globals.lijst.Clear();
                        string[] array1 = result2.Split('[');
                        string[] array2 = array1[1].Split('{');
                        for (int i = 1; i < array2.Length; i++)
                        {
                            string[] array3 = array2[i].Split(',');
                            string idString = array3[0].Substring(9);
                            list.Add(idString);
                        }

                        foreach (string item in list)
                        {

                            var resultJson3 = await baseClient.GetAsync("https://dev.carfac.com/standard/api/File/GetFileById?id=" + item);

                            if (resultJson3.IsSuccessStatusCode)
                            {
                                var responseContent3 = await resultJson3.Content.ReadAsStringAsync();
                                globals.lijst.Add(responseContent3);
                            }
                            else
                            {
                                await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                            }
                        }
                        globals.fotodoel = "";
                        uploadbutton.IsVisible = false;
                        Navigation.PushAsync(new resultaatscherm());

                    }
                    else
                    {
                        await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                    }
                    Navigation.PushAsync(new resultaatscherm());
                }
            }
        }

        public void bewerk_clicked(object sender, EventArgs e)
        {
            uploadbutton.IsVisible = true;
        }

        public async void save_clicked(object sender, EventArgs e)
        {
            DependencyService.Get<ITestInterface>().storePhotoToGallery(globals.bytes, Guid.NewGuid().ToString() + ".mp4");
            await DisplayAlert("", "Download geslaagd.", "ok");
        }

        public async void share_clicked(object sender, EventArgs e)
        {
            if (globals.fotoFullPath == "")
            {
                string naam = Guid.NewGuid().ToString();
                DependencyService.Get<ITestInterface>().storePhotoToGallery(globals.bytes, naam + ".png");

                string storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                string fullPath = storagePath + "/" + naam + ".png";

                globals.fotoFullPath = fullPath;
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "foto",
                    File = new ShareFile(globals.fotoFullPath)
                });
                globals.fotoFullPath = "";
            }
            else
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "foto",
                    File = new ShareFile(globals.fotoFullPath)
                });
                globals.fotoFullPath = "";
            }
        }

        public async void verwijder_clicked(object sender, EventArgs e)
        {
            indicator.IsVisible= true;
            string action = await DisplayActionSheet("Ben je zeker dat je dit wilt verwijderen", "ja", "nee");

            if (action == "ja")
            {
                string soort = "Vehicle";

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

                var baseClient = new HttpClient();
                baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

                string id = globals.fotoId;

                var resultJson = await baseClient.DeleteAsync("https://dev.carfac.com/standard/api/File/DeleteFileById?id=" + id);

                if (resultJson.IsSuccessStatusCode)
                {
                    var loginContract2 = new
                    {
                        Type = soort,
                        Id = int.Parse(globals.id),
                        Paging = new Paging { StartAtRecord = 1, NumberOfRecords = 100 }
                    };

                    var content2 = new StringContent(JsonSerializer.Serialize(loginContract2), System.Text.Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTask2 = baseClient.PostAsync($"https://dev.carfac.com/standard/api/File/GetFileList", content2);
                    responseTask2.Wait();
                    HttpResponseMessage response2 = responseTask2.Result;
                    Task<string> resultTask2 = response2.Content.ReadAsStringAsync();
                    resultTask2.Wait();
                    string result2 = resultTask2.Result;

                    List<string> list = new List<string>();

                    if (result2 != null && result2 != "\"There was an internal server error: .\"")
                    {
                        globals.lijst.Clear();
                        string[] array1 = result2.Split('[');
                        string[] array2 = array1[1].Split('{');
                        for (int i = 1; i < array2.Length; i++)
                        {
                            string[] array3 = array2[i].Split(',');
                            string idString = array3[0].Substring(9);
                            list.Add(idString);
                        }

                        foreach (string item in list)
                        {

                            var resultJson3 = await baseClient.GetAsync("https://dev.carfac.com/standard/api/File/GetFileById?id=" + item);

                            if (resultJson3.IsSuccessStatusCode)
                            {
                                var responseContent3 = await resultJson3.Content.ReadAsStringAsync();
                                globals.lijst.Add(responseContent3);
                            }
                            else
                            {
                                await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                            }
                        }
                        globals.fotodoel = "";
                        uploadbutton.IsVisible = false;
                        Navigation.PushAsync(new resultaatscherm());
                    }
                    else
                    {
                        await DisplayAlert("", "Er ging iets mis, probeer opnieuw.", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("", "Er ging iets mis, probeer opnieuw.", "ok");
                }
            }
        }

    }
}