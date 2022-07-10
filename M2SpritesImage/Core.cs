using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace M2UiElementMaganerCore
{
    public class Core
    {
        public static Bitmap LoadBitmap(string szFileName)
        {
            try
            {
                Bitmap bitmap = DevIL.DevIL.LoadBitmap(szFileName);
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool SaveBitmap(string szFileName, Bitmap bitmap)
        {
            try
            {
                DevIL.DevIL.SaveBitmap(szFileName, bitmap);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
