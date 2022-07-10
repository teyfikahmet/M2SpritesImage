using System;
using System.Collections.Generic;
using System.Text;

namespace M2SpritesImage
{
    class SubMaker
    {
        string result;
        public SubMaker(SubData data)
        {
            result = "title subImage\nversion 1.0\n";
            result += "image \"" + data.DDSName + "\"\n";
            result += "left " + data.Left + "\n";
            result += "top " + data.Top + "\n";
            result += "right " + data.Right + "\n";
            result += "bottom " + data.Bottom + "\n";
        }
        public string Get()
        {
            return result;
        }
    }
}
