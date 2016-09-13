﻿/* Taken form https://github.com/CorpenEldorito/Corps-H5F-Tool
 * Credit goes to Corpen
 */

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Memory
{
    class Manager
    {
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        public static byte[] ReadFromAddress(Int32 address)
        {
            Process p = Process.GetProcessesByName("halo5forge").FirstOrDefault();
            Int64 startOffset = p.MainModule.BaseAddress.ToInt64();
            Int64 offset = startOffset + address;
            var hProc = OpenProcess(ProcessAccessFlags.All, false, (int)p.Id);
            int unused = 0;
            IntPtr addr = new IntPtr(offset);
            byte[] hex = new byte[4];
            ReadProcessMemory(hProc, addr, hex, (UInt32)hex.LongLength, ref unused);
            return hex;
        }

        public static void WriteToAddress(Int32 address, byte[] hex)
        {
            Process p = Process.GetProcessesByName("halo5forge").FirstOrDefault();
            Int64 startOffset = p.MainModule.BaseAddress.ToInt64();
            Int64 offset = startOffset + address;
            var hProc = OpenProcess(ProcessAccessFlags.All, false, (int)p.Id);
            int unused = 0;
            IntPtr addr = new IntPtr(offset);
            WriteProcessMemory(hProc, addr, hex, (UInt32)hex.LongLength, out unused);

            CloseHandle(hProc);
        }
    }
}