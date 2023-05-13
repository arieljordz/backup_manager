using Backup_Manager.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using static Backup_Manager.Class.NetworkIdentity;

namespace Backup_Manager.Class
{
    public class GlobalRepository
    {
        public NetworkCredential networkCredential = new NetworkCredential();
        public List<DirectoryFile> GetFiles(string location)
        {
            var files = new List<DirectoryFile>();
            var dirsPassed = new List<string>();
            var dirRoots = new List<string>();
            dirRoots.Add(location);
            do
            {
                foreach (var dir in dirRoots.ToList().Where(x => !dirsPassed.Contains(x)))
                {
                    var subdirs = Directory.GetDirectories(dir);
                    foreach (var subdir in subdirs)
                    {
                        files.Add(new DirectoryFile { IsFolder = true, path = subdir });
                        dirRoots.Add(subdir);
                    }
                    var subfiles = Directory.GetFiles(dir);
                    foreach (var file in subfiles)
                    {
                        files.Add(new DirectoryFile { IsFolder = false, path = file });
                    }
                    dirsPassed.Add(dir);
                }
            }
            while (dirRoots.Where(x => !dirsPassed.ToList().Contains(x)).Count() != 0);
            return files.ToList();
        }

        public void Compress(List<DirectoryFile> files, FileOutput output)
        {
            using (var stream = new FileStream(output.source + ".zip", FileMode.Create))
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    foreach (var item in files)
                    {
                        var fname = item.path.Replace("\\", "/").Split('/').LastOrDefault();

                        var fileLocation = item.path.Replace(output.source, "").Replace("\\", "/");
                        var isFolder = item.IsFolder;
                        if (fileLocation.Replace("/", "@").IndexOf("@") == 0)
                        {
                            fileLocation = fileLocation.Substring(1);
                            if (isFolder)
                                fileLocation += "/";
                        }

                        if (!isFolder)
                        {
                            try
                            {
                                using (var dataStream = new MemoryStream())
                                {
                                    dataStream.Seek(0, SeekOrigin.Begin);
                                    using (var fs = new FileStream(item.path, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        fs.Seek(0, SeekOrigin.Begin);
                                        fs.CopyTo(dataStream);
                                    }

                                    var zipFile = archive.CreateEntry(fileLocation, CompressionLevel.Optimal);
                                    using (var zip = zipFile.Open())
                                    {
                                        dataStream.Seek(0, SeekOrigin.Begin);
                                        dataStream.CopyTo(zip);
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                isFolder = true;
                            }
                        }

                        if (isFolder)
                        {
                            var zipFile = archive.CreateEntry(fileLocation, CompressionLevel.Optimal);
                        }
                    }
                }
            }
        }

        public List<string> PrepareDestination(FileOutput output, BackupSchedule backupSchedule, SettingsInfo settingsInfo)
        {
            var list = new List<string>();
            var destFname = output.filename != "" ? output.filename
                    : output.source.Replace("\\", "/").Split('/').LastOrDefault() + ".zip";

            var destinations = new List<string>();

            destFname = settingsInfo.CustomPrefix + destFname;
            destinations.Add(output.destination);

            foreach (var destFolder in destinations)
            {
                var destLocation = destFolder + "\\" + destFname;
                if (settingsInfo.IsDatedFolder)
                {
                    var date = DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Year;
                    if (!Directory.Exists(destFolder + "\\" + date))
                    {
                        if (settingsInfo.IsDefaultNetwork)
                        {
                            Directory.CreateDirectory(destFolder + "\\" + date);
                        }
                        else
                        {
                            ImpersonationHelper.Impersonate(networkCredential.domain, networkCredential.username, networkCredential.password, delegate
                            {
                                Directory.CreateDirectory(destFolder + "\\" + date);
                            });
                        }
                    }
                    destLocation = destFolder + "\\" + date + "\\" + destFname;
                }
                if (settingsInfo.IsDatedSuffix)
                {
                    var date = DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Year;
                    destLocation = destLocation.Replace(".zip", "_" + date + ".zip");
                }
                list.Add(destLocation);
            }
            return list;
        }


        public Task CopyToLocation(string src, string dest, SettingsInfo settingsInfo)
        {
            return Task.Run(() => ImpersonateCopy(src, dest, settingsInfo));
        }

        private void ImpersonateCopy(string src, string dest, SettingsInfo settingsInfo)
        {
            if (settingsInfo.IsDefaultNetwork)
            {
                File.Copy(src, dest, true);
            }
            else
            {
                ImpersonationHelper.Impersonate(networkCredential.domain, networkCredential.username, networkCredential.password, delegate
                {
                    File.Copy(src, dest, true);
                });
            }
        }















    }
}