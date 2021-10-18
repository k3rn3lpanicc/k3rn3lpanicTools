using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace k3rn3lpanicTools
{
    public class SystemInfo
    {
        public enum InfoType {
            Machinename,
            Username,
            ApplicationFullPath,
            ApplicationFullPath2,
            CurrentDirectory,
            OSversion,
            SystemDirectory,
            IsRunasADMIN,
            AppName
        }
        /// <summary>
        /// Returns the information U wish
        /// </summary>
        /// <returns></returns>
        public static string GetInfo(InfoType inf)
        {
            switch (inf) {
                case InfoType.Machinename:
                    return Environment.MachineName;       
                case InfoType.Username:
                    return Environment.UserName;
                case InfoType.ApplicationFullPath:
                    return Environment.GetCommandLineArgs()[0];
                case InfoType.CurrentDirectory:
                    return Environment.CurrentDirectory;
                case InfoType.OSversion:
                    return Environment.OSVersion.ToString();
                case InfoType.SystemDirectory:
                    return Environment.SystemDirectory;
                case InfoType.ApplicationFullPath2:
                    return Assembly.GetExecutingAssembly().Location;
                case InfoType.IsRunasADMIN:
                    return Tools.IsAdministrator().ToString();
                case InfoType.AppName:
                    return GetInfo(InfoType.ApplicationFullPath).Split('\\')[GetInfo(InfoType.ApplicationFullPath).Split('\\').Length-1];
            }
            
            return "";
        }
        public static string[] GetArgs()
        {
            return Environment.GetCommandLineArgs();
        }
       
    }
}
