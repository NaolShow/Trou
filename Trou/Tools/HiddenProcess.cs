using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Trou.Tools {

    /// <summary>
    /// Allows to start processes, they are placed in a virtual desktop in order to be hidden
    /// </summary>
    public class HiddenProcess : IDisposable {

        #region Variables

        #region Path

        /// <summary>
        /// <see cref="HiddenProcess"/> working directory
        /// </summary>
        public string WorkingDirectory;

        /// <summary>
        /// <see cref="HiddenProcess"/> executable file path
        /// </summary>
        public string FileName;

        #endregion

        #region Process

        /// <summary>
        /// <see cref="HiddenProcess"/> process instance
        /// </summary>
        public Process Process;

        /// <summary>
        /// Determines if the <see cref="HiddenProcess"/> should close when his parent process close
        /// </summary>
        public readonly bool EndAsParent;

        /// <summary>
        /// <see cref="HiddenProcess"/> arguments
        /// </summary>
        public string Arguments;

        /// <summary>
        /// <see cref="HiddenProcess"/> priority
        /// </summary>
        public PriorityClass Priority;

        /// <summary>
        /// <see cref="HiddenProcess"/> read buffer size
        /// </summary>
        public uint ReadBufferSize = 4096;

        #endregion

        #region Events

        /// <summary>
        /// Triggered when the process has completely started
        /// </summary>
        public event EventHandler<Process> OnStart;

        /// <summary>
        /// Triggered when the process output something
        /// </summary>
        public event EventHandler<string> OnOutput;

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new intance of the <see cref="HiddenProcess"/> class
        /// </summary>
        public HiddenProcess(string workingDirectory, string fileName, string arguments, PriorityClass priority, bool endAsParent) {

            // Save
            WorkingDirectory = workingDirectory;
            FileName = fileName;
            Arguments = arguments;
            EndAsParent = endAsParent;

            Priority = priority;

        }

        /// <summary>
        /// Starts the process, returns true if the process is started successfully
        /// </summary>
        public bool Start() {

            #region Create desktop

            // Create process virtual desktop
            CreateDesktop("HiddenDesktop", IntPtr.Zero, IntPtr.Zero, 0, (uint)ACCESS_MASK.GENERIC_ALL, IntPtr.Zero);

            #endregion

            #region Create process

            // Process informations (this is set after the process is started)
            PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
            // Process startup informations
            STARTUPINFO sInfo = new STARTUPINFO {

                dwFlags = 0x100, // STARTF_USESTDHANDLES
                lpDesktop = "HiddenDesktop"

            };

            #region Pipe

            // Pipes security attributes
            SECURITY_ATTRIBUTES pipeSecurityAttributes = new SECURITY_ATTRIBUTES() {
                nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES)),
                lpSecurityDescriptor = IntPtr.Zero,
                bInheritHandle = 1
            };

            // Pipe handles
            IntPtr hRead = IntPtr.Zero;
            IntPtr hWrite = IntPtr.Zero;

            // We want to read the process output
            if (OnOutput != null) {

                // The pipe has been created successfully
                if (CreatePipe(out hRead, out hWrite, ref pipeSecurityAttributes, 0)) {

                    // Set process error output, and start reading
                    sInfo.hStdOutput = hWrite;
                    Task.Run(() => ReadOutput(hRead));

                } else { return false; }

            }

            #endregion

            // Create the process
            if (!CreateProcess(null, string.Format("{0} {1}", FileName, Arguments), IntPtr.Zero, IntPtr.Zero, true, (uint)Priority, IntPtr.Zero, WorkingDirectory, ref sInfo, out pInfo)) {
                return false;
            }

            // Get process and save it
            Process = Process.GetProcessById(pInfo.dwProcessId);

            // Trigger event
            OnStart?.Invoke(this, Process);

            // Close process handles
            CloseHandle(pInfo.hProcess);
            CloseHandle(pInfo.hThread);

            // Close write pipe
            CloseHandle(hWrite);

            #endregion

            #region Process check

            // Check if the process isn't already closed
            // (Can occur when it's for example a console app)
            if (Process != null && !Process.HasExited) {

                // If the process should end when the parent process end
                if (EndAsParent) {

                    // Track the process to end when the parent end
                    ChildProcessTracker.AddProcess(Process);

                }

            }

            #endregion

            return true;

        }

        /// <summary>
        /// Reads the process output from an handle
        /// </summary>
        private void ReadOutput(IntPtr readHandle) {

            // Buffer & read length
            byte[] buffer = new byte[ReadBufferSize];
            uint dwRead = 0;

            // Read as long as the process is available (not closed..)
            while (ReadFile(readHandle, buffer, ReadBufferSize, ref dwRead, IntPtr.Zero)) {

                // For every buffer lines
                foreach (string line in Encoding.UTF8.GetString(buffer, 0, Convert.ToInt32(dwRead)).Split(Environment.NewLine)) {

                    // If the line isn't empty
                    if (!string.IsNullOrEmpty(line)) {

                        // Trigger event
                        OnOutput?.Invoke(this, line.Trim());

                    }

                }

                // Reset buffer
                buffer = new byte[ReadBufferSize];

            }

            // Close read handle
            CloseHandle(readHandle);

        }

        /// <summary>
        /// Kill and dispose the process
        /// </summary>
        public void Dispose() {

            // Dispose process
            Process?.Kill();
            Process?.Dispose();

        }

        #region Enums

        #region Process priorities

        /// <summary>
        /// Process priority enumeration
        /// </summary>
        public enum PriorityClass {

            ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
            BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
            HIGH_PRIORITY_CLASS = 0x00000080,
            IDLE_PRIORITY_CLASS = 0x00000040,
            NORMAL_PRIORITY_CLASS = 0x00000020,
            REALTIME_PRIORITY_CLASS = 0x00000100

        }


        #endregion
        #region Security Attributes

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        #endregion

        #endregion
        #region Dll-Imports

        #region Desktops

        [DllImport("user32.dll")]
        public static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags, uint dwDesiredAccess, IntPtr lpsa);

        [Flags]
        internal enum ACCESS_MASK : uint {
            DESKTOP_NONE = 0,
            DESKTOP_READOBJECTS = 0x0001,
            DESKTOP_CREATEWINDOW = 0x0002,
            DESKTOP_CREATEMENU = 0x0004,
            DESKTOP_HOOKCONTROL = 0x0008,
            DESKTOP_JOURNALRECORD = 0x0010,
            DESKTOP_JOURNALPLAYBACK = 0x0020,
            DESKTOP_ENUMERATE = 0x0040,
            DESKTOP_WRITEOBJECTS = 0x0080,
            DESKTOP_SWITCHDESKTOP = 0x0100,

            GENERIC_ALL = (DESKTOP_READOBJECTS | DESKTOP_CREATEWINDOW | DESKTOP_CREATEMENU |
                            DESKTOP_HOOKCONTROL | DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK |
                            DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP),
        }

        #endregion

        #region Pipes

        [DllImport("kernel32.dll")]
        static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, uint nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(IntPtr handle, byte[] buffer, uint toRead, ref uint read, IntPtr lpOverLapped);

        #endregion
        #region Create process

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFO {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        #endregion

    }

}
