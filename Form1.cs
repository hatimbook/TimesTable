using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace TimesTable
{
    public partial class Form1 : Form
    {
        #region GLOBALS
        
        private const int SLOWSPEED = 20;
        private const int MEDSPEED = 10;
        private const int HIGHSPEED = 5;
        int numpoints = 200;
        decimal factor = 2.0m;
        bool autorunflag = false;
        bool showcontrols = true;
        bool fullscreen = false;
        int autorunspeed = MEDSPEED;
        bool endthread = false;
        
        #endregion

        public Form1()
        {
            InitializeComponent();
            trackBar1.Value = numpoints;
            trackBar2.Value = 1;
            Factor.Value = factor;
            //Width = 1920;
            //Height = 1080;
            this.Paint += Form1_Paint;
            this.FormClosing += Form1_FormClosing;
            DoubleBuffered = true;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            endthread = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            float RADIUS = (Height - 100) / 2;

            //define paints and brushes
            Pen mypen = new Pen(Color.White);
            Brush mybrush = new SolidBrush(Color.Black);
            Color mycolor1, mycolor2;
            if(Colorbox.Checked)
            {
                mycolor1 = Color.FromArgb((byte)(factor * 2.5m), (byte)(250 - factor * 2.5m), 125);
                mycolor2 = Color.FromArgb((byte)(250 - factor * 2.5m), (byte)(factor * 2.5m), 125);
            }
            else
            {
                mycolor1 = Color.GhostWhite;
                mycolor2 = Color.GhostWhite;
            }
            Pen linepen = new Pen(mycolor1);
            Pen linepen1 = new Pen(mycolor2);

            // move origin to centre of canvas
            g.TranslateTransform(Width / 2, Height / 2);
            PointF origin = new PointF(0, 0);

            // Draw the lines
            decimal delta = (decimal)((2 * Math.PI / numpoints));
            for (int i = 0; i < numpoints; i++)
            {
                double a = (double)((i % numpoints) * delta);
                double b = (double)(((i * factor) % numpoints) * delta);
                PointF pointa, pointb;
                pointa = CirclePoint(RADIUS, a, origin);
                pointb = CirclePoint(RADIUS, b, origin);
                if (i % 2 == 0)
                {
                    g.DrawLine(linepen1, pointa, pointb);
                }
                else
                {
                    g.DrawLine(linepen, pointa, pointb);
                }
            }

            // Draw the big circle
            DrawCircle(RADIUS, origin, mypen, g);

            if (!autorunflag)
            {
                button1.Text = "Play";
                button1.BackColor = Color.ForestGreen;
                progressBar1.Value = 0;
                groupBox2.Visible = true;
            }
            else
            {
                button1.Text = "Stop";
                button1.BackColor = Color.DarkRed;
                progressBar1.Value = (int)factor;
            }
            
            Factor.Value = factor;
        }

        #region PRIVATE_FUNCTIONS
        // Draws a circle with a given pen of a given radius at the given origin point
        private void DrawCircle(float radius, PointF origin, Pen p, Graphics g)
        {
            RectangleF rect = new RectangleF(origin.X - radius, origin.Y - radius, radius * 2, radius * 2);
            g.DrawEllipse(p, rect);
        }
        
        // determines point on a circle of given radius of given origin with a given angle
        private PointF CirclePoint(float radius, double angleinradians, PointF origin)
        {
            float x = (float)(radius * Math.Cos(angleinradians) + origin.X);
            float y = (float)(radius * Math.Sin(angleinradians) + origin.Y);
            return new PointF(x, y);
        }
        #endregion

        #region USERINTERFACE
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TrackBar val = (TrackBar)sender;
            numpoints = (int)val.Value;
            this.Invalidate();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            TrackBar speedbar = (TrackBar)sender;
            switch (speedbar.Value)
            {
               
                case 0:
                    autorunspeed = HIGHSPEED;
                    break;
                default:
                case 1:
                    autorunspeed = MEDSPEED;
                    break;

                case 2:
                    autorunspeed = SLOWSPEED;
                    break;
            };
                    
        }

        private void Factor_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown val = (NumericUpDown)sender;
            factor = (decimal)val.Value;
            this.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button thisbtn = (Button)sender;
            factor = 1.0m;
            Thread run = new Thread(autorun);
            run.Start();
            autorunflag = !autorunflag;
            this.Invalidate();            
        }

        #endregion

        private void autorun()
        {
            while (true)
            {
                if (autorunflag)
                {
                    while (decimal.Compare(factor, 100.00m) < 0)
                    {
                        factor = decimal.Add(factor, 0.001m);
                        this.Invalidate();
                        Thread.Sleep(autorunspeed);
                        if(endthread | (!autorunflag))
                        {
                            this.Invalidate();
                            break;
                        }
                    }
                }
                autorunflag = false;
                break;
            }
        }

        private void hideAllControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            if(showcontrols)
            {
                groupBox1.Visible = false;
                groupBox2.Visible = false;
                menu.Text = "Show Controls";
            }
            else
            {
                groupBox1.Visible = true;
                groupBox2.Visible = true;
                menu.Text = "Hide All Controls";
            }
            showcontrols = !showcontrols; 
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            if (!fullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                menu.Text = "Exit Full Screen";
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                menu.Text = "Full Screen";
            }
            fullscreen = !fullscreen;
        }
    }
}
