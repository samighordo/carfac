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
    public partial class search_werkorder : ContentPage
    {
        public search_werkorder()
        {
            InitializeComponent();
            werkorderbar.SearchButtonPressed += (s, e) =>
            {
                zoek_clicked(s, e);
            };

            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#0081AB");
        }

        async void zoek_clicked(object sender, EventArgs e)
        {
            globals.werkorderItemLijst.Clear();

            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            String s = werkorderbar.Text;
            List<int> list = new List<int>();
            list.Clear();

            if (werkorderbar.Text == null)
            {
                await DisplayAlert("", "De zoekbalk mag niet leeg zijn.", "ok");
            }
            else
            {
                for (int i = 1; i <= 100; i++)
                {
                    if (i.ToString().Contains(s))
                    {
                        list.Add(i);
                    }
                }

                var loginContract = new
                {
                    WorkOrderSequenceNumberList = list
                };
                var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/Workorder/GetWorkorders", content);
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

                            String[] a4 = a3.Split(',');
                            String a5 = a4[1].Substring(26).Replace("\"", "");
                            String a6 = a4[4].Substring(8).Replace("\"", "");
                            globals.werkorderItemLijst.Add(new werkorderItem { naam = a5, nummer = a6 });
                        }
                    }
                    globals.soort = "werkorder";
                    Navigation.PushAsync(new lijst());
                }
                else
                {
                    await DisplayAlert("", "Geen werkorder met dit nummer gevonden.", "ok");
                }
            }

             
        }
    }
}


public static class werkorder
{
    public static String werkordernummer;
    public static String datum;
}

public class werkorderItem
{
    public string naam { get; set; }
    public string nummer { get; set; }
}