using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LR4
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        [DllImport("kernel32.dll")]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);
        [DllImport("kernel32.dll")]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
         ProcessAccessFlags processAccess,
         bool bInheritHandle,
         int processId
     );
        public Form1()
        {
            InitializeComponent();
        }

        private void getMapProcess(int id)
        {
            IntPtr hHandle = OpenProcess(ProcessAccessFlags.PROCESS_QUERY_INFORMATION, false, id);

            this.listBox3.Items.Add("Youre input the process number " + id);

            long finAddress = 0x7FFFFFFF;

            for (long bAddress = 0; bAddress < finAddress;)
            {
                MEMORY_BASIC_INFORMATION mbi;
                VirtualQueryEx(hHandle, (IntPtr)bAddress, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                this.listBox3.Items.Add("Current address: " + bAddress);
                this.listBox3.Items.Add("Base address of the area: " + (IntPtr)mbi.BaseAddress);

                if (mbi.State != StateEnum.MEM_FREE)
                {
                    this.listBox3.Items.Add("Base address of the selected area: " + mbi.AllocationBase + "\r\n");
                }
                bAddress = (long)mbi.BaseAddress + mbi.RegionSize;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            IntPtr hSnapshot = CreateToolhelp32Snapshot(SnapshotFlags.TH32CS_SNAPPROCESS, 0);
            PROCESSENTRY32 pstruct = new PROCESSENTRY32();
            pstruct.dwSize = (uint)Marshal.SizeOf(pstruct);
            Process32First(hSnapshot, ref pstruct);
            this.listBox2.Items.Add("ID       " + " Process name ");
            do
            {
                this.listBox2.Items.Add(pstruct.th32ProcessID + "    " + pstruct.szExeFile);
            } while (Process32Next(hSnapshot, ref pstruct));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            CathegoryOfMemory status = CathegoryOfMemory.CreateInstance();
            double MemoryLoad = status.MemoryLoad;
            double TotalPhys = status.TotalPhys;
            double AvailPhys = status.AvailPhys;
            double TotalPageFile = status.TotalPageFile;
            double AvailPageFile = status.AvailPageFile;
            double TotalVirtual = status.TotalVirtual;
            double AvailVirtual = status.AvailVirtual;
            double AvailExtendedVirtual = status.AvailExtendedVirtual;
            listBox1.Items.Add("1. Total RAM: " + Math.Round(TotalPhys / 1024 / 1024,2) + " MB");
            listBox1.Items.Add("2. Free RAM space: " + Math.Round(AvailPhys / 1024 / 1024,2) + " MB");
            listBox1.Items.Add("3. Swap file size: " + Math.Round(TotalPageFile / 1024 / 1024,2) + " MB");
            listBox1.Items.Add("4. Free swap file space : " + Math.Round(AvailPageFile / 1024 / 1024,2) + " MB");
            listBox1.Items.Add("5. Total virtual memory: " + Math.Round(TotalVirtual / 1024 / 1024,2) + " MB");
            listBox1.Items.Add("6. Free virtual memory space: " + Math.Round(AvailVirtual / 1024 / 1024,2) + " MB");
            listBox1.Items.Add("7. Memory involved in this process: " + Math.Round(MemoryLoad / 1024 / 1024, 4) + " MB");

            List<double> statuses = new List<double>() { TotalPhys, AvailPhys, TotalPageFile, AvailPageFile, TotalVirtual, AvailVirtual, MemoryLoad };
            this.chart1.Series["Распределение памяти"].Points.DataBindY(statuses);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox3.Items.Clear();

            int id = -1;
            try
            {
                id = Convert.ToInt32(textBox1.Text);
            }
            catch
            {
                listBox3.Items.Add("Enter ID ");
                return;
            }
            if (id != -1)
                getMapProcess(id);
            listBox3.Items.Add("Build completed successfully ");
        }

        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            All = (HeapList | Process | Thread | Module),
            Inherit = 0x80000000,
            TH32CS_SNAPPROCESS = 0x00000002,
            NoHeaps = 0x40000000

        }
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        };

        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            PROCESS_QUERY_INFORMATION = 0x00000400,
            QueryLimitedInformation = 0x00001000,

            Synchronize = 0x00100000
        }
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public AllocationProtectEnum AllocationProtect;
            public uint RegionSize;
            public StateEnum State;
            public AllocationProtectEnum Protect;
            public TypeEnum Type;
        }
        public enum AllocationProtectEnum : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        public enum StateEnum : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        public enum TypeEnum : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
