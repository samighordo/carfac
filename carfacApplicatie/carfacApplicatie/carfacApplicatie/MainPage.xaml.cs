using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Net.WebRequestMethods;

namespace carfacApplicatie
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        void GetToken(object sender, EventArgs e)
        {
            var baseClient = new HttpClient();
            var loginContract = new
            {
                Username = "SamiStage.CarfacMStage.StageSami",
                Password = "qHBCnfvg%bbGGyvzkIixPZ!%",
                HdcDealerCode = 129364
            };
            var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/User/Login", content);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;
            Task<string> resultTask = response.Content.ReadAsStringAsync();
            resultTask.Wait();
            string result = resultTask.Result;
           
            globals.token = result;

            Debug.Write(result);

            Navigation.PushAsync(new homepage());
        }

    }
}

public static class globals
{
    public static String token;
    public static String soort;
    public static String id;
    public static ImageSource source;
    public static String imageBase64;
    public static List<string> lijst = new List<string>();
    public static String fotodoel;

    public static int lijstlengte;
    public static String fotoBeschrijving;
    public static byte[] bytes;
    public static List<String> gefilterdeResultatenLijst = new List<string>();

    public static List<klantItem> klantItemLijst = new List<klantItem>();
    public static List<wagenItem> wagenItemLijst = new List<wagenItem>();
    public static List<werkorderItem> werkorderItemLijst = new List<werkorderItem>();
    public static List<artikelItem> artikelItemLijst = new List<artikelItem>();

    public static String wagenParameter;

}