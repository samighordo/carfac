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
        }

        async void zoek_clicked(object sender, EventArgs e)
        {
            var baseClient = new HttpClient();
            baseClient.DefaultRequestHeaders.TryAddWithoutValidation("CarfacStandardApiJWT", globals.token);

            String s = artikelbar.Text;

            var resultJson = await baseClient.GetAsync("https://dev.carfac.com/standard/api/Part/GetPartById?id=" + s);

            if (resultJson.IsSuccessStatusCode)
            {
                var responseContent = await resultJson.Content.ReadAsStringAsync();

                String[] resultArray = responseContent.Split(',');

                artikel.artikelnummer = resultArray[0].Substring(11).Replace("\"", "");
                artikel.merk = resultArray[8].Substring(10).Replace("\"", "");
                artikel.omschrijving = resultArray[2].Substring(14).Replace("\"", "");

                globals.soort = "artikel";
                globals.id = s;

                Debug.Write(responseContent);







                var baseClient2 = new HttpClient();
                var loginContract = new
                {
                    Type = "Part",
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


public static class artikel
{
    public static String artikelnummer;
    public static String merk;
    public static String omschrijving;
}