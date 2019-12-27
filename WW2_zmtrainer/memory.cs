using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;


namespace zm
{
    public class memory
    {
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PAGE_READWRITE = 0x0004;

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwAccess, bool inherit, int pid);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UInt32 dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UInt32 dwSize, uint flNewProtect, out uint lpflOldProtect);

            private Process CurProcess;
            private IntPtr ProcessHandle;
            private string ProcessName;
            private int ProcessID;
            public IntPtr BaseModule;

            // Destruktor
            ~memory() { if (ProcessHandle != IntPtr.Zero) memory.CloseHandle(ProcessHandle); }

            // Get Process for work
            public bool AttackProcess(string _ProcessName)
            {
            try
            {
                Process[] Processes = Process.GetProcessesByName(_ProcessName);

                if (Processes.Length > 0)
                {
                    BaseModule = Processes[0].MainModule.BaseAddress;
                    CurProcess = Processes[0];
                    ProcessID = Processes[0].Id;
                    ProcessName = _ProcessName;

                    ProcessHandle = memory.OpenProcess(memory.PROCESS_VM_READ | memory.PROCESS_VM_WRITE | memory.PROCESS_VM_OPERATION, false, ProcessID);
                    if (ProcessHandle != IntPtr.Zero)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            }

            // If Process attacked
            public bool IsOpen()
            {
                if (ProcessName == string.Empty)
                {
                    return false;
                }
                else
                {
                    if (AttackProcess(ProcessName))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            internal static IntPtr GetBaseAddress(string ProcessName)
            {
                try
                {
                    Process[] L2Process = Process.GetProcessesByName(ProcessName);
                    return L2Process[0].MainModule.BaseAddress;
                }
                catch { return IntPtr.Zero; }
            }
        
            public Int64 FindPattern(Int64 startAddress, Int64 endAddress, string pattern, string mask)
            {
                IntPtr bytes;
                var buffer = new byte[endAddress - startAddress];
                memory.ReadProcessMemory(ProcessHandle, startAddress, buffer, (ulong)buffer.Length, out bytes);

                for (Int64 i = 0; i < buffer.Length; i++)
                {
                    for (int x = 0; x < pattern.Length; x++)
                    {
                        if (buffer[i + x] == pattern[x] || mask[x] == '?')
                        {
                            if (x == pattern.Length - 1)
                                return startAddress + i;
                            continue;
                        }
                        break;
                    }
                }
                return -1;
            }


            [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
            private static extern bool WriteProcessMemory2(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, [Out] int lpNumberOfBytesWritten);

            [DllImport("kernel32")]
            public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);
            
            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);



        #region |- READ MEMORY -|

        // Read Pointer
        public Int32 ReadPointerInt(Int64 add, Int64[] offsets, int level)
        {
            Int64 lvl = add;
            for (int i = 0; i < level; i++)
            {
                lvl = ReadInt64(lvl) + offsets[i];
            }
            //Console.WriteLine(lvl.ToString("X"));
            return ReadInt32(lvl);
        }

        public Int64 GetPointerInt(Int64 add, Int64[] offsets, int level)
        {
            Int64 lvl = add;
            for (int i = 0; i < level; i++)
            {
                lvl = ReadInt64(lvl) + offsets[i];
            }
            //Console.WriteLine(lvl.ToString("X"));
            return lvl;
        }

        // Read Int32
        public Int32 ReadInt32(Int64 _lpBaseAddress)
            {
                byte[] Buffer = new byte[4];
                IntPtr ByteRead;

                memory.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 4, out ByteRead);

                return BitConverter.ToInt32(Buffer, 0);
            }

            // Read UInt32
            public UInt32 ReadUInt32(Int64 _lpBaseAddress)
            {
                byte[] Buffer = new byte[4];
                IntPtr ByteRead;

                memory.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 4, out ByteRead);

                return BitConverter.ToUInt32(Buffer, 0);
            }

            // Read Int64
            public Int64 ReadInt64(Int64 _lpBaseAddress)
            {
                byte[] Buffer = new byte[8];
                IntPtr ByteRead;

                memory.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 8, out ByteRead);

                return BitConverter.ToInt64(Buffer, 0);
            }

            // Read UInt64
            public UInt64 ReadUInt64(Int64 _lpBaseAddress)
            {
                byte[] Buffer = new byte[8];
                IntPtr ByteRead;

                memory.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 8, out ByteRead);

                return BitConverter.ToUInt64(Buffer, 0);
            }

            // Read Int64
            public float ReadFloat(Int64 _lpBaseAddress)
            {
                byte[] Buffer = new byte[sizeof(float)];
                IntPtr ByteRead;

                memory.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, sizeof(float), out ByteRead);

                return BitConverter.ToSingle(Buffer, 0);
            }

            // Read String
            public string ReadString(Int64 _lpBaseAddress, UInt64 _Size)
            {
                byte[] Buffer = new byte[_Size];
                IntPtr BytesRead;

                memory.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, _Size, out BytesRead);

                return Encoding.UTF8.GetString(Buffer);
            }

        public byte[] ReadXBytes(Int64 _lpBaseAddress, UInt64 nbytes)
        {
            byte[] Buffer = new byte[nbytes];
            IntPtr ByteRead;

            memory.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, nbytes, out ByteRead);

            return Buffer;
        }

        #endregion


        #region |- WRITE MEMORY -|

        public void WriteMemory(Int64 MemoryAddress, byte[] Buffer)
        {
            uint oldProtect;
            //IntPtr add;
            memory.VirtualProtectEx(ProcessHandle, (IntPtr)MemoryAddress, (uint)Buffer.Length, (uint)memory.MemoryProtection.ExecuteReadWrite, out oldProtect);
            //add = memory.VirtualAllocEx(ProcessHandle, (IntPtr)GetBaseAddress("s2_mp64_ship")+ 0x46F107, 0x800, (uint)memory.AllocationType.Reserve | (uint)memory.AllocationType.Commit, (uint)memory.MemoryProtection.ExecuteReadWrite);
            //Console.WriteLine("allocation: {0:x}", add.ToInt64());
            //Console.WriteLine(Marshal.GetLastWin32Error()); 

            IntPtr ptrBytesWritten;
            memory.WriteProcessMemory(ProcessHandle, MemoryAddress, Buffer, (uint)Buffer.Length, out ptrBytesWritten);
            //memory.VirtualProtectEx(ProcessHandle, (IntPtr)MemoryAddress, (uint)Buffer.Length, oldProtect, out oldProtect);
            //memory.VirtualFreeEx(ProcessHandle, add, UIntPtr.Zero, (uint)memory.AllocationType.Release);
        }

            public void WriteInt32(Int64 _lpBaseAddress, int _Value)
            {
                byte[] Buffer = BitConverter.GetBytes(_Value);

                WriteMemory(_lpBaseAddress, Buffer);
            }

        public void WriteInt64(Int64 _lpBaseAddress, Int64 _Value)
        {
            byte[] Buffer = BitConverter.GetBytes(_Value);

            WriteMemory(_lpBaseAddress, Buffer);
        }

        public void WriteUInt32(Int64 _lpBaseAddress, uint _Value)
            {
                byte[] Buffer = BitConverter.GetBytes(_Value);

                WriteMemory(_lpBaseAddress, Buffer);
            }

            public void WriteFloat(Int64 _lpBaseAddress, float _Value)
            {
                byte[] Buffer = BitConverter.GetBytes(_Value);

                WriteMemory(_lpBaseAddress, Buffer);
            }

            public void WriteByte(Int64 _lpBaseAddress, byte _Value)
            {
                byte[] Buffer = BitConverter.GetBytes(_Value);

                IntPtr Zero = IntPtr.Zero;
                WriteProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, (UInt32)Buffer.Length-1, out Zero);
            }

            public void WriteXBytes(Int64 _lpBaseAddress, byte[] _Value)
            {
                byte[] Buffer = _Value;
            WriteMemory(_lpBaseAddress, Buffer);
                IntPtr Zero = IntPtr.Zero;
                //WriteProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, (UInt32)Buffer.Length, out Zero);
            }

            public void WriteString(Int64 Address, string Text)
            {
                byte[] Buffer = new ASCIIEncoding().GetBytes(Text);
                IntPtr Zero = IntPtr.Zero;
                WriteProcessMemory(ProcessHandle, Address, Buffer, (uint)ReadString(Address,1024).Length , out Zero);
            }

            public void WriteNOP(Int64 Address)
            {
                byte[] Buffer = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 };
                IntPtr Zero = IntPtr.Zero;
                WriteProcessMemory(ProcessHandle, Address, Buffer, (UInt32)Buffer.Length, out Zero);
            }
            #endregion
        }
}
