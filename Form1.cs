using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Management;

namespace OS_lr1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")]
        static extern int GetSysColor(int nIndex);
        [DllImport("kernel32")]
        static extern int SetSysColor(int nIndex);
        [DllImport("kernel32")]
        public static extern int GetWindowsDirectory(StringBuilder buf, int nMaxCount);
        [DllImport("kernel32")]
        public static extern int GetSystemDirectory(StringBuilder buf, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        static extern int SystemParametersInfo(int actionNumber, ref int uiParam, ref int lpvParam, int funcWinInit);
        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT rc);
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder buf, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hwnd, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int nMaxCount);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        ManagementObjectSearcher paramSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

        string desktopName = Environment.MachineName;
        string userName = SystemInformation.UserName;
        string versionOS = Convert.ToString(Environment.OSVersion.Version);
        DateTime dateTime = DateTime.Now;
        string displayWidth = GetSystemMetrics(0).ToString();
        string displayLength = GetSystemMetrics(1).ToString();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fillingOfInfobox();
        }

        private void fillingOfInfobox()
        {
            var newLineTag = Environment.NewLine;
            StringBuilder systemDirectory = new StringBuilder(50);
            GetSystemDirectory(systemDirectory, systemDirectory.Capacity);

            const int COLOR_3DFACE = 5;
            const int COLOR_CAPTIONTEXT = 23;
            const int COLOR_BACKGROUND = 1;

            textBox1.Text = "desktopName = " + desktopName + newLineTag +
            "userName = " + userName + newLineTag +
            "versionOS = " + versionOS + newLineTag +
            "dateTime = " + dateTime + newLineTag +
            "displayWidth (First Metric) : " + displayWidth + newLineTag +
            "displayLength (Second Metric) : " + displayLength + newLineTag +
            "System directory : " + systemDirectory.ToString() + "\r\n\r\n" +
            "System colors : " + newLineTag +
            " Value of COLOR_3DFACE = " + GetSysColor(COLOR_3DFACE) + newLineTag +
            " Value of COLOR_CAPTIONTEXT = " + GetSysColor(COLOR_CAPTIONTEXT) + newLineTag +
            " Value of BACKGROUND = " + GetSysColor(COLOR_BACKGROUND) + newLineTag;

            foreach (ManagementObject queryObj in paramSearcher.Get())
            {
                textBox2.Text =
                    string.Format("AdapterRAM: {0}", Convert.ToDouble(queryObj["AdapterRAM"])/1024/1024) + " MB" + newLineTag +
                    string.Format("Caption: {0}", queryObj["Caption"]) + newLineTag +
                    string.Format("Description: {0}", queryObj["Description"]) + newLineTag +
                    string.Format("VideoProcessor: {0}", queryObj["VideoProcessor"]) + newLineTag;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int w, h;
            RECT rc = new RECT();
            GetWindowRect(this.Handle, ref rc);
            w = rc.right - rc.left;
            h = rc.bottom - rc.top;
            MessageBox(IntPtr.Zero, "Ширина формы Form1: " + w + "\n\rВысота формы Form1: " + h, "This is MessageBox from user32.dll", 0);

            var emptyInfo1 = GetWindowText(IntPtr.Zero, new StringBuilder(), 25);
            var emptyInfo2 = GetClassName(IntPtr.Zero, new StringBuilder(), 25);
        }
    }
}
