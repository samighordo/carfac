using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using System.Collections.Generic;
using Android;
using Android.Util;
using System.Diagnostics;
using Xamarin.Essentials;
using System.Xml.Serialization;
using Android.Content;

namespace carfacApplicatie.Droid
{
    [Activity(Label = "carfacApplicatie", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            var required_permissions = new[]
            {
                Manifest.Permission.AccessNetworkState,
                Manifest.Permission.Camera,
                Manifest.Permission.ManageExternalStorage,
                Manifest.Permission.ReadMediaImages,
                Manifest.Permission.ReadMediaVideo,
                Manifest.Permission.ReadMediaAudio,
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage,
                Manifest.Permission.ManageMedia
            };
            var permissions = new List<string>();
            foreach (var perm in required_permissions)
            {
                if (ContextCompat.CheckSelfPermission(this, perm) != Permission.Granted)
                    permissions.Add(perm);
            }
            if (permissions.Count != 0)
                ActivityCompat.RequestPermissions(this, permissions.ToArray(), 1);

            base.OnCreate(savedInstanceState);
            NativeMedia.Platform.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

            protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
            {
                if(NativeMedia.Platform.CheckCanProcessResult(requestCode, resultCode, data))
                {
                    NativeMedia.Platform.OnActivityResult(requestCode, resultCode, data);
                }

            base.OnActivityResult(requestCode, resultCode, data);
            }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}