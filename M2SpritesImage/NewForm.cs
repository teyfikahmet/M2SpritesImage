using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace M2SpritesImage
{
    public partial class NewForm : Form
    {
        private ColorConverter converter = new ColorConverter();
        NewFileData res = new NewFileData();
        public NewForm()
        {
            InitializeComponent();
            BackColor = (Color)converter.ConvertFromString("#222222");
            Text = "Yeni Oluştur";
        }

        public NewFileData GetResult()
        {
            res.FileName = fileNameBox.Text;
            res.PicName = pictureNameBox.Text;
            return res;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    public class NewFileData
    {
        public string FileName { get; set; }
        public string PicName { get; set; }
    }
}
