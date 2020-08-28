using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace notepad
{
    class Texts
    {
        public int id;
        public string text;
        public string title;
        public List<Images> images=new List<Images>();

        public void AddImage(int id, byte[] image)
        {
            this.images.Add(new Images(id,image,this.id));
        }
    }

    class Images
    {
        public int id;
        public byte[] image;
        public int tId;
        public Images(int id,byte[]image,int tId)
        {
            this.id = id;
            this.image = image;
            this.tId = tId;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);
            return bitmap;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg,0,ms);
                image = ms.ToArray();
            }
        }
    }

    static class CVTR
    {
        public static byte[] strim(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }
    }
}