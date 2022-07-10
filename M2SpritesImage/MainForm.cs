using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using M2UiElementMaganerCore;

namespace M2SpritesImage
{
    public partial class MainForm : Form
    {
        private OpenFileDialog Opendialog;
        private SaveFileDialog SaveFileDialog;
        private string APP_TITLE = "M2 Ui Element Manager V1";
        private string lastDirectory = null;
        private string lastFileName = null;
        private string fullPath = null;
        private ColorConverter converter = new ColorConverter();
        private Color BackGroundColor;
        private Bitmap lastImage;
        private TextBox MainPathTextBox;
        private string MainDir = null;
        private ToolTip ToolTip;
        private SubData LastSubData = null;
        private bool isImageLoaded = false;
        public MainForm()
        {
            InitializeComponent();
            Opendialog = new OpenFileDialog();
            SaveFileDialog = new SaveFileDialog();
            Text = APP_TITLE;
            Resize += Form1_Resize;
            BackGroundColor = (Color)converter.ConvertFromString("#222222");
            

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
            IniReader config = new IniReader("config.ini");
            if (config.KeyExists("DefaultMainPath", "Config"))
                SetMainPath(config.Read("DefaultMainPath", "Config"));

            //SetMainPath("D:\\WORK\\FILES\\MakePack\\etc\\ui");
            //LoadHardSubFile("D:\\WORK\\FILES\\MakePack\\etc\\ui\\public\\slotactiveeffect\\slot3\\05.sub");
        }
        private void LoadImage(string fileName, SubData result)
        {
            try
            {
                Bitmap bitmap = Core.LoadBitmap(fileName);
                ReLocateImageBox();
                Size btSize = DrawBurshes(bitmap, result);
                SetTitle(String.Format("{0} - {1} ({2}x{3})", lastFileName, APP_TITLE, btSize.Width, btSize.Height));
                SetInfos(result, btSize.Width, btSize.Height);
                SaveFileButton.Enabled = true;
                isImageLoaded = true;
                LastSubData = result;
                LastSubData.FileName = fileName;
                reloadTimer.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "ERROR");
            }
        }

        private Size DrawBurshes(Bitmap image, SubData result)
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

        private void SetFieldReadOnly(bool readOnly)
        {
            TopField.ReadOnly = readOnly;
            LeftField.ReadOnly = readOnly;
            BottomField.ReadOnly = readOnly;
            RightField.ReadOnly = readOnly;
        }

        private void SetInfos(SubData result, int width, int height)
        {
            fileNameField.Text = lastFileName;
            DDSNameField.Text = result.DDSName;
            TopField.Text = result.Top.ToString();
            BottomField.Text = result.Bottom.ToString();
            LeftField.Text = result.Left.ToString();
            RightField.Text = result.Right.ToString();
            SizeField.Text = String.Format("{0}x{1}", width, height);
            FullPathField.Text = fullPath;
            SetFieldReadOnly(false);
            /*TopField.TextChanged += Field_TextChanged;
            LeftField.TextChanged += Field_TextChanged;
            RightField.TextChanged += Field_TextChanged;
            BottomField.TextChanged += Field_TextChanged;*/
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
            SubData result = subReader.GetResult();
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
                lastDirectory = Path.GetDirectoryName(Opendialog.FileName);
                lastFileName = Path.GetFileName(Opendialog.FileName);
                fullPath = Opendialog.FileName;
                Stream stream = Opendialog.OpenFile();
                StreamReader reader = new StreamReader(stream);
                LoadSubFile(reader.ReadToEnd());
            }
        }

        private void SaveButtonClick()
        {
            if (lastDirectory != null)
                SaveFileDialog.InitialDirectory = lastDirectory;
            SaveFileDialog.Filter = "sub dosyası|*.sub|png dosyası|*.png|dds dosyası|*.dds";
            SaveFileDialog.FileName = lastFileName;
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if(SaveFileDialog.FilterIndex == 1)
                {
                    if (LastSubData == null)
                        return;

                    lastDirectory = Path.GetDirectoryName(SaveFileDialog.FileName);
                    fullPath = SaveFileDialog.FileName;
                    FullPathField.Text = fullPath;
                    string content = new SubMaker(LastSubData).Get();

                    File.WriteAllText(SaveFileDialog.FileName, content);
                }
                else
                {
                    string fullfilename = String.Format("{0}", SaveFileDialog.FileName);
                    if (SaveFileDialog.FilterIndex == 1)
                        pictureBox1.Image.Save(fullfilename);
                    else
                        Core.SaveBitmap(fullfilename, (Bitmap)pictureBox1.Image);
                }
            }
        }

        private void NewButtonClick()
        {
            NewForm newForm = new NewForm();
            newForm.ShowDialog();
            NewFileData data = newForm.GetResult();
            if (!data.FileName.EndsWith(".sub"))
                data.FileName = data.FileName + ".sub";

            if (string.IsNullOrEmpty(data.FileName) || string.IsNullOrEmpty(data.PicName))
                return;

            string result = new SubMaker(
                new SubData()
                {
                    FileName = data.FileName,
                    DDSName = data.PicName,
                    Top = 0,
                    Right = 0,
                    Left = 0,
                    Bottom = 0,
                }
            ).Get();
            lastFileName = data.FileName;
            LoadSubFile(result);
        }

        private void SetMainPath(string path)
        {
            if(!Directory.Exists(path))
            {
                //MessageBox.Show("Dizin Bulunamadı... -> " + path, "ERROR");
                //OpenFolderDialog();
                return;
            }

            MainPathTextBox.Text = path;
            MainDir = path;
            OpenFileButton.Enabled = true;
            newButton.Enabled = true;
        }

        private void OpenFolderDialog()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Ana dizini seç...";
            if (System.IO.Directory.Exists("D:\\WORK\\FILES\\fullbinary\\pack\\etc\\ymir work\\ui\\"))
                folderBrowser.SelectedPath = "D:\\WORK\\FILES\\fullbinary\\pack\\etc\\ymir work\\ui\\";
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

        private void newButton_Click(object sender, EventArgs e)
        {
            NewButtonClick();
        }

        private void Field_TextChanged(object sender, EventArgs e)
        {
           
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!isImageLoaded)
                return;
            TextBox snd = sender as TextBox;
            SubData data = LastSubData;
            ReloadButton.Enabled = false;
            SetFieldReadOnly(true);
            int number;

            if (int.TryParse(TopField.Text, out number))
                data.Top = number;

            if (int.TryParse(BottomField.Text, out number))
                data.Bottom = number;

            if (int.TryParse(LeftField.Text, out number))
                data.Left = number;

            if (int.TryParse(RightField.Text, out number))
                data.Right = number;

            isImageLoaded = false;
            LoadImage(data.FileName, data);
        }

        private void reloadTimer_Tick(object sender, EventArgs e)
        {
            ReloadButton.Enabled = true;
            SetFieldReadOnly(false);
        }
    }
}
