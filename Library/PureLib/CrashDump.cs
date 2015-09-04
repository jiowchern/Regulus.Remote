using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Regulus.Utility
{
    public static class CrashDump
    {
        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);


        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct MiniDumpExceptionInformation
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            [MarshalAs(UnmanagedType.Bool)]
            public bool ClientPointers;
        }

        [Flags]
        public enum MINIDUMP_TYPE
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000
        }

        

        public static bool Write()
        {
            /*var options =
                MINIDUMP_TYPE.MiniDumpWithDataSegs |
                MINIDUMP_TYPE.MiniDumpWithProcessThreadData | 
                MINIDUMP_TYPE.MiniDumpWithThreadInfo |
                MINIDUMP_TYPE.MiniDumpWithFullMemory;*/

            //var options = MINIDUMP_TYPE.MiniDumpWithThreadInfo;

            var options = 
                MINIDUMP_TYPE.MiniDumpWithProcessThreadData
                | MINIDUMP_TYPE.MiniDumpWithFullMemory ;

            return Write(options);
        }
        public static bool Write(MINIDUMP_TYPE options )
        {

            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".dmp");
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                Process currentProcess = Process.GetCurrentProcess();

                IntPtr currentProcessHandle = currentProcess.Handle;

                uint currentProcessId = (uint)currentProcess.Id;
                
                return MiniDumpWriteDump(currentProcessHandle, currentProcessId, fs.SafeFileHandle, (uint)options, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}