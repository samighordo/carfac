using Java.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class search_artikel : ContentPage
    {
        public search_artikel()
        {
            InitializeComponent();
            artikelbar.SearchButtonPressed += (s, e) =>
            {
                zoek_clicked(s, e);
            };

            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#0081AB");
        }

        async void zoek_clicked(object sender, EventArgs e)
        {
            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            globals.artikelItemLijst.Clear();

            String s = artikelbar.Text;

            if (artikelbar.Text == null)
            {
                await DisplayAlert("", "De zoekbalk mag niet leeg zijn.", "ok");
            }
            else
            {
                var loginContract = new
                {
                    description = "%" + s + "%"
                };
                var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Part/GetParts", content);
                responseTask.Wait();
                HttpResponseMessage response = responseTask.Result;
                Task<string> resultTask = response.Content.ReadAsStringAsync();

                String result = resultTask.Result;

                if (response.IsSuccessStatusCode)
                {
                    result = result.Replace("]", "").Trim();
                    String[] a = result.Split('}');

                    foreach (String str in a)
                    {
                        if (str != "")
                        {
                            String ss = str.Substring(1);
                            String[] arr = ss.Split(',');
                            Debug.Write(str);

                            String artikelnummer = arr[1].Substring(12).Replace("\"", "");
                            String omschrijving = arr[2].Substring(15).Replace("\"", "");
                            globals.artikelItemLijst.Add(new artikelItem { naam = omschrijving, nummer = "nr = " + artikelnummer });
                        }
                    }


                    globals.soort = "artikel";
                    Navigation.PushAsync(new lijst());
                }
                else
                {
                    await DisplayAlert("", "Geen artikel met deze beschrijving gevonden.", "ok");
                }
            }


           
        }
    }
}


public static class artikel
{
    public static String artikelnummer;
    public static String merk;
    public static String omschrijving;
}

public class artikelItem
{
    public string naam { get; set; }
    public string nummer { get; set; }
}