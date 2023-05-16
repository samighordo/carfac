using Android.OS;
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using static Android.Content.ClipData;

namespace carfacApplicatie
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class resultaatscherm : ContentPage
	{
        List<ItemFoto> list;
        List<ItemFoto> list2;

        public resultaatscherm()
		{
			InitializeComponent();

            if (globals.soort == "wagen")
			{
                eerste.Text = wagen.model;
                tweede.Text = "merk = " + wagen.merk;

				if(wagen.id == "")
                    derde.Text = "nummer = /";
				else
                    derde.Text = "nummer = " + wagen.id;

                if (wagen.nummerplaat == "")
					vierde.Text = "nummerplaat = /";
				else
                    vierde.Text = "nummerplaat = " + wagen.nummerplaat;
            }
            else if(globals.soort == "klant")
			{
				eerste.Text = klant.naam;
				tweede.Text = "klantnr = " + klant.klantnummer;
			}
			else if(globals.soort == "artikel")
			{
				eerste.Text = artikel.omschrijving;
				tweede.Text = "merk = " + artikel.merk;
				derde.Text = "artikelnummer = " + artikel.artikelnummer;
			}
			else if(globals.soort == "werkorder")
			{
				eerste.Text = werkorder.werkordernummer;
				tweede.Text ="datum = " + werkorder.datum;
			}

			this.list = new List<ItemFoto>();
            this.list2 = new List<ItemFoto>();

            int i = (globals.lijst.Count / 2);

			for(int j = 0; j<i; j++) 
			{
				// dit is voor de image te verkrijgen
                String[] array = globals.lijst[j].Split(',');
                String s = array[10].Substring(8).Replace("\"", "").Trim();
                s = s.Replace("}", "");

                byte[] imageData = Convert.FromBase64String(s);
                ImageSource imagesource = ImageSource.FromStream(() => new MemoryStream(imageData));

                // dit is voor de beschrijving te verkrijgen
                String b = array[2].Substring(18).Replace("\"", "");

                // dit is voor het id te verkrijgen
                string id = array[0].Substring(10);

                // dit is voor de datum te verkrijgen
                String c = array[7];
                c = c.Substring(16, 10);

                this.list.Add(new ItemFoto { beschrijving = b, ImageSource = imagesource, bytes = imageData, id = id, datum = c });
            }

			for(int j = i; j < globals.lijst.Count; j++)
			{
                // dit is voor de image te verkrijgen
                String[] array = globals.lijst[j].Split(',');
                String s = array[10].Substring(8).Replace("\"", "").Trim();
                s = s.Replace("}", "");

                byte[] imageData = Convert.FromBase64String(s);
                ImageSource imagesource = ImageSource.FromStream(() => new MemoryStream(imageData));

                // dit is voor de beschrijving te verkrijgen
                String b = array[2].Substring(18).Replace("\"", "");

                // dit is voor het id te verkrijgen
                string id = array[0].Substring(10);

                // dit is voor de datum te verkrijgen
                String c = array[7];
                c = c.Substring(16, 10);

                this.list2.Add(new ItemFoto { beschrijving = b, ImageSource = imagesource, bytes = imageData, id = id, datum = c });
            }	

			afbeeldinglijst.ItemsSource = list;
			afbeeldinglijst2.ItemsSource = list2;
			globals.lijstlengte = list.Count + 1;

            globals.fotoFullPath = "";
        }

        async Task kiesfoto(Object sender, EventArgs e)
		{
			var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
			{
				Title = "kies een foto"
			});

            String path = result.FullPath;

            var stream = await result.OpenReadAsync();

            globals.source = new FileImageSource { File = path };

            byte[] imageBytes = File.ReadAllBytes(path);
            string base64String = Convert.ToBase64String(imageBytes);

            globals.imageBase64 = base64String;
            globals.fotodoel = "upload";
            globals.fotoBeschrijving = "";
            globals.fotoFullPath = path;

            Navigation.PushAsync(new foto());
        }

		async Task maakFoto(Object sender, EventArgs e)
		{
			var result = await MediaPicker.CapturePhotoAsync();

			String path = result.FullPath;

			var stream = await result.OpenReadAsync();

			globals.source = new FileImageSource { File = path };

            byte[] imageBytes = File.ReadAllBytes(path);
            string base64String = Convert.ToBase64String(imageBytes);

			globals.imageBase64 = base64String;
			globals.fotodoel = "upload";
            globals.fotoBeschrijving = "";
            globals.fotoFullPath = path;

            Navigation.PushAsync(new foto());

        }

        async Task maakVideo(Object sender, EventArgs e)
        {
            var result = await MediaPicker.CaptureVideoAsync();

            String path = result.FullPath;

            var stream = await result.OpenReadAsync();

            globals.videoSource = new FileMediaSource { File = path };

            byte[] imageBytes = File.ReadAllBytes(path);
            string base64String = Convert.ToBase64String(imageBytes);

            globals.imageBase64 = base64String;
            globals.fotodoel = "upload";
            globals.fotoBeschrijving = "";
            globals.fotoFullPath = path;

            Navigation.PushAsync(new video());

        }

        async Task kiesVideo(Object sender, EventArgs e)
        {
            var filename = await MediaPicker.PickVideoAsync(new MediaPickerOptions
            {
                Title = "kies een video"
            });

            String path = filename.FullPath;

            globals.videoSource = new FileMediaSource { File = path };

            byte[] imageBytes = File.ReadAllBytes(path);
            string base64String = Convert.ToBase64String(imageBytes);

            globals.imageBase64 = base64String;

            Navigation.PushAsync(new video());
        }

        public void afbeelding_clicked(object sender, ItemTappedEventArgs e)
        {
            ItemFoto itemfoto = e.Item as ItemFoto;

            if (itemfoto != null)
            {
                globals.fotoId = itemfoto.id;
                globals.bytes = itemfoto.bytes;
                globals.fotoDatum = itemfoto.datum;
                globals.source = itemfoto.ImageSource;
                globals.fotoBeschrijving = itemfoto.beschrijving;
            }

            Navigation.PushAsync(new foto());
        }

		async void toon_popup(object sender, EventArgs e)
		{
            string action = await DisplayActionSheet("Wat wil je doen?", "cancel", null, "maak foto", "maak video", "kies foto", "kies video");

            if (action == "kies foto")
                kiesfoto(sender, e);
            else if (action == "maak foto")
                maakFoto(sender, e);
            else if (action == "kies video")
                kiesVideo(sender, e);
            else if (action == "maak video")
                maakVideo(sender, e);
        }

        

        async void filter_beschrijving(object sender, EventArgs e)
        {
            List<ItemFoto> lijst = this.list;
            List<ItemFoto> lijst2 = this.list2;

            List<ItemFoto> lijst3 = new List<ItemFoto>();
            List<ItemFoto> lijst4 = new List<ItemFoto>();

            string result = await DisplayPromptAsync("beschrijving", "");

            if(result != null)
            {
                foreach (ItemFoto item in lijst)
                {

                    if (item.beschrijving.Contains(result))
                    {
                        lijst3.Add(item);
                    }
                }
                foreach (ItemFoto item in lijst2)
                {

                    if (item.beschrijving.Contains(result))
                    {
                        lijst4.Add(item);
                    }
                }

                if (lijst3.Count == 0 && lijst4.Count != 0)
                {
                    lijst3.Add(lijst4.Last());
                    lijst4.RemoveAt(lijst4.Count - 1);
                }

                afbeeldinglijst.ItemsSource = null;
                afbeeldinglijst.ItemsSource = lijst3;

                afbeeldinglijst2.ItemsSource = null;
                afbeeldinglijst2.ItemsSource = lijst4;
            }
        }
    }

    public class VideoHelperKlasse
    {
        public ImageSource videoSource { get; set; }
    }
}