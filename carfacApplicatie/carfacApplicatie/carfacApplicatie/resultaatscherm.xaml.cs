using Android.OS;
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		public resultaatscherm()
		{
			InitializeComponent();

			if(globals.soort == "wagen")
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

			List<ItemFoto> list = new List<ItemFoto>();
            List<ItemFoto> list2 = new List<ItemFoto>();


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



                list.Add(new ItemFoto { beschrijving = b, ImageSource = imagesource, bytes = imageData });
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



                list2.Add(new ItemFoto { beschrijving = b, ImageSource = imagesource, bytes = imageData });
            }	

			afbeeldinglijst.ItemsSource = list;
			afbeeldinglijst2.ItemsSource = list2;
			globals.lijstlengte = list.Count + 1;

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

            Navigation.PushAsync(new foto());
        }

		async Task neemfoto(Object sender, EventArgs e)
		{
			var result = await MediaPicker.CapturePhotoAsync();

			String path = result.FullPath;

			var stream = await result.OpenReadAsync();

			globals.source = new FileImageSource { File = path };

            byte[] imageBytes = File.ReadAllBytes(path);
            string base64String = Convert.ToBase64String(imageBytes);

			globals.imageBase64 = base64String;
			globals.fotodoel = "upload";

            Navigation.PushAsync(new foto());

        }

        private void afbeelding_clicked(object sender, ItemTappedEventArgs e)
        {
            
            ItemFoto itemfoto = e.Item as ItemFoto;

            if (itemfoto != null)
            {
				globals.bytes = itemfoto.bytes;
				globals.source = itemfoto.ImageSource;
				globals.fotoBeschrijving = itemfoto.beschrijving;
            }

            Navigation.PushAsync(new foto());
        }

		async void toon_popup(object sender, EventArgs e)
		{
            string action = await DisplayActionSheet("Wat wil je doen?", "upload foto uit gallerij", "neem foto");

			if (action == "upload foto uit gallerij")
				kiesfoto(sender, e);
			else if(action == "neem foto")
				neemfoto(sender, e);
        }

        public void OnDelete(object sender, EventArgs e)
        {
            DisplayAlert("Delete Context Action", " delete context action", "OK");
        }
    }
}