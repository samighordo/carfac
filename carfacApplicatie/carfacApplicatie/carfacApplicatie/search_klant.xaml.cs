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
using static Java.Util.Jar.Attributes;

namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class search_klant : ContentPage
    {
        public search_klant()
        {
            InitializeComponent();
            klantbar.SearchButtonPressed += (s, e) =>
            {
                zoek_clicked(s, e);
            };

            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#0081AB");
        }

        async void zoek_clicked(object sender, EventArgs e)
        {
            globals.klantItemLijst.Clear();

            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            String s = klantbar.Text;
            List<int> list = new List<int>();
            list.Clear();

            if (klantbar.Text == null)
            {
                await DisplayAlert("", "De zoekbalk mag niet leeg zijn.", "ok");
            }
            else
            {
                indicator.IsVisible = true;
                for (int i = 1; i <= 100; i++)
                {
                    if (i.ToString().Contains(s))
                    {
                        list.Add(i);
                    }
                }

                var loginContract = new
                {
                    CustomerIdList = list
                };
                var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Customer/GetCustomers", content);
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
                            Debug.WriteLine(a3);

                            String[] a4 = a3.Split(',');
                            String a5 = a4[2].Substring(8).Replace("\"", "");
                            String a6 = a4[0].Substring(14).Replace("\"", "");
                            globals.klantItemLijst.Add(new klantItem { naam = a5, nummer = "id = " + a6 });
                        }
                    }
                    globals.soort = "klant";
                    indicator.IsVisible= false;
                    Navigation.PushAsync(new lijst());
                }
                else
                {
                    indicator.IsVisible = false;
                    await DisplayAlert("", "Geen klant met dit nummer gevonden.", "ok");
                }
            }

            
        }
    }
}

public static class klant
{
    public static String naam;
    public static String klantnummer;
}

public class klantItem
{
    public string naam { get; set; }
    public string nummer { get; set; }
}