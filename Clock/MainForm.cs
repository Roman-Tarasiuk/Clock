using System;
using System.Drawing;
using System.Windows.Forms;

using WindowsManipulations;
using VisualComponents;

namespace Clock
{
    public partial class MainForm : Form
    {
        #region Private Members

        private bool m_Minimize = false;
        private DateTime m_TheDate = DateTime.Now;

        private Size m_WindowSize = new Size(78, 38);

        private TransparentPanel panel1;

        #endregion

        const int WS_MINIMIZEBOX = 0x20000;
        const int WS_SYSMENU = 0x80000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
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

            this.panel1.Move += Panel1_Moving;
            this.panel1.MouseClick += Panel1_MouseClick;

            this.panel1.ContextMenuStrip = contextMenuStrip1;

            this.Controls.Add(this.panel1);

            this.panel1.BringToFront();
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            CalendarForm calendar = new CalendarForm();
            calendar.SetLocationAndSize(this);

            calendar.ShowDialog();

            User32Helper.ShowWindow(this.Handle, User32Helper.SW_RESTORE);
        }

        private void Panel1_Moving(object sender, MovingPanelEventArgs e)
        {
            Location = PointToScreen(new Point(e.X, e.Y));
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

        private void currentDateAndTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NowToClipblard();
        }

        private static void NowToClipblard()
        {
            var now = DateTime.Now;
            var strDateTime = now.ToLocalTime();

            Clipboard.SetText(strDateTime.ToString("dd.MM.yyyy HH:mm:ss,fff"));
        }
    }
}
