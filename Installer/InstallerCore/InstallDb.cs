﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InstallerCore
{
    public class InstallRecord
    {
        [JsonPropertyName("versionString")]
        public string VersionString { get; }

        [JsonPropertyName("vanillaMd5")]
        public string VanillaMd5 { get; }

        public InstallRecord(string versionString, string vanillaMd5)
        {
            VersionString = versionString;
            VanillaMd5 = vanillaMd5;
        }
    }

    public class InstallDb
    {
        private static readonly Logger _log = Logger.GetLogger;

        [JsonPropertyName("default")]
        public InstallRecord DefaultRecord { get; set; }

        [JsonPropertyName("standalone")]
        public InstallRecord Standalone { get; set; }

        public InstallDb()
        {
        }

        public static InstallDb LoadOrEmpty(string fileName)
        {
            try
            {
                string jsonString = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<InstallDb>(jsonString);
            }
            catch (FileNotFoundException)
            {
                return new InstallDb();
            }
        }

        public void Save(string fileName)
        {
            string tmpFileName = fileName + ".tmp";

            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(this);
            File.WriteAllBytes(tmpFileName, jsonUtf8Bytes);

            _log.WriteLine($"Updating {fileName}");
            File.Delete(fileName);      // If the file to be deleted does not exist, no exception is thrown.
            File.Move(tmpFileName, fileName);
        }
    }
}
