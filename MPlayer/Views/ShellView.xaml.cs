using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MPlayer.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private HwndSource _hwndSource;
        private const int WM_SYSCOMMAND = 0x112;

        public ShellView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            SourceInitialized += HandleSourceInitialized;

            base.OnInitialized(e);
        }

        private void HandleSourceInitialized(object sender, EventArgs e)
        {
            // Don't he hatin' on my Win32... he's just misunderstood
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);

            HwndSource.FromHwnd(_hwndSource.Handle).AddHook(new HwndSourceHook(WndProc));

            int style = GetWindowLong(_hwndSource.Handle, GWL_STYLE);
            SetWindowLong(_hwndSource.Handle, GWL_STYLE, style & ~WS_SYSMENU);
        }

        //"AddHook" needs a deligate, so that's why this is here
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        public void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        public void Maximize(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                this.Margin = new Thickness(0);

                ResizerB.Cursor = Cursors.SizeNS;
                ResizerBr.Cursor = Cursors.SizeNWSE;
                ResizerR.Cursor = Cursors.SizeWE;

                WindowState = System.Windows.WindowState.Normal;
                ResizeMode = System.Windows.ResizeMode.CanResize;
            }
            else
            {
                WindowState = WindowState.Maximized;
                this.Margin = new Thickness(0, 4, 4, 0);

                ResizerB.Cursor = Cursors.Arrow;
                ResizerBr.Cursor = Cursors.Arrow;
                ResizerR.Cursor = Cursors.Arrow;

                WindowState = System.Windows.WindowState.Maximized;
                //ResizeMode = System.Windows.ResizeMode.NoResize;
            }
        }

        public void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == System.Windows.WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    this.Margin = new Thickness(0, 0, 0, 0);
                }
                else if (WindowState == System.Windows.WindowState.Normal)
                {
                    WindowState = System.Windows.WindowState.Maximized;
                    this.Margin = new Thickness(0, 4, 4, 0);
                }
            }
            else
            {
                DragMove();
            }
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        public void ResizeRight(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == System.Windows.ResizeMode.CanResize)
            {
                Resize(ResizeDirection.Right);
                e.Handled = true;
            }
        }

        public void ResizeBottom(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == System.Windows.ResizeMode.CanResize)
            {
                Resize(ResizeDirection.Bottom);
                e.Handled = true;
            }
        }

        public void ResizeBottomRight(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == System.Windows.ResizeMode.CanResize)
            {
                Resize(ResizeDirection.BottomRight);
                e.Handled = true;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hwndSource != null)
            {
                if (this.Height == System.Windows.Forms.Screen.FromHandle(_hwndSource.Handle).WorkingArea.Height ||
                    this.Width == System.Windows.Forms.Screen.FromHandle(_hwndSource.Handle).WorkingArea.Width)
                {
                    this.Margin = new Thickness(0, 4, 4, 0);
                }
                else if (this.Margin.Top == 0 && WindowState == System.Windows.WindowState.Normal)
                {
                    this.Margin = new Thickness(0);
                }
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.Margin = new Thickness(0, 4, 4, 0);
            }
            base.OnStateChanged(e);
        }

        #region WIN32 IMPORTS
        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public Int32 m_x;
            public Int32 m_y;

            public POINT(Int32 x, Int32 y)
            {
                m_x = x;
                m_y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        public enum MonitorFromWindowFlags
        {
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            public RECT rcMonitor;

            public RECT rcWork;

            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int Left;

            public int Top;

            public int Right;

            public int Bottom;

            public static readonly RECT Empty;

            public int Width
            {
                get
                {
                    return Math.Abs(this.Right - this.Left);
                } // Abs needed for BIDI OS
            }

            public int Height
            {
                get
                {
                    return this.Bottom - this.Top;
                }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                this.Left = rcSrc.Left;
                this.Top = rcSrc.Top;
                this.Right = rcSrc.Right;
                this.Bottom = rcSrc.Bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return this.Left >= this.Right || this.Top >= this.Bottom;
                }
            }

            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + this.Left + " / top : " + this.Top + " / right : " + this.Right + " / bottom : " +
                       this.Bottom + " }";
            }

            public override bool Equals(object obj)
            {
                if (!(obj is RECT))
                {
                    return false;
                }
                return (this == (RECT)obj);
            }

            public override int GetHashCode()
            {
                return this.Left.GetHashCode() + this.Top.GetHashCode() + this.Right.GetHashCode() +
                       this.Bottom.GetHashCode();
            }

            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.Left == rect2.Left && rect1.Top == rect2.Top && rect1.Right == rect2.Right &&
                        rect1.Bottom == rect2.Bottom);
            }

            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MARGINS
        {
            public Int32 m_leftWidth;
            public Int32 m_rightWidth;
            public Int32 m_topHeight;
            public Int32 m_bottomHeight;
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x00080000;

        [DllImport("user32.dll", SetLastError = true)]
        private extern static int SetWindowLong(IntPtr hwnd, int index, int value);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        public void Resize(ResizeDirection dir)
        {
            SendMessage(_hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + dir), IntPtr.Zero);
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        internal static extern Int32 DwmSetWindowAttribute(IntPtr hwnd, Int32 attr, ref Int32 attrValue, Int32 attrSize);

        [DllImport("dwmapi.dll")]
        internal static extern Int32 DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            IntPtr monitor = MonitorFromWindow(hwnd, (int)MonitorFromWindowFlags.MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.m_x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.m_y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.m_x = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.ptMaxSize.m_y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                mmi.ptMinTrackSize.m_x = 640;
                mmi.ptMinTrackSize.m_y = 480;
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        #endregion
    }
}
