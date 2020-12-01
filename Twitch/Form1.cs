using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitch
{
    public partial class Form1 : Form
    {
        Form2 sizer = new Form2();
        public bool settingSize = false;
        public bool rectSet = false;
        public bool idle = true;
        public int ptsGain = 0;

        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        Rectangle myRect = new Rectangle();

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(rectSet)
            {
                int cursorX = Cursor.Position.X;
                int cursorY = Cursor.Position.Y;
                double total = 0;
                double match = 0;
                Bitmap bmp = new Bitmap(myRect.Width, myRect.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(myRect.X, myRect.Y, 0, 0, bmp.Size);
                pictureBox1.Image = bmp;
                Color color = ColorTranslator.FromHtml("#00e6cb");

                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        total++;
                        if (bmp.GetPixel(x, y) == color)
                        {
                            g.CopyFromScreen(myRect.X, myRect.Y, 0, 0, bmp.Size);
                            match++;
                            if(match/total > .60)
                            {
                                esgeddit(x, y, myRect.X, myRect.Y, 500);
                                if(idle)
                                {
                                    ptsGain += 50;
                                    Log("Info", "Clicked! +"+ptsGain+" total!");
                                    idle = false;
                                }
                            }
                        }
                    }
                }
                idle = true;
                SetCursorPos(cursorX, cursorY);
            }
            else
            {
                Log("Warn", "Please set position!");
                Log("Warn", "Disabled...");
                this.Text = "Twitch Points Bot - Disabled";
                timer1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            this.Text = "Twitch Points Bot - Enabled";
            Log("Info", "Enabled!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Text = "Twitch Points Bot - Disabled";
            Log("Info", "Disabled!");
        }

        private void esgeddit(int x, int y, int a, int b, int sleep)
        {
            SetCursorPos(a + x, b + y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, a + x, b + y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, a + x, b + y, 0, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(settingSize)
            {
                sizer.Hide();
                button3.Text = "Set Position";
                settingSize = false;
                rectSet = true;
                
                myRect = new Rectangle(sizer.Location, sizer.Size);
                Log("Info", "Position saved.");
            }
            else
            {
                sizer.Show();
                button3.Text = "Save Position";
                settingSize = true;
                Log("Info", "Setting position...");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Info", "Twitch bot initialized!");
        }
        public void Log(string type, string text)
        {
            listBox1.Items.Add("(" + type + ") " + Timestamp() + text);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            listBox1.ClearSelected();
        }
        public string Timestamp()
        {
            DateTime dt = DateTime.Now;
            string time = "[" + dt.ToString("hh:mm tt") + "] -- ";
            return time;
        }
    }
}
