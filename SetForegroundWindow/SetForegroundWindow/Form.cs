using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SetForegroundWindow
{
    public partial class Form : System.Windows.Forms.Form
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private extern static IntPtr BringWindowToTop(int hWnd);

        /// <summary>
        /// 设置窗口前置
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="hWndInsertAfter">值为-1，将窗口置于所有非顶层窗口之上。即使窗口未被激活窗口也将保持顶级位置。</param>
        /// <param name="X">以客户坐标指定窗口新位置的左边界。</param>
        /// <param name="Y">以客户坐标指定窗口新位置的顶边界。</param>
        /// <param name="cx">以像素指定窗口的新的宽度。</param>
        /// <param name="cy">以像素指定窗口的新的高度。</param>
        /// <param name="uFlags">窗口尺寸和定位的标志</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public Form()
        {
            InitializeComponent();
            foreach (var process in Process.GetProcesses())
            {
                if(process.MainWindowTitle.Length != 0)
                {
                    textBox2.AppendText(process.MainWindowTitle + Environment.NewLine);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr parenthWnd = new IntPtr(0);
            parenthWnd = FindWindow(null, textBox1.Text);
            //判断这个窗体是否有效
            if (parenthWnd != IntPtr.Zero && IsWindowVisible(parenthWnd))
            {
                //SetWindowPos(parenthWnd, -1, 0, 0, 0, 0, 3);
                //SetForegroundWindow((int)parenthWnd);
                BringWindowToTop((int)parenthWnd);
            }
            else
            {
                MessageBox.Show("没有找到窗口");
            }

        }
    }
}
