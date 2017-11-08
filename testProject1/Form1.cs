using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProject1
{
    public struct BorderPoint
    {
        public Point point;
        public int preDir;

        public BorderPoint(int x, int y, int pd)
        {
            point = new Point(x, y);
            preDir = pd;
        }
    }

    public partial class Form1 : Form
    {
        Graphics g;
        int sizePen;
        Bitmap bitmap;
        Bitmap myPicture;
        Point mouse;
        private static Image sampleImage;

        public Form1()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(this.MyMouseMove);
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            pictureBox1.Image = bitmap;

            sizePen = 5;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouse = e.Location;
        }

        private void Draw(MouseEventArgs e)
        {
            g.DrawLine(new Pen(colorDialog1.Color, sizePen), mouse, e.Location);
            mouse = e.Location;
            pictureBox1.Refresh();
        }

        private void MyMouseMove(object sender, MouseEventArgs e)
        {
            if (!radioButton3.Enabled || !radioButton3.Checked)
                return;
            if (e.Button == MouseButtons.Left)
                Draw(e);
        }

        private void FillPicture(Point e, int x, int y, Color backColor)
        {
            int x1 = e.X, x2 = e.X;
            int y1 = e.Y, y2 = e.Y;
            Color pixelColor = new Color();
            pixelColor = bitmap.GetPixel(x1, y1);

            if (pixelColor.ToArgb() != backColor.ToArgb())
                return;
            // если цвет пиксела равен цвету фона
            while (pixelColor.ToArgb() == backColor.ToArgb() && x1 > 0)
            {
                // цвет левого пикселя
                --x1;
                pixelColor = bitmap.GetPixel(x1, y1);
            }
            pixelColor = backColor;
            while (pixelColor.ToArgb() == backColor.ToArgb() && x2 < pictureBox1.Image.Width - 1)
            {
                // цвет правого пикселя
                ++x2;
                pixelColor = bitmap.GetPixel(x2, y2);
            }

            // рисуем линию
            int newX;
            newX = myPicture.Width - (mouse.X - x1) % myPicture.Width;

            if (y == -1)
                y = myPicture.Height - 1;
            ++x1;
            --x2;

            for (int i = x1; i < x2; ++i)
                bitmap.SetPixel(i, y1, myPicture.GetPixel((myPicture.Width + newX++) % myPicture.Width, (myPicture.Height + y) % myPicture.Height));

            for (int i = x1; i < x2; ++i)
            {
                if (y1 < pictureBox1.Image.Height - 1)
                    FillPicture(new Point(i, y1 + 1), mouse.X, y + 1, backColor);
                if (y1 > 0)
                    FillPicture(new Point(i, y2 - 1), mouse.X, y - 1, backColor);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked && radioButton1.Enabled)
            {
                if (sampleImage == null)
                {
                    MessageBox.Show("Не установлен шаблон.");
                    return;
                }

                FillPicture(new Point(e.X, e.Y), int.MaxValue, 0, bitmap.GetPixel(e.X, e.Y));
            }

            pictureBox1.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton3.Checked)
            {
                pictureBox1.MouseMove -= MyMouseMove;
            }
            else
            {
                pictureBox1.MouseMove += MyMouseMove;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                myPicture = new Bitmap(ofd.FileName);

            sampleImage = Image.FromFile(ofd.FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Refresh();
        }
    }
}