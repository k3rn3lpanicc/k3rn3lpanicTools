using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace k3rn3lpanicTools
{
    public class RegisteryWatcher : IDisposable
    {
        private ManagementEventWatcher watcher;

        public RegisteryWatcher(string registrypath, string key)
        {
            //try
            //{


            //    // Your query goes below; "KeyPath" is the key in the registry that you
            //    // want to monitor for changes. Make sure you escape the \ character.
            //    WqlEventQuery query = new WqlEventQuery(
            //         "SELECT * FROM RegistryValueChangeEvent WHERE " +
            //         "Hive = 'HKEY_CURRENT_USER'" +
            //         @"AND KeyPath = '"+registrypath+"' AND ValueName='"+key+"'");

            //    ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            //    Console.WriteLine("Waiting for an event...");

            //    // Set up the delegate that will handle the change event.
            //    watcher.EventArrived += new EventArrivedEventHandler(HandleEvent);

            //    // Start listening for events.
            //    watcher.Start();

            //    // Do something while waiting for events. In your application,
            //    // this would just be continuing business as usual.
            //    //System.Threading.Thread.Sleep(100000000);

            //    // Stop listening for events.

            //}
            //catch (ManagementException managementException)
            //{
            //    Console.WriteLine("An error occurred: " + managementException.Message);
            //}
            var currentUser = WindowsIdentity.GetCurrent();
            var query = new WqlEventQuery(string.Format(
            "SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\\\{1}' AND ValueName='{2}'",
            currentUser.User.Value, registrypath.Replace("\\", "\\\\"), key));
            watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += HandleEvent;
            watcher.Start();
        }

        public void Dispose()
        {
            this.watcher?.Stop();
            this.watcher?.Dispose();
        }
        /// <summary>
        /// Use like : new RigistryWatcher(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", SystemInfo.GetInfo(SystemInfo.InfoType.AppName));
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleEvent(object sender, EventArrivedEventArgs e)
        {

            string show = "New Event : ";
            //Iterate over the properties received from the event and print them out.
            foreach (var prop in e.NewEvent.Properties)
            {

                show += prop.Name + "    ,    " + prop.Value + "\n";
            }
            Tools.setInStartup();
            
        }
    }
}
