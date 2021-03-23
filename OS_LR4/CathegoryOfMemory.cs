using System.Runtime.InteropServices;
using System.Security;

namespace LR4
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    class CathegoryOfMemory
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GlobalMemoryStatusEx([In, Out] CathegoryOfMemory lpBuffer);

        private uint dwLength;
        public uint MemoryLoad;
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;

        private static volatile CathegoryOfMemory singleton;
        private static readonly object syncroot = new object();
        public static CathegoryOfMemory CreateInstance()
        {
            if (singleton == null)
                lock (syncroot)
                    if (singleton == null)
                        singleton = new CathegoryOfMemory();
            return singleton;
        }

        [SecurityCritical]
        private CathegoryOfMemory()
        {
                dwLength = (uint)Marshal.SizeOf(typeof(CathegoryOfMemory));
                GlobalMemoryStatusEx(this);
        }
    }
}