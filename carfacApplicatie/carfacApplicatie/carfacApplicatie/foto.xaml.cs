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
using Android.Webkit;
using Java.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Runtime.InteropServices.ComTypes;
using Android.PrintServices;
using Plugin.Toast;
using Android.Widget;
using Java.Net;
using System.Text.Json.Serialization;
using Xamarin.Essentials;
using static Android.Resource;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class foto : ContentPage
    {
        public foto()
        {
            InitializeComponent();
            relatief.RaiseChild(indicator);

            photo.Source = globals.source;
            editorBeschrijving.Placeholder = "beschrijving";
            datum.Text = globals.fotoDatum;

            if (globals.fotoBeschrijving != null)
            {
                editorBeschrijving.Text = globals.fotoBeschrijving;
            }

            if (globals.fotodoel == "upload")
            {
                uploadbutton.IsVisible = true;
                editorBeschrijving.Placeholder = "beschrijving";
            }
        }

        async void upload_clicked(object sender, EventArgs e)
        {
            if (globals.fotodoel == "upload")
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
                    indicator.IsVisible = true;

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
                                    indicator.IsVisible= false;
                                    await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                                }
                            }
                            globals.fotodoel = "";
                            uploadbutton.IsVisible = false;
                            indicator.IsVisible = false;
                            Navigation.PushAsync(new resultaatscherm());

                        }
                        else
                        {
                            indicator.IsVisible = false;
                            await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                        }
                        indicator.IsVisible = false;
                        Navigation.PushAsync(new resultaatscherm());
                    }
                    else
                    {
                        indicator.IsVisible = false;
                        await DisplayAlert("", "Er ging iets mis, probeer opnieuw.", "ok");

                    }
                }
            }
            else if (globals.fotodoel == "patch")
            {
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

                if (beschrijving == "")
                {
                    await DisplayAlert("", "Beschrijving mag niet leeg zijn.", "ok");
                }
                else
                {
                    indicator.IsVisible = true;
                    var baseClient = new HttpClient();
                    baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

                    var loginContract = new
                    {
                        fileId = globals.fotoId,
                        FileDescription = beschrijving,
                        WebImage = false,
                        WebVisible = false,
                        IsInvoice = false,
                    };

                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginContract), System.Text.Encoding.UTF8, "application/json");


                    var method = new HttpMethod("PATCH");

                    var request = new HttpRequestMessage(method, "https://dev.carfac.com/standard/api/File/PatchFile")
                    {
                        Content = content
                    };

                    var response = await baseClient.SendAsync(request);




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
                                    indicator.IsVisible= false;
                                    await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                                }
                            }
                            globals.fotodoel = "";
                            uploadbutton.IsVisible = false;
                            indicator.IsVisible = false;
                            Navigation.PushAsync(new resultaatscherm());

                        }
                        else
                        {
                            indicator.IsVisible = false;
                            await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                        }
                        indicator.IsVisible = false;
                        Navigation.PushAsync(new resultaatscherm());
                    }
                }
            }
        }

        public void bewerk_clicked(object sender, EventArgs e)
        {
            uploadbutton.IsVisible = true;
            globals.fotodoel = "patch";
        }

        public async void save_clicked(object sender, EventArgs e)
        {
            indicator.IsVisible = true;
            DependencyService.Get<ITestInterface>().storePhotoToGallery(globals.bytes, Guid.NewGuid().ToString() + ".png");
            indicator.IsVisible = false;
            await DisplayAlert("", "Download geslaagd.", "ok");
        }
        
        public async void share_clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "foto",
                File = new ShareFile(globals.fotoFullPath)
            });
        }

        public async void verwijder_clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Ben je zeker dat je dit wilt verwijderen", "ja", "nee");

            if (action == "ja")
            {
                indicator.IsVisible = true;
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
                                indicator.IsVisible = false;
                                await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                            }
                        }
                        globals.fotodoel = "";
                        uploadbutton.IsVisible = false;
                        indicator.IsVisible = false;
                        Navigation.PushAsync(new resultaatscherm());
                    }
                    else
                    {
                        indicator.IsVisible = false;
                        await DisplayAlert("", "Er ging iets mis, probeer opnieuw.", "ok");
                    }
                }
                else
                {
                    indicator.IsVisible = false;
                    await DisplayAlert("", "Er ging iets mis, probeer opnieuw.", "ok");
                }
            }
        }
    }
}
