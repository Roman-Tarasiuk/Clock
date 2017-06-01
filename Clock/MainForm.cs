using System;
using System.Drawing;
using System.Windows.Forms;

using WindowsManipulations;

namespace Clock
{
    public partial class MainForm : Form
    {
        #region Private Members

        private bool m_MouseIsPressed = false;
        private bool m_MouseClick = false;
        private bool m_Minimize = false;
        private Point m_PressedCoordinates = new Point();
        private DateTime m_TheDate = DateTime.Now;
        private int m_Ticks = 0;

        private Size m_WindowSize = new Size(78, 38);
        private Size m_CalendarWindowSize = new Size(186, 207);
        private int m_OffsetX = 8; // I don't know, why without this value, calendar window displays to left
        private int m_OffsetY = 2; // Pixels between clock and calendars windows

        private System.Windows.Forms.Panel panel1;

        #endregion

        const int WS_MINIMIZEBOX = 0x20000;
        const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX | WS_SYSMENU;
                return cp;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            InitializePanel();
            ShowDateTime(true);
        }

        private void InitializePanel()
        {
            this.panel1 = new TransparentPanel();

            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = m_WindowSize;
            this.panel1.TabIndex = 2;

            this.panel1.MouseDown += Panel1_MouseDown;
            this.panel1.MouseUp += Panel1_MouseUp;
            this.panel1.MouseMove += Panel1_MouseMove;
            this.panel1.MouseClick += Panel1_MouseClick;

            this.panel1.ContextMenuStrip = contextMenuStrip1;

            this.Controls.Add(this.panel1);

            this.panel1.BringToFront();
            this.lblClock.SendToBack();
            this.lblDate.SendToBack();
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!m_MouseClick)
            {
                return;
            }

            CalendarForm calendar = new CalendarForm();

            int X = 0,
                Y = 0;

            int tmpX = this.Location.X + m_WindowSize.Width - m_CalendarWindowSize.Width + m_OffsetX;
            if (tmpX >= 0)
            {
                X = tmpX;
            }
            else
            {
                X = this.Location.X - m_OffsetX;
            }

            int tmpY = this.Location.Y + m_WindowSize.Height + m_OffsetY;
            if (tmpY + m_CalendarWindowSize.Height + m_OffsetY <= Screen.PrimaryScreen.WorkingArea.Height)
            {
                Y = tmpY;
            }
            else
            {
                Y = this.Location.Y - m_CalendarWindowSize.Height + m_OffsetY;
            }

            calendar.StartPosition = FormStartPosition.Manual;
            calendar.Location = new Point(X, Y);

            calendar.ShowDialog();

            User32Helper.ShowWindow(this.Handle, User32Helper.SW_RESTORE);
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            m_MouseIsPressed = true;
            if (e.Button != MouseButtons.Right)
            {
                m_MouseClick = true;
            }
            m_PressedCoordinates = e.Location;
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            m_MouseIsPressed = false;
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.m_MouseIsPressed)
            {
                this.Location = new Point(this.Location.X + e.Location.X - m_PressedCoordinates.X,
                                            this.Location.Y + e.Location.Y - m_PressedCoordinates.Y);
                m_MouseClick = false;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.Size = m_WindowSize;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowDateTime(false);
        }

        private void ShowDateTime(bool start)
        {
            DateTime now = DateTime.Now;

            lblClock.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", now.Hour, now.Minute, now.Second);

            if (start || DateChanged(now))
            {
                lblDate.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", now.Day, now.Month, now.Year);
            }
        }

        private bool DateChanged(DateTime now)
        {
            if (m_TheDate.Day != now.Day || m_TheDate.Month != now.Month || m_TheDate.Year != now.Year)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (!m_Minimize)
                {
                    User32Helper.ShowWindow(this.Handle, User32Helper.SW_RESTORE);
                }
                else
                {
                    m_Minimize = false;
                }
            }
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Minimize = true;
            User32Helper.ShowWindow(this.Handle, User32Helper.SW_MINIMIZE);
        }
    }
}
