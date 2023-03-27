using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using carfacApplicatie.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(save_foto))]
namespace carfacApplicatie.Droid
{
    public class save_foto : ITestInterface
    {
        public async void storePhotoToGallery(byte[] bytes, string fileName)
        {
            Context context = MainActivity.Instance;
            try
            {
                string storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                string path = System.IO.Path.Combine(storagePath, fileName);
                System.IO.File.WriteAllBytes(path, bytes);
                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));

                context.SendBroadcast(mediaScanIntent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}