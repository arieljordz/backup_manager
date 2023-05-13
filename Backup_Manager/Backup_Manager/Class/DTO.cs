using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Backup_Manager.Class
{
    public class FileOutput
    {
        public string selected { get; set; }
        public string destination { get; set; }
        public string source { get; set; }
        public string filename { get; set; }
        public int? rowID { get; set; }
    }

    public class DirectoryFile
    {
        public string path { get; set; }
        public bool IsFolder { get; set; }
    }

    public class NetworkCredential
    {
        public string domain { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class BackupSchedule
    {
        public DateTime Time { get; set; }
        public DateTime CustomTime { get; set; }
        public bool IsDaily { get; set; }
        public bool IsCustom { get; set; }
        public DateTime Date { get; set; }
        public string CustomPath { get; set; }
    }

    public class SettingsInfo
    {
        public bool IsDatedFolder { get; set; }
        public bool IsDatedSuffix { get; set; }
        public bool IsDefaultNetwork { get; set; }
        public bool IsAutoClose { get; set; }
        public bool IsAutoRunOnStart { get; set; }
        public bool IsScheduled { get; set; }
        public string Repository { get; set; }
        public bool IsDaily { get; set; }
        public string CustomPrefix { get; set; }
    }
}