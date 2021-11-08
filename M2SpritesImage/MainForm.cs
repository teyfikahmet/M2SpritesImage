using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevIL;
using System.IO;


namespace M2SpritesImage
{
    public partial class MainForm : Form
    {
        private OpenFileDialog Opendialog;
        private SaveFileDialog SaveFileDialog;
        private string APP_TITLE = "M2SpritesImage";
        private string lastDirectory = null;
        private string lastFileName = null;
        private string fullPath = null;
        private bool isInit = false;
        private ColorConverter converter = new ColorConverter();
        private Color BackGroundColor;
        private Bitmap lastImage;
        private TextBox MainPathTextBox;
        private string MainDir = null;
        private ToolTip ToolTip;
        private IniFile configs;
        public MainForm()
        {
            InitializeComponent();
            Opendialog = new OpenFileDialog();
            SaveFileDialog = new SaveFileDialog();
            Text = APP_TITLE;
            Resize += Form1_Resize;
            BackGroundColor = (Color)converter.ConvertFromString("#222222");
            configs = new IniFile("Conf.ini");
            

            MainPathTextBox = textBox1;

            ToolTip = new ToolTip();
        }

        private void MainHoverEvent(object sender, EventArgs e)
        {
            Control control = sender as Control;
            switch(control.Name)
            {
                case "ChangePathButton":
                    ToolTip.SetToolTip(control, "Ana Dizini Seç. (etc/ui)");
                    break;
                case "OpenFileButton":
                    ToolTip.SetToolTip(control, "Sub Dosyasını Aç.");
                    break;
                case "SaveFileButton":
                    ToolTip.SetToolTip(control, "Resmi Kaydet.");
                    break;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ReLocateImageBox();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            BackColor = BackGroundColor;
            //SetMainPath("D:\\WORK\\FILES\\MakePack\\etc\\ui");
            //LoadHardSubFile("D:\\WORK\\FILES\\MakePack\\etc\\ui\\public\\slotactiveeffect\\slot3\\05.sub");
        }
        private void LoadImage(string fileName, SubReaderResult result)
        {
            try
            {
                Bitmap image = DevIL.DevIL.LoadBitmap(fileName);
                
                ReLocateImageBox();
                Size btSize = DrawBurshes(image, result);
                SetTitle(String.Format("{0} - {1} ({2}x{3})", lastFileName, APP_TITLE, btSize.Width, btSize.Height));
                SetInfos(result, btSize.Width, btSize.Height);
                SaveFileButton.Enabled = true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "ERROR");
            }
        }

