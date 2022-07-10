using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace M2SpritesImage
{
    class SubReader
    {
        private SubData result = new SubData();
        public SubReader(string fileText)
        {
            Match m;
            // dds name
            string fileNamePattern = @"image.(.*).(.dds|tga)";
            RegexOptions options = RegexOptions.IgnoreCase;
            m = Regex.Match(fileText, fileNamePattern, options);
            result.DDSName = m.Value.Replace("image ", "").Replace("\"", "");

            string leftPattern = @"left.([0-9]+)";
            m = Regex.Match(fileText, leftPattern, options);
            result.Left = Int32.Parse(m.Value.Replace("left ", ""));

            string topPattern = @"top.([0-9]+)";
            m = Regex.Match(fileText, topPattern, options);
            result.Top = Int32.Parse(m.Value.Replace("top ", ""));

            string rightPattern = @"right.([0-9]+)";
            m = Regex.Match(fileText, rightPattern, options);
            result.Right = Int32.Parse(m.Value.Replace("right ", ""));

            string bottomPattern = @"bottom.([0-9]+)";
            m = Regex.Match(fileText, bottomPattern, options);
            result.Bottom = Int32.Parse(m.Value.Replace("bottom ", ""));
        }

        public SubData GetResult()
        {
            return result;
        }

    }

    class SubData
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public string DDSName { get; set; }
        public string FileName { get; set; }

    }
}
