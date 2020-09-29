using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace StormFog.Agent.BusinessLogic.Services
{
    public class ConfigService
    {
        public ConfigService()
        {

        }

        public void Initialize()
        {
            // Create a StormFog software key if it does not exist
            if (Registry.CurrentUser.OpenSubKey(@"SOFTWARE\StormFog") != null)
                Registry.CurrentUser.CreateSubKey(@"SOFTWARE\StormFog");

            // Create a jobs key if it does not exist
            if (Registry.CurrentUser.OpenSubKey(@"SOFTWARE\StormFog\Jobs") != null)
                Registry.CurrentUser.CreateSubKey(@"SOFTWARE\StormFog\Jobs");

            // Define software values
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\StormFog"))
            {
                    
                //key.SetValue("StormFogPath", System.Reflection.Assembly.GetExecutingAssembly().get);
                key.SetValue("StormFogExe", System.Reflection.Assembly.GetExecutingAssembly().GetName().FullName);
                key.SetValue("LastExecution", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                key.SetValue("Version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            }
        }


    }
}
