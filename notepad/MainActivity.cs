using Android;
using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Media;
using AndroidHUD;
using AlertDialog = Android.App.AlertDialog;
using Button = Android.Widget.Button;
using File = System.IO.File;
using View = Android.Views.View;


namespace notepad
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        CultureInfo cl = new CultureInfo("en-US");
        private string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private int id;
        private Button menuChoose;
        private LinearLayout tempView;
        private EditText title;
        private EditText text;
        private EditText input;
        private Button newItem;
        private Button addAudio;
        private Button fordeleteButton;
        private Button delFromBdButton;
        private DBhelper MDbhelper;
        //private DBhelper DBhelper;
        private ImageView gallery;
        private ImageView camera;
        private Button goToMenu;
        private static Dialog dialog;
        private static int CAMERA_PERMISSION_CODE = 100;
        private static int STORAGE_PERMISSION_CODE = 101;
        private static int STORAGE2_PERMISSION_CODE = 102;
        private Uri selectedImage;
        private bool longclicked = false;
        private Button add, delete, remake;
        private Button menuListAdd;
        private ImageView bigimage, tempimage;
        private Picture picture;
        private ImageView tmpView;
        private List<Texts> textses = new List<Texts>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            LoadMenu();
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, STORAGE_PERMISSION_CODE);
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, STORAGE2_PERMISSION_CODE);
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ManageDocuments }, 103);
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission_group.Storage }, 104);
            if (File.Exists(path + "/id.txt"))
            {
                id = int.Parse(File.ReadAllText(path + "/id.txt"));
            }
            else
            {
                File.WriteAllText(path + "/id.txt", id.ToString());
            }

        }



        private void LoadMenu()
        {
            SetContentView(Resource.Layout.activity_menu);
            menuListAdd = FindViewById<Button>(Resource.Id.listadd);
            menuListAdd.Click += MenuListAdd_Click;
            delFromBdButton = FindViewById<Button>(Resource.Id.deletelist);
            delFromBdButton.Click += DelFromBdButton_Click;
            MDbhelper = new DBhelper(this);
            SQLiteDatabase database = MDbhelper.WritableDatabase;
            ContentValues contentValues = new ContentValues();
            //contentValues.Put(MDbhelper.LKEY_TEXT, );
            LoadFromBD();
        }

        private void DelFromBdButton_Click(object sender, EventArgs e)
        {
            //AndHUD.Shared.Show(this, "Please, wait", 100, MaskType.Black, TimeSpan.FromSeconds(5));
            FindViewById<Button>(Resource.Id.listadd).Visibility = ViewStates.Visible;
            FindViewById<Button>(Resource.Id.deletelist).Visibility = ViewStates.Invisible;
            FindViewById<LinearLayout>(Resource.Id.menuitemlist).RemoveAllViews();
            DeleteFromBD();
            //AndHUD.Shared.Dismiss(this);
        }

        private void MenuListAdd_Click(object sender, EventArgs e)
        {

            tempView = FindViewById<LinearLayout>(Resource.Id.menuitemlist);
            newItem = new Button(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(tempView.Width, 150)
            };
            newItem.SetPadding(0, 10, 0, 10);
            newItem.TextSize = 22;
            newItem.SetBackgroundResource(Resource.Drawable.LabelStyle);
            tempView.AddView(newItem);
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Input new list label");
            input = new EditText(this);
            alert.SetView(input);
            alert.SetPositiveButton("Ok", Positive_Click);
            alert.SetNegativeButton("Cancel", Negative_Click);
            alert.Show();
            id++;
            File.Delete(path + "/id.txt");
            File.WriteAllText(path + "/id.txt", id.ToString());


        }

        private void Negative_Click(object sender, DialogClickEventArgs e)
        {
            tempView.RemoveView(newItem);
        }

        private void Positive_Click(object sender, DialogClickEventArgs e)
        {
            menuChoose = newItem;
            string label;
            newItem.Text = input.Text;
            label = newItem.Text;
            SetContentView(Resource.Layout.activity_main);
            NewList();
            FindViewById<EditText>(Resource.Id.newlabel).Text = label;
        }

        private void NewList()
        {
            SetContentView(Resource.Layout.activity_main);
            add = FindViewById<Button>(Resource.Id.addbutton);
            add.Click += Add_Click;
            delete = FindViewById<Button>(Resource.Id.deletebutton);
            delete.Click += Delete_Click;
            remake = FindViewById<Button>(Resource.Id.remakebutton);
            remake.Click += Remake_Click;
            bigimage = FindViewById<ImageView>(Resource.Id.bigimage);
            bigimage.Click += Bigimage_Click;
            goToMenu = FindViewById<Button>(Resource.Id.menubutton);
            goToMenu.Click += GoToMenu_Click;
            title = FindViewById<EditText>(Resource.Id.newlabel);
            title.TextChanged += MainActivity_TextChanged;
            text = FindViewById<EditText>(Resource.Id.text);
            text.TextChanged += MainActivity_TextChanged;
            addAudio = FindViewById<Button>(Resource.Id.addaudio);
            addAudio.Click += AddAudio_Click;
            AndHUD.Shared.ShowSuccess(this, "Loaded successfully", MaskType.Black, TimeSpan.FromMilliseconds(1000));
            //DBhelper = new DBhelper(this);
        }

        private void AddAudio_Click(object sender, EventArgs e)
        {
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.RecordAudio }, 105);
            Button newAudio = new Button(this);
            newAudio.TextAlignment = TextAlignment.ViewEnd;
            newAudio.Hover += NewAudio_Hover;
            FindViewById<LinearLayout>(Resource.Id.audiolinear).AddView(newAudio);
        }

        private void NewAudio_Hover(object sender, View.HoverEventArgs e)
        {
            var t = (Button) sender;
            string filename = (t.Id).ToString();
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            MediaRecorder recorder = new MediaRecorder();
            recorder.SetAudioSource(AudioSource.Mic);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetOutputFile(filename);
            recorder.Prepare();
            recorder.Start();

        }



        private void SaveTextToBD()
            {
                //AndHUD.Shared.Show(this, "Please, wait", 100, MaskType.Black, TimeSpan.FromSeconds(5));
                var database = MDbhelper.WritableDatabase;
                ContentValues contentValues = new ContentValues();
                contentValues.Put(DBhelper.TKEY_ID, id);
                contentValues.Put(DBhelper.TKEY_LABEL, title.Text);
                contentValues.Put(DBhelper.TKEY_TEXT, text.Text);
                if (textses.Any(x => x.id == id))
                {
                    database.Update(DBhelper.TABLE_TEXT, contentValues, DBhelper.TKEY_ID + "= ?", new string[] { id.ToString() });
                }
                else
                {
                    database.Insert(DBhelper.TABLE_TEXT, null, contentValues);
                }
                database.Close();
                TextLoadHelper();
                //AndHUD.Shared.Dismiss(this);
            }


            private void SaveToBD()
            {
                AndHUD.Shared.Show(this, "Please, wait", 100, MaskType.Black, TimeSpan.FromSeconds(5));
                var database = MDbhelper.WritableDatabase;
                ContentValues contentValues;
                database.BeginTransaction();
                try
                {
                    contentValues = new ContentValues();
                    contentValues.Put(DBhelper.TKEY_ID, id);
                    contentValues.Put(DBhelper.TKEY_LABEL, title.Text);
                    contentValues.Put(DBhelper.TKEY_TEXT, text.Text);
                    if (textses.Any(x => x.id == id))
                    {
                        database.Update(DBhelper.TABLE_TEXT, contentValues, DBhelper.TKEY_ID + "= ?",
                            new string[] { id.ToString() });
                    }
                    else
                    {
                        database.Insert(DBhelper.TABLE_TEXT, null, contentValues);
                    }

                    database.SetTransactionSuccessful();
                }
                finally
                {
                    database.EndTransaction();
                }
                LinearLayout linear = FindViewById<LinearLayout>(Resource.Id.imagelinear);
                List<View> imgs = new List<View>();
                for (int j = 0; j < linear.ChildCount; j++)
                {
                    imgs.Add(linear.GetChildAt(j));
                }

                if (textses.Any(x => x.id == id))
                {
                    database.Delete(DBhelper.TABLE_IMAGE, DBhelper.ITKEY_ID + "= ?", new string[] { id.ToString() });
                }
                database.BeginTransaction();
                try
                {
                    imgs.ForEach(x =>
                {
                    contentValues = new ContentValues();
                    contentValues.Put(DBhelper.ITKEY_ID, id);
                    contentValues.Put(DBhelper.IKEY_IMAGE, ((BitmapDrawable)((ImageView)x).Drawable).Bitmap.strim());
                    database.Insert(DBhelper.TABLE_IMAGE, null, contentValues);
                });
                    database.SetTransactionSuccessful();
                }
                finally
                {
                    database.EndTransaction();
                }
                database.Close();
                LoadHelper();
                //AndHUD.Shared.Dismiss(this);
                AndHUD.Shared.ShowSuccess(this, "Saved successfully", MaskType.Black, TimeSpan.FromMilliseconds(500));
            }

            private void TextLoadHelper()
            {
                textses.Clear();
                var database = MDbhelper.WritableDatabase;
                var cursor = database.Query(DBhelper.TABLE_TEXT, null, null, null, null, null, null);
                if (cursor.MoveToFirst())
                {
                    int idIndex = cursor.GetColumnIndex(DBhelper.TKEY_ID);
                    int nameIndex = cursor.GetColumnIndex(DBhelper.TKEY_LABEL);
                    int textIndex = cursor.GetColumnIndex(DBhelper.TKEY_TEXT);
                    do
                    {
                        textses.Add(new Texts()
                        {
                            id = cursor.GetInt(idIndex),
                            title = cursor.GetString(nameIndex),
                            text = cursor.GetString(textIndex)
                        });
                    } while (cursor.MoveToNext());
                }

                cursor.Close();
                MDbhelper.Close();
            }

            private void LoadHelper()
            {
                textses.Clear();
                var database = MDbhelper.WritableDatabase;
                var cursor = database.Query(DBhelper.TABLE_TEXT, null, null, null, null, null, null);
                if (cursor.MoveToFirst())
                {
                    int idIndex = cursor.GetColumnIndex(DBhelper.TKEY_ID);
                    int nameIndex = cursor.GetColumnIndex(DBhelper.TKEY_LABEL);
                    int textIndex = cursor.GetColumnIndex(DBhelper.TKEY_TEXT);
                    do
                    {
                        textses.Add(new Texts()
                        {
                            id = cursor.GetInt(idIndex),
                            title = cursor.GetString(nameIndex),
                            text = cursor.GetString(textIndex)
                        });
                    } while (cursor.MoveToNext());
                }

                cursor.Close();
                cursor = database.Query(DBhelper.TABLE_IMAGE, null, null, null, null, null, null);
                if (cursor.MoveToFirst())
                {
                    int idIndex = cursor.GetColumnIndex(DBhelper.IKEY_ID);
                    int tidIndex = cursor.GetColumnIndex(DBhelper.ITKEY_ID);
                    int imageIndex = cursor.GetColumnIndex(DBhelper.IKEY_IMAGE);
                    do
                    {
                        int tid = cursor.GetInt(tidIndex);
                        textses.Find(x => x.id == tid).AddImage(cursor.GetInt(idIndex), cursor.GetBlob(imageIndex));
                    } while (cursor.MoveToNext());
                }
                cursor.Close();
                MDbhelper.Close();
            }

            private void LoadFromBD()
            {
                //AndHUD.Shared.Show(this, "Please, wait", 100, MaskType.Black, TimeSpan.FromSeconds(5));
                LoadHelper();
                LinearLayout linear = FindViewById<LinearLayout>(Resource.Id.menuitemlist);
                foreach (var item in textses)
                {
                    Button newButton = new Button(this)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(1030, 150)
                    };
                    newButton.SetPadding(0, 30, 0, 30);
                    newButton.LongClick += NewButton_LongClick;
                    newButton.TextSize = 22;
                    newButton.SetBackgroundResource(Resource.Drawable.LabelStyle);
                    newButton.Id = item.id;
                    newButton.Text = item.title;
                    newButton.Click += NewButton_Click;
                    linear.AddView(newButton);
                }
                //AndHUD.Shared.Dismiss(this);
            }

            private void DeleteFromBD()
            {
                var database = MDbhelper.WritableDatabase;
                if (textses.Any(x => x.id == fordeleteButton.Id))
                {
                    database.Delete(DBhelper.TABLE_TEXT, DBhelper.TKEY_ID + "= ?", new string[] { fordeleteButton.Id.ToString() });
                    database.Delete(DBhelper.TABLE_IMAGE, DBhelper.ITKEY_ID + "= ?", new string[] { fordeleteButton.Id.ToString() });
                }
                database.Close();
                LoadFromBD();
            }

            private void NewButton_LongClick(object sender, View.LongClickEventArgs e)
            {
                FindViewById<Button>(Resource.Id.listadd).Visibility = ViewStates.Invisible;
                FindViewById<Button>(Resource.Id.deletelist).Visibility = ViewStates.Visible;
                fordeleteButton = (Button)sender;
                fordeleteButton.SetBackgroundResource(Resource.Drawable.waitingfordelete);
                FindViewById<RelativeLayout>(Resource.Id.zakrivashka).Visibility = ViewStates.Visible;
                FindViewById<RelativeLayout>(Resource.Id.zakrivashka).Alpha = 0.01f;
                FindViewById<RelativeLayout>(Resource.Id.zakrivashka).SetBackgroundResource(Resource.Color.button_material_dark);
                FindViewById<RelativeLayout>(Resource.Id.zakrivashka).Click += MainActivity_Click;
            }

            private void MainActivity_Click(object sender, EventArgs e)
            {
                FindViewById<RelativeLayout>(Resource.Id.zakrivashka).Visibility = ViewStates.Invisible;
                FindViewById<Button>(Resource.Id.listadd).Visibility = ViewStates.Visible;
                FindViewById<Button>(Resource.Id.deletelist).Visibility = ViewStates.Invisible;
                fordeleteButton.SetBackgroundResource(Resource.Drawable.LabelStyle);
            }

            private void NewButton_Click(object sender, EventArgs e)
            {
                SetContentView(Resource.Layout.activity_main);
                NewList();
                var b = (Button)sender;
                id = b.Id;
                var txt = textses.Find(x => x.id == b.Id);
                text.Text = txt.text;
                title.Text = txt.title;
                txt.images.ForEach(x =>
                {
                    var layout = FindViewById<LinearLayout>(Resource.Id.imagelinear);
                    var newImage = new ImageView(this)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(250, 455)
                    };
                    newImage.SetPadding(5, 30, 5, 30);

                    layout.AddView(newImage);
                    newImage.Click += Image_Click;
                    newImage.LongClick += Image_LongClick;
                    newImage.SetImageBitmap(x.GetBitmap());
                });
            }

            private void MainActivity_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
            {
                SaveTextToBD();
            }

            private void GoToMenu_Click(object sender, EventArgs e)
            {
                SaveToBD();
                //AndHUD.Shared.Dismiss(this);
                SetContentView(Resource.Layout.activity_menu);
                LoadMenu();
            }

            private void Image_LongClick(object sender, EventArgs e)
            {
                //longclicked = true;
                delete.Visibility = ViewStates.Visible;
                remake.Visibility = ViewStates.Visible;
                tmpView = (ImageView)sender;
            }

            private async void Remake_Click(object sender, EventArgs e)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera }, CAMERA_PERMISSION_CODE);
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Choose");
                alert.SetMessage("Choose what to use");
                alert.SetPositiveButton("Camera", Camera_Click);
                alert.SetNegativeButton("Gallery", Gallery_Click);
                dialog = alert.Create();
                dialog.Show();
                delete.Visibility = ViewStates.Invisible;
                remake.Visibility = ViewStates.Invisible;
                //SaveToBD();
            }

            //private void Cancel_Add_Image()
            //{
            //    FindViewById<HorizontalScrollView>(Resource.Id.horizontalScrollView).RemoveView(tmpView);
            //    Toast.MakeText(this, "Sorry, but your image is to large", ToastLength.Long);
            //}

            private void Ok_Click(object sender, EventArgs e)
            {
                dialog.Dispose();
            }

            private async void Gallery_Click(object sender, EventArgs e)
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    dialog.Dismiss();
                    // Context context = this;
                    MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                    AndHUD.Shared.Show(this, "Please, wait", 100, MaskType.Black, TimeSpan.FromSeconds(5));
                    if (photo != null)
                    {
                        Bitmap bitmap = BitmapFactory.DecodeFile(photo.Path);
                        if (bitmap.Height > 4500 || bitmap.Width > 3500)
                        {
                            //await Task.Delay(5000);
                            //Cancel_Add_Image();
                            AlertDialog.Builder alert = new AlertDialog.Builder(this);
                            alert.SetTitle("Error");
                            alert.SetMessage("Image is too large");
                            alert.SetPositiveButton("Ok", Ok_Click);
                            dialog = alert.Create();
                            dialog.Show();
                            //AndHUD.Shared.ShowError(context,"Image is too large",MaskType.Black,TimeSpan.FromSeconds(1.5));
                        }
                        else
                        {
                            var layout = FindViewById<LinearLayout>(Resource.Id.imagelinear);
                            var newImage = new ImageView(this)
                            {
                                LayoutParameters = new ViewGroup.LayoutParams(250, 455)
                            };
                            newImage.SetPadding(5, 5, 5, 5);
                            layout.AddView(newImage);
                            newImage.Click += Image_Click;
                            newImage.LongClick += Image_LongClick;
                            tmpView = newImage;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 30, ms);
                                bitmap = BitmapFactory.DecodeByteArray(ms.ToArray(), 0, ms.ToArray().Length);
                            }
                            //Android.Net.Uri Uri = Android.Net.Uri.Parse(photo.Path);


                            tmpView.Background = null;
                            tmpView.LayoutChange += NewImage_LayoutChange;
                            tmpView.SetImageBitmap(bitmap);
                            tmpView.LayoutChange -= NewImage_LayoutChange;
                        }

                    }
                    AndHUD.Shared.Dismiss(this);

                }
                //SaveToBD();
            }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            var layout = FindViewById<LinearLayout>(Resource.Id.imagelinear);
            var newImage = new ImageView(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(250, 455)
            };
            newImage.SetPadding(5, 5, 5, 5);
            layout.AddView(newImage);
            newImage.Click += Image_Click;
            newImage.LongClick += Image_LongClick;
            tmpView = newImage;
            Bitmap bitmap = (Bitmap)data.Extras.Get("data");
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 30, ms);
                bitmap = BitmapFactory.DecodeByteArray(ms.ToArray(), 0, ms.ToArray().Length);
            }
            tmpView.LayoutChange += NewImage_LayoutChange;
            tmpView.SetImageBitmap(bitmap);
            tmpView.LayoutChange -= NewImage_LayoutChange;
        }

        private async void Camera_Click(object sender, EventArgs e)
        {
            dialog.Dismiss();
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            AndHUD.Shared.Show(this, "Please, wait", 100, MaskType.Black, TimeSpan.FromSeconds(5));
            StartActivityForResult(intent, 0);
            AndHUD.Shared.Dismiss(this);
            //SaveToBD();
        }

        private void Bigimage_Click(object sender, EventArgs e)
        {
            bigimage.Visibility = ViewStates.Invisible;
        }

        private void Image_Click(object sender, EventArgs e)
        {
            bigimage.SetImageDrawable(((ImageView)sender).Drawable);
            //bigimage.Background = image.Background;
            bigimage.Visibility = ViewStates.Visible;

        }

        private void Delete_Click(object sender, EventArgs e)
        {
            var layout = FindViewById<LinearLayout>(Resource.Id.imagelinear);
            layout.RemoveView(tmpView);
            delete.Visibility = ViewStates.Invisible;
            remake.Visibility = ViewStates.Invisible;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera }, CAMERA_PERMISSION_CODE);
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, STORAGE_PERMISSION_CODE);
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, STORAGE2_PERMISSION_CODE);
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Choose");
            alert.SetMessage("Choose what to use");
            alert.SetPositiveButton("Camera", Camera_Click);
            alert.SetNegativeButton("Gallery", Gallery_Click);
            dialog = alert.Create();
            dialog.Show();
            //newImage.LayoutChange += NewImage_LayoutChange;
            //newImage.ViewAttachedToWindow += NewImage_ViewAttachedToWindow;
        }

        private void NewImage_ViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e)
        {
            SaveToBD();
        }

        private void NewImage_LayoutChange(object sender, View.LayoutChangeEventArgs e)
        {
            SaveToBD();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }


    }
}