        private Size DrawBurshes(Bitmap image, SubReaderResult result)
        {
            int left = result.Left;
            int top = result.Top;
            int right = result.Right;
            int bottom = result.Bottom;
            int newWidth = (left - right) < 0 ? (left - right) * -1 : (left - right);
            int newHeight = (top - bottom) < 0 ? (top - bottom) * -1 : (top - bottom);

            Rectangle cropRect = new Rectangle(left,top, newWidth, newHeight);

            Bitmap newbt = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(newbt))
            {
                g.DrawImage(image, new Rectangle(0, 0, newbt.Width, newbt.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }
            
            pictureBox1.Size = newbt.Size;
            pictureBox1.Image = newbt;
            lastImage = newbt;

            /*lastImage.MakeTransparent(Color.White);
            System.IntPtr icH = lastImage.GetHicon();
            Icon ico = Icon.FromHandle(icH);
            Icon = ico;*/
            
            ReLocateImageBox();
            return new Size(newbt.Width, newbt.Height);
        }

        private void SetTitle(string TitleName)
        {
            Text = TitleName;
        }

        private void SetInfos(SubReaderResult result, int width, int height)
        {
            fileNameField.Text = lastFileName;
            DDSNameField.Text = result.DDSName;
            TopField.Text = result.Top.ToString();
            BottomField.Text = result.Bottom.ToString();
            LeftField.Text = result.Left.ToString();
            RightField.Text = result.Right.ToString();
            SizeField.Text = String.Format("{0}x{1}", width, height);
            FullPathField.Text = fullPath;
        }

        private void ReLocateImageBox()
        {
            int new_Left = (panel1.Width / 2) - (pictureBox1.Width / 2);
            if (new_Left < 0)
                new_Left = 0;
            int new_top = (panel1.Height / 2) - (pictureBox1.Height / 2);
            if (new_top < 0)
                new_top = 0;
            pictureBox1.Location = new Point(new_Left, new_top);
        }

        private void LoadHardSubFile(string filename)
        {
            if(!File.Exists(filename))
            {
                MessageBox.Show("Dosya bulunamadı\n" + filename, "HATA");
                return;
            }
            string innerSub = File.ReadAllText(filename);
            if (innerSub != null)
            {
                fullPath = filename;
                lastFileName = Path.GetFileName(filename);
                lastDirectory = Path.GetDirectoryName(filename);
                LoadSubFile(innerSub);
            }

        }

        private void LoadSubFile(string subInner)
        {
            SubReader subReader = new SubReader(subInner);
            SubReaderResult result = subReader.GetResult();
            Console.Write(result);
            string file_Name = String.Format("{0}\\{1}", MainDir, result.DDSName);
            if(!File.Exists(file_Name))
            {
                MessageBox.Show("DDS Dosyası Bulunamadı \n" + file_Name, "ERROR");
                return;
            }
            LoadImage(file_Name, result);
        }

        private void OpenButtonClick()
        {
            /*Opendialog.Filter = "DirectDraw Surface Image (*.dds)|*.dds";
            if(Opendialog.ShowDialog() == DialogResult.OK)
            {
                lastDirectory = System.IO.Path.GetDirectoryName(Opendialog.FileName);
                lastFileName = System.IO.Path.GetFileName(Opendialog.FileName);
                LoadImage(Opendialog.FileName);
            }*/

            Opendialog.Filter = "SubImage (*.sub)|*.sub";
            if(Opendialog.ShowDialog() == DialogResult.OK)
            {
                lastDirectory = System.IO.Path.GetDirectoryName(Opendialog.FileName);
                lastFileName = System.IO.Path.GetFileName(Opendialog.FileName);
                fullPath = Opendialog.FileName;
                Stream stream = Opendialog.OpenFile();
                StreamReader reader = new StreamReader(stream);
                LoadSubFile(reader.ReadToEnd());
            }
        }

        private void SaveButtonClick()
        {
            if (lastDirectory == null)
                return;
            SaveFileDialog.InitialDirectory = lastDirectory;
            SaveFileDialog.Filter = "Png Dosyası|*.png|Dds Dosyası|*.dds";
            SaveFileDialog.FileName = lastFileName.Replace(".sub", ".png");
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fullfilename = String.Format("{0}", SaveFileDialog.FileName);
                if(SaveFileDialog.FilterIndex == 1)
                    pictureBox1.Image.Save(fullfilename);
                else
                    DevIL.DevIL.SaveBitmap(fullfilename, (Bitmap)pictureBox1.Image);
            }
        }

        private void SetMainPath(string path)
        {
            if(!Directory.Exists(path))
            {
                MessageBox.Show("Dizin Bulunamadı...", "ERROR");
                OpenFolderDialog();
                return;
            }
            MainPathTextBox.Text = path;
            MainDir = path;
            OpenFileButton.Enabled = true;
        }

        private void OpenFolderDialog()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Ana dizini seç...";
            if (System.IO.Directory.Exists("D:\\WORK\\FILES\\MakePack\\etc\\ui"))
                folderBrowser.SelectedPath = "D:\\WORK\\FILES\\MakePack\\etc\\ui";
            folderBrowser.ShowNewFolderButton = false;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                SetMainPath(folderBrowser.SelectedPath);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            OpenFolderDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenButtonClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveButtonClick();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.mmotutkunlari.com/uye/ahmetteyfik.7300/");
        }
    }
}
