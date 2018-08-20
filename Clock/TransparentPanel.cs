using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualComponents
{
    // https://stackoverflow.com/questions/4463363/how-can-i-set-the-opacity-or-transparency-of-a-panel-in-winforms

    public class TransparentPanel : Panel
    {
        #region Private Fields

        private bool m_MouseIsDown = false;
        private bool m_MouseClick = false;
        private Point m_MouseDownCoordinates;

        #endregion


        #region Public Events

        public event EventHandler<MovingPanelEventArgs> Move;
        public event EventHandler<MouseEventArgs> MouseClick;

        #endregion


        #region Overriden Protected Methods

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_MouseIsDown = true;

            if (e.Button != MouseButtons.Right)
            {
                m_MouseClick = true;
            }

            m_MouseDownCoordinates = e.Location;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_MouseIsDown = false;

            base.OnMouseUp(e);
        }

        //
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            m_MouseClick = false;

            if (m_MouseIsDown)
            {
                OnMoving(e);
            }
        }

        private void OnMoving(MouseEventArgs e)
        {
            Point LocationNew = new Point(this.Location.X + e.Location.X - m_MouseDownCoordinates.X,
                this.Location.Y + e.Location.Y - m_MouseDownCoordinates.Y);

            if (Move != null)
            {
                Move.Invoke(this, new MovingPanelEventArgs(LocationNew.X, LocationNew.Y));
            }
        }

        //
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (!m_MouseClick)
            {
                return;
            }

            OnClicking();
        }

        private void OnClicking()
        {
            if (MouseClick != null)
            {
                MouseClick.Invoke(this, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
            }
        }

        #endregion
    }
}
