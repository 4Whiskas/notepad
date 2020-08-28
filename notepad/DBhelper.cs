using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Database.Sqlite;
using Android.Text.Style;

namespace notepad
{
    public class DBhelper : SQLiteOpenHelper
    {
        public static int DATABASE_VERSION = 1;
        public static string DATABASE_NAME = "notePadDB";
        public static string TABLE_TEXT = "texts";
        public static string TABLE_IMAGE = "images";

        public static string TKEY_ID = "_id";
        public static string TKEY_LABEL = "label";
        public static string TKEY_TEXT = "text";

        public static string ITKEY_ID = "_idT";
        public static string IKEY_ID = "_idI";
        public static string IKEY_IMAGE = "image";

        public DBhelper(Context context) : base(context, DATABASE_NAME, null, DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            
            db.ExecSQL("create table " + TABLE_TEXT + "(" + TKEY_ID + " integer primary key, " + TKEY_LABEL + " text," +TKEY_TEXT + " text" + ")");
            db.ExecSQL("create table " + TABLE_IMAGE + "(" + IKEY_ID + " integer primary key, " + ITKEY_ID + " integer, " + IKEY_IMAGE + " blob" + ")");
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            
        }
    }
}