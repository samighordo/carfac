using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using static Android.Resource;

namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class lijst : ContentPage
    {
        public lijst()
        {
            InitializeComponent();

            switch (globals.soort)
            {
                case "wagen":
                    globals.wagenItemLijst.Sort((s1,s2) => s1.naam.CompareTo(s2.naam));
                    resultaatLijst.ItemsSource = globals.wagenItemLijst;
                    break;
                case "klant":
                    globals.klantItemLijst.Sort((s1, s2) => s1.naam.CompareTo(s2.naam));
                    resultaatLijst.ItemsSource = globals.klantItemLijst;
                    break;
                case "artikel":
                    globals.artikelItemLijst.Sort((s1, s2) => s1.naam.CompareTo(s2.naam));
                    resultaatLijst.ItemsSource = globals.artikelItemLijst;
                    break;
                case "werkorder":
                    globals.werkorderItemLijst.Sort((s1, s2) => s1.naam.CompareTo(s2.naam));
                    resultaatLijst.ItemsSource = globals.werkorderItemLijst;
                    break;
                default:
                    break;
            }
        }

        public async void item_clicked(object sender, ItemTappedEventArgs e)
        {
            string str = "";
            string wagenParameter = "";

            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            switch (globals.soort)
            {
                case "wagen":

                    wagenItem wagenitem = e.Item as wagenItem;

                    if (wagenitem != null)
                    {
                        str = wagenitem.nummer;
                    }


                    var loginContract = new
                    {
                        VinNumber = str
                    };

                    var loginContractNummerplaat = new
                    {
                        licenseplate = str
                    };

                    var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");


                    if (globals.wagenParameter == "nummerplaat")
                    {
                        content = new StringContent(JsonSerializer.Serialize(loginContractNummerplaat), Encoding.UTF8, "application/json");
                    }

                   


                        Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Vehicle/GetVehicles", content);
                        responseTask.Wait();
                        HttpResponseMessage response = responseTask.Result;
                        Task<string> resultTask = response.Content.ReadAsStringAsync();
                    

                    if (response.IsSuccessStatusCode && resultTask.Result != "")
                    {
                        string result = resultTask.Result;
                        System.Diagnostics.Debug.Write("result1 " + result);
                        resultTask.Wait();

                        string[] a = result.Split(',');
                        string id = a[0].Substring(14);
                        globals.id = id;

                        var resultJson = await baseClient.GetAsync("https://dev.carfac.com/standard/api/Vehicle/GetVehicleByID?id=" + id);


                        System.Diagnostics.Debug.Write("result2 " + await resultJson.Content.ReadAsStringAsync());

                        if (resultJson.IsSuccessStatusCode)
                        {
                            var responseContent = await resultJson.Content.ReadAsStringAsync();

                            string[] resultArray = responseContent.Split(',');

                            wagen.id = resultArray[4].Substring(13).Replace("\"", "");
                            wagen.merk = resultArray[1].Substring(9).Replace("\"", "");
                            wagen.model = resultArray[2].Substring(9).Replace("\"", "");
                            wagen.nummerplaat = resultArray[3].Substring(16).Replace("\"", "");

                            globals.soort = "wagen";





                            var loginContract2 = new
                            {
                                Type = "Vehicle",
                                Id = int.Parse(globals.id),
                                Paging = new Paging { StartAtRecord = 1, NumberOfRecords = 100 }
                            };


                            var content2 = new StringContent(JsonSerializer.Serialize(loginContract2), Encoding.UTF8, "application/json");
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
                                Navigation.PushAsync(new resultaatscherm());
                            }
                            else
                            {
                                await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("", "Er ging iets fout, probeer opnieuw.", "ok");
                    }
                    break;
                case "klant":
                    klantItem klantitem = e.Item as klantItem;

                    if (klantitem != null)
                    {
                        str = klantitem.naam;
                    }


                    var loginContractklant = new
                    {
                        Name = str
                    };
                    var contentklant = new StringContent(JsonSerializer.Serialize(loginContractklant), Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTaskklant = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Customer/GetCustomers", contentklant);
                    responseTaskklant.Wait();
                    HttpResponseMessage responseklant = responseTaskklant.Result;

                    if (responseklant.IsSuccessStatusCode)
                    {
                        Task<string> resultTaskklant = responseklant.Content.ReadAsStringAsync();
                        resultTaskklant.Wait();
                        string result = resultTaskklant.Result;
                        string[] a = result.Split(',');
                        string id = a[0].Substring(15);
                        globals.id= id;

                        var resultJson = await baseClient.GetAsync("https://dev.carfac.com/standard/api/Customer/GetCustomerByID?id=" + id);

                        if (resultJson.IsSuccessStatusCode)
                        {
                            var responseContent = await resultJson.Content.ReadAsStringAsync();

                            string[] resultArray = responseContent.Split(',');

                            klant.klantnummer = resultArray[0].Substring(14);
                            klant.naam = resultArray[2].Substring(8).Replace("\"", "");

                            var loginContract2 = new
                            {
                                Type = "Customer",
                                Id = int.Parse(globals.id),
                                Paging = new Paging { StartAtRecord = 1, NumberOfRecords = 100 }
                            };


                            var content2 = new StringContent(JsonSerializer.Serialize(loginContract2), Encoding.UTF8, "application/json");
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
                                }
                                Navigation.PushAsync(new resultaatscherm());
                            }
                        }
                    }
                    break;
                case "werkorder":
                    werkorderItem werkorderitem = e.Item as werkorderItem;

                    if (werkorderitem != null)
                    {
                        str = werkorderitem.naam;
                    }
                    List<int> lijst= new List<int>();
                    lijst.Clear();
                    lijst.Add(int.Parse(str));

                    var loginContractwerkorder = new
                    {
                        WorkOrderSequenceNumberList = lijst
                    };
                    var contentwerkorder = new StringContent(JsonSerializer.Serialize(loginContractwerkorder), Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTaskwerkorder = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Workorder/GetWorkorders", contentwerkorder);
                    responseTaskwerkorder.Wait();
                    HttpResponseMessage responsewerkorder = responseTaskwerkorder.Result;
                    Task<string> resultTaskwerkorder = responsewerkorder.Content.ReadAsStringAsync();
                    string resultwerkorder = resultTaskwerkorder.Result;

                    if (responsewerkorder.IsSuccessStatusCode)
                    {
                        string[] resultArray = resultwerkorder.Split(',');
                        System.Diagnostics.Debug.Write(resultwerkorder);
                        werkorder.werkordernummer = resultArray[1].Substring(26);
                        werkorder.datum = resultArray[4].Substring(8).Replace("\"", "");
                        globals.id = resultArray[0].Substring(16);

                        var loginContract2 = new
                        {
                            Type = "Workorder",
                            Id = int.Parse(globals.id),
                            Paging = new Paging { StartAtRecord = 1, NumberOfRecords = 100 }
                        };


                        var content2 = new StringContent(JsonSerializer.Serialize(loginContract2), Encoding.UTF8, "application/json");
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
                            }
                            Navigation.PushAsync(new resultaatscherm());
                        }

                    }
                    break;
                case "artikel":
                    artikelItem artikelItem = e.Item as artikelItem;

                    if (artikelItem != null)
                    {
                        str = artikelItem.naam;
                    }

                    var loginContractartikel = new
                    {
                        description = str
                    };
                    var contentartikel = new StringContent(JsonSerializer.Serialize(loginContractartikel), Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTaskartikel = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Part/GetParts", contentartikel);
                    responseTaskartikel.Wait();
                    HttpResponseMessage responseartikel = responseTaskartikel.Result;
                    Task<string> resultTaskartikel = responseartikel.Content.ReadAsStringAsync();
                    string resultartikel = resultTaskartikel.Result;

                    if (responseartikel.IsSuccessStatusCode)
                    {
                        string[] arr = resultartikel.Split(',');
                        artikel.artikelnummer = arr[1].Substring(12).Replace("\"", "");
                        artikel.merk = arr[7].Substring(10).Replace("\"", "");
                        artikel.omschrijving = arr[2].Substring(15).Replace("\"", "");
                        globals.id = arr[0].Substring(12).Replace("\"", "");

                        var loginContract2 = new
                        {
                            Type = "Part",
                            Id = int.Parse(globals.id),
                            Paging = new Paging { StartAtRecord = 1, NumberOfRecords = 100 }
                        };


                        var content2 = new StringContent(JsonSerializer.Serialize(loginContract2), Encoding.UTF8, "application/json");
                        Task<HttpResponseMessage> responseTask2 = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Part/GetParts", content2);
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
                            }
                            Navigation.PushAsync(new resultaatscherm());
                        }

                    }
                    break;

                default:
                    break;
            }
        }

        async void toon_popup(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("sorteer", "numeriek", "alfabetisch");

            if (action == "alfabetisch")
            {
                switch (globals.soort)
                {
                    case "wagen":
                        globals.wagenItemLijst.Sort((s1, s2) => s1.naam.CompareTo(s2.naam));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.wagenItemLijst;
                        break;
                    case "klant":
                        globals.klantItemLijst.Sort((s1, s2) => s1.naam.CompareTo(s2.naam));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.klantItemLijst;
                        break;
                    case "artikel":
                        globals.artikelItemLijst.Sort((s1, s2) => s1.naam.CompareTo(s2.naam));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.artikelItemLijst;
                        break;
                    case "werkorder":
                        globals.werkorderItemLijst.Sort((s1, s2) => s1.naam.CompareTo(s2.naam));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.werkorderItemLijst;
                        break;
                    default:
                        break;
                }
            }
            else if (action == "numeriek")
            {
                switch (globals.soort)
                {
                    case "wagen":
                        globals.wagenItemLijst.Sort((s1, s2) => s1.nummer.CompareTo(s2.nummer));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.wagenItemLijst;
                        break;
                    case "klant":
                        globals.klantItemLijst.Sort((s1, s2) => s1.nummer.CompareTo(s2.nummer));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.klantItemLijst;
                        break;
                    case "artikel":
                        globals.artikelItemLijst.Sort((s1, s2) => s1.nummer.CompareTo(s2.nummer));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.artikelItemLijst;
                        break;
                    case "werkorder":
                        globals.werkorderItemLijst.Sort((s1, s2) => s1.nummer.CompareTo(s2.nummer));
                        resultaatLijst.ItemsSource = null;
                        resultaatLijst.ItemsSource = globals.werkorderItemLijst;
                        break;
                    default:
                        break;
                }
            }
        }

       
    }
}