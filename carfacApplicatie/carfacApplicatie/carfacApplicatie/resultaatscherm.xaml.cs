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

        public void afbeelding_clicked(object sender, ItemTappedEventArgs e)
        {
            ItemFoto itemfoto = e.Item as ItemFoto;

            afbeeldinglijst.

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

        async void toon_popup2(object sender, EventArgs e)
        {
            filterPicker.Focus();
        }

        async void kiesPickerOptie(object sender, EventArgs e)
        {
            if(filterPicker.SelectedItem == "beschrijving")
            {
                List<ItemFoto> lijst = this.list;
                List<ItemFoto> lijst2 = this.list2;

                List<ItemFoto> lijst3 = new List<ItemFoto>();
                List<ItemFoto> lijst4 = new List<ItemFoto>();

                string result = await DisplayPromptAsync("beschrijving", "");

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
            filterPicker.SelectedIndex = 2;
        }

        
    }
}