﻿using CruxPaint;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DrawingSketch
{
    public partial class Form1 : Form
    {
        string currentTool = "Pen";

        Point lastPoint = Point.Empty;
        bool isMouseDown = false;

        float width = 5;
        float savedWitdh = 5;

        public Color c = new Color();
        Color savedColor = Color.Black;

        public Point current = new Point();
        public Point old = new Point();
        public Pen p = new Pen(Color.Black, 5);
        public static Graphics g;

        Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();

            Cursor.Current = Cursors.Cross;

            c = Color.Black;
            p = new Pen(c, width);

            textBox1.ReadOnly = true;
            textBox1.BackColor = c;

            numericUpDown1.Value = 5;

            textBox3.ReadOnly = true;
            textBox3.Text = $"{currentTool}";

            AssignWidth.ReadOnly = true;
            AssignHeight.ReadOnly = true;
            AssignWidth.Text = $"{pictureBox1.Width}";
            AssignHeight.Text = $"{pictureBox1.Height}";

            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = e.Location;
            isMouseDown = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(isMouseDown == true)
            {
                if (pictureBox1.Image == null)
                {
                    Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    pictureBox1.Image = bmp;
                }

                if (lastPoint != null)
                {
                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    {
                        if (currentTool == "Pen")
                        {
                            c = savedColor;
                            textBox1.BackColor = c;
                        }

                        g.DrawLine(p, lastPoint, e.Location);

                        p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
                    }
                    pictureBox1.Invalidate();

                    lastPoint = e.Location;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            lastPoint = Point.Empty;
        }

        private void SizePlus_Click(object sender, EventArgs e)
        {
            width = width + 1;
            textBox2.Text = $"{width}";

            savedWitdh = width;

            if (currentTool == "Eraser") c = pictureBox1.BackColor;

            p = new Pen(c, width);
            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }

        private void SizeMinus_Click(object sender, EventArgs e)
        {
            if (currentTool == "Eraser") c = pictureBox1.BackColor;

            width = width - 1;
            p = new Pen(c, width);

            savedWitdh = width;

            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);

            if (width < 1) width = 1;

            textBox2.Text = $"{width}";
        }

        private void changecolor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();

            if (colorDialog1.Color == Color.White) colorDialog1.Color = Color.Black;

            textBox1.BackColor = colorDialog1.Color;

            c = colorDialog1.Color;
            p = new Pen(c, width);

            savedColor = c;

            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }

        private void eraser_Click(object sender, EventArgs e)
        {
            currentTool = "Eraser";

            changecolor.Enabled = false;

            Color color = textBox1.BackColor = DefaultBackColor;

            c = Form1.DefaultBackColor;
            p = new Pen(c, width);

            textBox3.Text = $"{currentTool}";

            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }

        private void pen_Click(object sender, EventArgs e)
        {
            if(savedColor == DefaultBackColor)
            {
                MessageBox.Show("Color was set to background color. \nThe color has been set back to default.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                savedColor = Color.Black;
            }

            changecolor.Enabled = true;

            currentTool = "Pen";

            textBox1.BackColor = savedColor;
            c = savedColor;
            p = new Pen(c, width);

            textBox3.Text = $"{currentTool}";

            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }

        private void reset_Click(object sender, EventArgs e)
        {
            width = 5;
            p = new Pen(c, width);

            textBox2.Text = $"{width}";

            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }

        private void clear_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show("Are you sure you wanna delete your masterpiece?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer == DialogResult.No) return;
            else
            {
                pictureBox1.Image = null;
                pictureBox1.BackColor = DefaultBackColor;
                Invalidate();
                p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string Width = width.ToString();

            string content = textBox2.Text;

            var contentNumbers = Regex.Matches(Width, @"[a-zA-Z]").Count;

            var letters = contentNumbers.ToString();

            string filtered = Regex.Replace(content, @"Size:\s*", "").Trim();

            if (!int.TryParse(filtered, out int value))
            {
                return;
            }

            Width = filtered;

            var output = float.Parse(Width);

            c = savedColor;

            width = output;
        }

        private void Help_Click(object sender, EventArgs e)
        {
            string response = "Hello, I see you are new to Crux Paint? Keep reading to get some information about the different features in Crux Paint!\n\nButtons:\n\n1. Size +: Increases the size by 1.\n2. Size -: Decreases the size by 1.\n3. Pen: Selects the pen tool.\n4. Eraser: Selects the eraser tool.\n5. Background Color: Changes the background to the targeted color. \n\n Fields: \n\n1. Size: Shows the current size, you can also change the size with this field.\n2. Current Tool: Displays the selected tool.\n3. Color: Displayes the selected color.\n4. Width: Displays the current drawboard width.\n5. Height: Displays the current drawboard height. \n\n Buttons 2:\n\n1. Reset Size: Resets the size to the default value (5)\n2. Clear: Clears all work on screen.\n3. Change Color: Opens up a dialog for changing colors with the Pen tool.\n4. Save Image: Saves you masterpiece to a .jpg file\n5. Open Image: Opens the selected image to screen.";

            MessageBox.Show(response, "Informaion", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            int picWidth = pictureBox1.Width;
            int picHeight = pictureBox1.Height;

            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "JPEG Image (.jpeg) | *.jpg";

            pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, picWidth, picHeight));

            if (sf.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            ImageFormat f = ImageFormat.Jpeg;
            var path = sf.FileName;

            bmp.Save(path, f);
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            AssignWidth.Text = $"{pictureBox1.Width}";
            AssignHeight.Text = $"{pictureBox1.Height}";

            Invalidate();
        }

        private void openfile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Open File";
                ofd.Filter = "JPEG Images (*.jpeg)|*.jpg";

                if(ofd.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                pictureBox1.Image = new Bitmap(ofd.FileName);
            }
        }

        private void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            targetColor = bmp.GetPixel(pt.X, pt.Y);
            if (targetColor.ToArgb().Equals(replacementColor.ToArgb()))
            {
                return;
            }

            Stack<Point> pixels = new Stack<Point>();

            pixels.Push(pt);
            while (pixels.Count != 0)
            {
                Point temp = pixels.Pop();
                int y1 = temp.Y;
                while (y1 >= 0 && bmp.GetPixel(temp.X, y1) == targetColor)
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < bmp.Height && bmp.GetPixel(temp.X, y1) == targetColor)
                {
                    bmp.SetPixel(temp.X, y1, replacementColor);

                    if (!spanLeft && temp.X > 0 && bmp.GetPixel(temp.X - 1, y1) == targetColor)
                    {
                        pixels.Push(new Point(temp.X - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.X - 1 == 0 && bmp.GetPixel(temp.X - 1, y1) != targetColor)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.X < bmp.Width - 1 && bmp.GetPixel(temp.X + 1, y1) == targetColor)
                    {
                        pixels.Push(new Point(temp.X + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.X < bmp.Width - 1 && bmp.GetPixel(temp.X + 1, y1) != targetColor)
                    {
                        spanRight = false;
                    }
                    y1++;
                }

            }
            pictureBox1.Refresh();

        }

        private void paintbucket_Click(object sender, EventArgs e)
        {
            pictureBox1.BackColor = savedColor;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value < 1) 
            {
                MessageBox.Show("You have to specify a number between 1 and 100.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                numericUpDown1.Value = 1;
                return;
            }

            width = (int)numericUpDown1.Value;

            textBox2.Text = $"{width}";
        }

        private void timec_Click(object sender, EventArgs e)
        {
            //Form2.ShowDialog();
        }

        private void paintbucket_MouseDown(object sender, MouseEventArgs e)
        {
            if((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;

                pictureBox1.BackColor = colorDialog1.Color;
            };
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (currentTool == "Eraser") return;

            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                savedColor = Color.Black;
                textBox1.BackColor = savedColor;

                p = new Pen(savedColor, width);
                p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);

                return;
            };

            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;

            savedColor = colorDialog1.Color;
            textBox1.BackColor = savedColor;

            p = new Pen(savedColor, width);
            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }
    }
}