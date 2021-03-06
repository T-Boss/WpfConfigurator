﻿using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using WpfConfiguratorLib.entities;

namespace WpfConfiguratorLib
{
    public static class ConfigManager
    {
        #region Private Fields



        #endregion



        #region Public Properties

        public static string WorkingDirectory { get; set; }

        #endregion



        static ConfigManager()
        {
            // Set working directory
            WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Application.Current.MainWindow.GetType().Assembly.GetName().Name, "configdata");
            if (!Directory.Exists(WorkingDirectory)) Directory.CreateDirectory(WorkingDirectory);
            Console.WriteLine("WorkingDirectory=" + WorkingDirectory);
        }



        #region Private Methods



        #endregion



        #region Public Methods

        public static void Save(ConfigGroup configGroup)
        {
            try
            {
                var json = JsonConvert.SerializeObject(configGroup, Formatting.Indented, new SecureStringSerializer());
                File.WriteAllText(Path.Combine(WorkingDirectory, configGroup.DisplayName + ".config"), json);

                // Mark as initialized after saving
                configGroup.IsInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static T Load<T>(string displayName)
        {
            try
            {
                return (T) Convert.ChangeType(Load(displayName, typeof (T)), typeof (T));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default(T);
            }
        }

        public static ConfigGroup Load(string displayName, Type targetType)
        {
            try
            {
                // Make file path
                var filePath = Path.Combine(WorkingDirectory, displayName + ".config");

                // Check if file exists
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Config file not found at " + filePath);

                // Read file
                using (var file = File.OpenText(filePath))
                {
                    // Deserialize
                    var serializer = new JsonSerializer();
                    serializer.Converters.Add(new SecureStringSerializer());
                    var value = serializer.Deserialize(file, targetType);

                    // Mark as initialized after loading
                    var configGroup = value as ConfigGroup;
                    if (configGroup != null)
                    {
                        configGroup.IsInitialized = true;
                    }

                    return configGroup;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #endregion
    }
}
