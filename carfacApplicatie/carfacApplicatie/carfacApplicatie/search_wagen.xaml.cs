using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace carfacApplicatie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class search_wagen : ContentPage
    {
        public search_wagen()
        {
            InitializeComponent();

            

            autobar.SearchButtonPressed += (s, e) =>
            {
                zoek_clicked(s, e);
            };
        }


        async void zoek_clicked(object sender, EventArgs e)
        {

            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            String s = autobar.Text;

            var resultJson = await baseClient.GetAsync("https://dev.carfac.com/standard/api/Vehicle/GetVehicleByID?id=" + s);

            if (resultJson.IsSuccessStatusCode)
            {
                var responseContent = await resultJson.Content.ReadAsStringAsync();

                String[] resultArray = responseContent.Split(',');

                wagen.id = resultArray[0].Substring(13).Replace("\"", "");
                wagen.merk = resultArray[1].Substring(9).Replace("\"", "");
                wagen.model = resultArray[2].Substring(9).Replace("\"", "");
                wagen.nummerplaat = resultArray[3].Substring(16).Replace("\"", "");

                Debug.Write(responseContent);

                globals.soort = "wagen";
                globals.id = autobar.Text;




                var baseClient2 = new HttpClient();
                var loginContract = new
                {
                    Type = "Vehicle",
                    Id = globals.id,
                    Paging = new Paging { StartAtRecord = 1, NumberOfRecords = 100 }
                };


                var content = new StringContent(JsonSerializer.Serialize(loginContract), Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseTask = baseClient.PostAsync($"https://dev.carfac.com/standard/api/File/GetFileList", content);
                responseTask.Wait();
                HttpResponseMessage response = responseTask.Result;
                Task<string> resultTask = response.Content.ReadAsStringAsync();
                resultTask.Wait();
                string result = resultTask.Result;

                Debug.Write(result);

                List<string> list = new List<string>();

                if (result != null && result != "\"There was an internal server error: .\"")
                {
                    globals.lijst.Clear();
                    String[] array1 = result.Split('[');
                    String[] array2 = array1[1].Split('{');
                    for (int i = 1; i < array2.Length; i++)
                    {
                        String[] array3 = array2[i].Split(',');
                        String idString = array3[0].Substring(9);
                        list.Add(idString);
                    }

                    foreach (String item in list)
                    {
                        var baseClient3 = new HttpClient();
                        baseClient3.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

                        var resultJson2 = await baseClient3.GetAsync("https://dev.carfac.com/standard/api/File/GetFileById?id=" + item);

                        if (resultJson.IsSuccessStatusCode)
                        {
                            var responseContent2 = await resultJson2.Content.ReadAsStringAsync();
                            globals.lijst.Add(responseContent2);
                            Debug.Write(responseContent2);
                        }
                    }
                    Navigation.PushAsync(new resultaatscherm());
                }
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

public class Paging
{
    public int StartAtRecord { get; set; }
    public int NumberOfRecords { get; set; }
}