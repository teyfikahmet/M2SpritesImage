using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace M2SpritesImage
{
    class nButton : Button
    {
        private int _buttonRadius = 5;
        private string _borderColor = "#ffffff";
        private string _foreColor = "#fdfdfd";

        public int ButtonRadius { get { return _buttonRadius; } set { _buttonRadius = value; } }
        public string BorderColor { get { return _borderColor; } set { _borderColor = value; } }
        public string TextColor { get { return _foreColor; } set { _foreColor = value; } }
        private ColorConverter converter = new ColorConverter();
        public nButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.Paint += NButton_Paint;
            this.ForeColor = (Color)converter.ConvertFromString(_foreColor);
        }

        private void NButton_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(this.ClientRectangle.X + 1,
                                            this.ClientRectangle.Y + 1,
                                            this.ClientRectangle.Width - 2,
                                            this.ClientRectangle.Height - 2);
            this.Region = new Region(GetRoundedRect(rect, _buttonRadius));
            rect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
            e.Graphics.DrawPath(new Pen((Color)converter.ConvertFromString(_borderColor)), GetRoundedRect(rect, _buttonRadius));

        }
        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();
            if (radius == 0) {
                path.AddRectangle(bounds);
                return path;
            }
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
