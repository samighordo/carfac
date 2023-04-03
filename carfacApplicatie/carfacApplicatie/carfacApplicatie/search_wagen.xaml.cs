using Rg.Plugins.Popup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Resource;
using Color = Xamarin.Forms.Color;
using String = System.String;

namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class search_wagen : ContentPage
    {

        public search_wagen()
        {
            InitializeComponent();

            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#0081AB");

            autobar.SearchButtonPressed += (s, e) =>
            {
                zoek_clicked(s, e);
            };
        }


        async void zoek_clicked(object sender, EventArgs e)
        {
            globals.wagenItemLijst.Clear();

            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            String s = autobar.Text;
            String str = "";

           if(autobar.Text == null)
            {
                await DisplayAlert("", "De zoekbalk mag niet leeg zijn.", "ok");
            }
            else
            {
                if (autobar.Placeholder == "nummerplaat")
                {
                    var loginContract = new
                    {
                        Licensceplate = "%" + s + "%"
                    };
                    var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Vehicle/GetVehicles", content);
                    responseTask.Wait();
                    HttpResponseMessage response = responseTask.Result;
                    Task<string> resultTask = response.Content.ReadAsStringAsync();

                    string result = resultTask.Result;

                    if (response.IsSuccessStatusCode)
                    {
                        result = result.Replace("]", "");
                        String[] a = result.Split('}');
                        foreach (String a2 in a)
                        {
                            if (a2 != "")
                            {
                                String a3 = a2.Substring(1);

                                String a5 = "";
                                String a6 = "";

                                String[] a4 = a3.Split(',');
                                if (a4[4].Substring(13).Replace("\"", "") != "" && a4[1].Substring(9).Replace("\"", "") != "" && a4[3].Substring(15).Replace("\"", "") != "")
                                {
                                    a5 = a4[3].Substring(15).Replace("\"", "");
                                    a6 = a4[1].Substring(9).Replace("\"", "");
                                    globals.wagenItemLijst.Add(new wagenItem { naam = a6, nummer = a5 });
                                }
                            }
                        }
                        globals.soort = "wagen";
                        globals.wagenParameter = "nummerplaat";
                        Navigation.PushAsync(new lijst());
                    }
                    else
                    {
                        await DisplayAlert("", "Geen wagen met dit nummer gevonden.", "ok");
                    }
                }
                else
                {
                    var loginContract = new
                    {
                        VinNumber = "%" + s + "%"
                    };
                    var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Vehicle/GetVehicles", content);
                    responseTask.Wait();
                    HttpResponseMessage response = responseTask.Result;
                    Task<string> resultTask = response.Content.ReadAsStringAsync();

                    string result = resultTask.Result;
                    Debug.Write(result);

                    if (response.IsSuccessStatusCode)
                    {
                        result = result.Replace("]", "");
                        String[] a = result.Split('}');
                        foreach (String a2 in a)
                        {
                            if (a2 != "")
                            {
                                String a3 = a2.Substring(1);
                                Debug.Write(a3);

                                String[] a4 = a3.Split(',');
                                String a5 = a4[4].Substring(13).Replace("\"", "");
                                String a6 = a4[1].Substring(9).Replace("\"", "");
                                globals.wagenItemLijst.Add(new wagenItem { naam = a6, nummer = a5 });
                            }
                        }
                        globals.soort = "wagen";
                        globals.wagenParameter = "wagennummer";
                        Navigation.PushAsync(new lijst());
                    }
                    else
                    {
                        await DisplayAlert("", "Geen wagen met dit nummer gevonden.", "ok");
                    }
                }
            }

           
        }

        async void toon_popup(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Kies een optie", "Vinnummer", "nummerplaat");

            if (action == "Vinnummer")
            {
                autobar.Placeholder = "Vinnummer";
            }
            else if (action == "nummerplaat")
            {
                autobar.Placeholder = "nummerplaat";
            }
        }
    }
}

public static class wagen
{
    public static String id;
    public static String merk;
    public static String model;
    public static String nummerplaat;
}

public class wagenItem
{
    public string naam { get; set; }
    public string nummer { get; set; }
}

public class Paging
{
    public int StartAtRecord { get; set; }
    public int NumberOfRecords { get; set; }
}