using System;
using System.IO;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Converters {
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity {
		readonly string[] permissionGroup =
		{
			Manifest.Permission.ReadExternalStorage,
			Manifest.Permission.WriteExternalStorage,
			Manifest.Permission.AccessNetworkState,
			Manifest.Permission.Internet
		};
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

			FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += FabOnClick;

			// Запрашиваем разрешения.
			RequestPermissions(permissionGroup, 0);
		}

		public override bool OnCreateOptionsMenu(IMenu menu) {
			MenuInflater.Inflate(Resource.Menu.menu_main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item) {
			int id = item.ItemId;
			if (id == Resource.Id.action_settings) {
				return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		private void FabOnClick(object sender, EventArgs eventArgs) {
			View view = (View)sender;
			var text = FindViewById<EditText>(Resource.Id.editText1);
			var sb = new StringBuilder();
			sb.Append("button cliced" + System.Environment.NewLine);

			string dirPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.CanonicalPath, "testFolder");
			sb.Append("selected folder : " + dirPath + System.Environment.NewLine);

			var res = Seacher.SearchInFolder(dirPath, Seacher.FileExtensions.img, "Задание");

			sb.Append(res.Count);
			text.Text = sb.ToString();
		}
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

