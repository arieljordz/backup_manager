using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Backup_Manager.Class
{
    public class XML_Config
    {
        //public void Create_XML(List<FileOutput> sources, BackupSchedule backupSchedule, SettingsInfo settings)
        //{
        //    var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        //    var path = currentDir + "Settings.xml";


        //    XmlWriter writer = XmlWriter.Create(path);
        //    writer.WriteStartDocument();

        //    writer.WriteStartElement("BackupSchedule");
        //    writer.WriteStartAttribute("Date");
        //    writer.WriteValue(Convert.ToString(backupSchedule.Date));
        //    writer.WriteEndAttribute();
        //    writer.WriteStartAttribute("Time");
        //    writer.WriteValue(Convert.ToString(backupSchedule.Time));
        //    writer.WriteEndAttribute();
        //    writer.WriteStartAttribute("CustomTime");
        //    writer.WriteValue(Convert.ToString(backupSchedule.CustomTime));
        //    writer.WriteEndAttribute();
        //    writer.WriteStartAttribute("IsDaily");
        //    writer.WriteValue(Convert.ToString(backupSchedule.IsDaily));
        //    writer.WriteEndAttribute();
        //    writer.WriteStartAttribute("IsCustom");
        //    writer.WriteValue(Convert.ToString(backupSchedule.IsCustom));
        //    writer.WriteEndAttribute();
        //    writer.WriteStartAttribute("CustomPath");
        //    writer.WriteValue(Convert.ToString(backupSchedule.CustomPath));
        //    writer.WriteEndAttribute();

        //    writer.WriteStartElement("IsDated");
        //    writer.WriteStartAttribute("value");
        //    writer.WriteValue(settings.IsDatedFolder);
        //    writer.WriteEndAttribute();

        //    writer.WriteStartElement("IsSuffixed");
        //    writer.WriteStartAttribute("value");
        //    writer.WriteValue(settings.IsDatedSuffix);
        //    writer.WriteEndAttribute();

        //    writer.WriteStartElement("IsDefaultNetwork");
        //    writer.WriteStartAttribute("value");
        //    writer.WriteValue(settings.IsDefaultNetwork);
        //    writer.WriteEndAttribute();

        //    writer.WriteStartElement("IsAutoClose");
        //    writer.WriteStartAttribute("value");
        //    writer.WriteValue(settings.IsAutoClose);
        //    writer.WriteEndAttribute();

        //    writer.WriteStartElement("IsAutoRunOnStart");
        //    writer.WriteStartAttribute("value");
        //    writer.WriteValue(settings.IsAutoRunOnStart);
        //    writer.WriteEndAttribute();

        //    writer.WriteStartElement("IsScheduled");
        //    writer.WriteStartAttribute("value");
        //    writer.WriteValue(settings.IsScheduled);
        //    writer.WriteEndAttribute();

        //    writer.WriteStartElement("IsDaily");
        //    writer.WriteStartAttribute("value");
        //    writer.WriteValue(settings.IsDaily);
        //    writer.WriteEndAttribute();

        //    var CustomPrefix = settings.IsDaily == true ? "DBS_" : "WBS_";
        //    writer.WriteStartElement("CustomPrefix");
        //    writer.WriteAttributeString("value", CustomPrefix);
        //    writer.WriteEndElement();

        //    foreach (var source in sources)
        //    {
        //        writer.WriteStartElement("Source");
        //        writer.WriteAttributeString("destination", source.destination);
        //        writer.WriteAttributeString("name", source.filename);
        //        writer.WriteAttributeString("path", source.source);
        //        writer.WriteAttributeString("rowID", Convert.ToString(source.rowID));
        //        writer.WriteEndElement();
        //    }
        //    //

        //    writer.WriteEndDocument();
        //    writer.Flush();
        //    writer.Dispose();

        //    if (System.IO.File.Exists(path))
        //    {
        //        XDocument xdoc = XDocument.Load(path);
        //        xdoc.Save(path);
        //    }

        //}

        public void Save_XML_Settings(BackupSchedule backupSchedule, SettingsInfo settings, List<FileOutput> sources)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = currentDir + "Settings.xml";

            XmlWriter writer = XmlWriter.Create(path);
            writer.WriteStartDocument();

            writer.WriteStartElement("BackupSchedule");
            writer.WriteStartAttribute("Date");
            writer.WriteValue(Convert.ToString(backupSchedule.Date));
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("Time");
            writer.WriteValue(Convert.ToString(backupSchedule.Time));
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("CustomTime");
            writer.WriteValue(Convert.ToString(backupSchedule.CustomTime));
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("IsDaily");
            writer.WriteValue(Convert.ToString(backupSchedule.IsDaily));
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("IsCustom");
            writer.WriteValue(Convert.ToString(backupSchedule.IsCustom));
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("CustomPath");
            writer.WriteValue(Convert.ToString(backupSchedule.CustomPath));
            writer.WriteEndAttribute();

            writer.WriteStartElement("IsDated");
            writer.WriteStartAttribute("value");
            writer.WriteValue(settings.IsDatedFolder);
            writer.WriteEndAttribute();

            writer.WriteStartElement("IsSuffixed");
            writer.WriteStartAttribute("value");
            writer.WriteValue(settings.IsDatedSuffix);
            writer.WriteEndAttribute();

            writer.WriteStartElement("IsDefaultNetwork");
            writer.WriteStartAttribute("value");
            writer.WriteValue(settings.IsDefaultNetwork);
            writer.WriteEndAttribute();

            writer.WriteStartElement("IsAutoClose");
            writer.WriteStartAttribute("value");
            writer.WriteValue(settings.IsAutoClose);
            writer.WriteEndAttribute();

            writer.WriteStartElement("IsAutoRunOnStart");
            writer.WriteStartAttribute("value");
            writer.WriteValue(settings.IsAutoRunOnStart);
            writer.WriteEndAttribute();

            writer.WriteStartElement("IsScheduled");
            writer.WriteStartAttribute("value");
            writer.WriteValue(settings.IsScheduled);
            writer.WriteEndAttribute();

            writer.WriteStartElement("IsDaily");
            writer.WriteStartAttribute("value");
            writer.WriteValue(settings.IsDaily);
            writer.WriteEndAttribute();

            var CustomPrefix = settings.IsDaily == true ? "DBS_" : "WBS_";
            writer.WriteStartElement("CustomPrefix");
            writer.WriteAttributeString("value", CustomPrefix);
            writer.WriteEndElement();
            
            writer.WriteEndDocument();
            writer.Flush();
            writer.Dispose();

            if (System.IO.File.Exists(path))
            {
                XDocument xdoc = XDocument.Load(path);
                foreach (var item in sources)
                {
                    string rowID = Convert.ToString(item.rowID);
                    var qry = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == rowID).SingleOrDefault();

                    if (qry != null)
                    {
                        qry.Attribute("selected").Value = item.selected;
                        qry.Attribute("name").Value = item.filename;
                        qry.Attribute("path").Value = item.source;
                        qry.Attribute("destination").Value = item.destination;
                    }
                    else
                    {
                        xdoc.Descendants("CustomPrefix")
                        .ElementAt(0)
                        .Add(new XElement("Source"
                        , new XAttribute("rowID", Convert.ToString(item.rowID))
                        , new XAttribute("selected", item.selected == null? "false": "true")
                        , new XAttribute("name", item.filename)
                        , new XAttribute("path", item.source)
                        , new XAttribute("destination", item.destination)));
                    }
                    xdoc.Save(path);
                }
            }

        }

        public void Save_XML_Task(FileOutput sources)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = currentDir + "Settings.xml";

            if (System.IO.File.Exists(path))
            {
                XDocument xdoc = XDocument.Load(path);

                string rowID = Convert.ToString(sources.rowID);
                var qry = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == rowID).SingleOrDefault();

                if (qry != null)
                {
                    qry.Attribute("name").Value = sources.filename;
                    qry.Attribute("path").Value = sources.source;
                    qry.Attribute("destination").Value = sources.destination;
                }
                else
                {
                    xdoc.Descendants("CustomPrefix")
                    .ElementAt(0)
                    .Add(new XElement("Source"
                    , new XAttribute("rowID", Convert.ToString(sources.rowID))
                    , new XAttribute("name", sources.filename)
                    , new XAttribute("path", sources.source)
                    , new XAttribute("destination", sources.destination)));
                }
                xdoc.Save(path);
            }
        }
        public static BackupSchedule LoadXMLBackupSchedule()
        {
            BackupSchedule backupSchedule = new BackupSchedule();
            try
            {
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                XDocument xdoc = XDocument.Load(currentDir + "\\Settings.xml");
                var date = xdoc.Descendants("BackupSchedule").Select(x => x.Attribute("Date").Value).FirstOrDefault();
                backupSchedule.Date = Convert.ToDateTime(date);

                var time = xdoc.Descendants("BackupSchedule").Select(x => x.Attribute("Time").Value).FirstOrDefault();
                backupSchedule.Time = Convert.ToDateTime(time);

                var ctime = xdoc.Descendants("BackupSchedule").Select(x => x.Attribute("CustomTime").Value).FirstOrDefault();
                backupSchedule.CustomTime = Convert.ToDateTime(ctime);

                var isdaily = xdoc.Descendants("BackupSchedule").Select(x => x.Attribute("IsDaily").Value).FirstOrDefault();
                backupSchedule.IsDaily = Convert.ToBoolean(isdaily);

                var iscustom = xdoc.Descendants("BackupSchedule").Select(x => x.Attribute("IsCustom").Value).FirstOrDefault();
                backupSchedule.IsCustom = Convert.ToBoolean(iscustom);

                var CustomPath = xdoc.Descendants("BackupSchedule").Select(x => x.Attribute("CustomPath").Value).FirstOrDefault();
                backupSchedule.CustomPath = Convert.ToString(CustomPath);
                return backupSchedule;
            }
            catch (Exception ex)
            {
                return new BackupSchedule();
            }
        }

        public static List<FileOutput> LoadXMLFileOutput(string[] task_ids)
        {
            List<FileOutput> Source = new List<FileOutput>();
            try
            {
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                XDocument xdoc = XDocument.Load(currentDir + "\\Settings.xml");

                var sources_list = xdoc.Descendants("Source").ToList();

                if (task_ids != null)
                {
                    foreach (var item in task_ids)
                    {
                        var data = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == item).SingleOrDefault();
                        Source.Add(new FileOutput()
                        {
                            selected = Convert.ToString(data.Attribute("selected").Value),
                            destination = Convert.ToString(data.Attribute("destination").Value),
                            filename = Convert.ToString(data.Attribute("name").Value),
                            source = Convert.ToString(data.Attribute("path").Value),
                            rowID = Convert.ToInt32(data.Attribute("rowID").Value)
                        });
                    }
                }
                else
                {
                    foreach (var item in sources_list)
                    {
                        var data = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == item.Attribute("rowID").Value).SingleOrDefault();
                        Source.Add(new FileOutput()
                        {
                            selected = Convert.ToString(data.Attribute("selected").Value),
                            destination = Convert.ToString(data.Attribute("destination").Value),
                            filename = Convert.ToString(data.Attribute("name").Value),
                            source = Convert.ToString(data.Attribute("path").Value),
                            rowID = Convert.ToInt32(data.Attribute("rowID").Value)
                        });
                    }
                }
                return Source;
            }
            catch (Exception ex)
            {
                return new List<FileOutput>();
            }
        }

        public static SettingsInfo LoadXMLSettings()
        {
            SettingsInfo settingsInfo = new SettingsInfo();
            try
            {
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                XDocument xdoc = XDocument.Load(currentDir + "\\Settings.xml");
                var dated = xdoc.Descendants("IsDated").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.IsDatedFolder = Convert.ToBoolean(dated);

                var suffixed = xdoc.Descendants("IsSuffixed").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.IsDatedSuffix = Convert.ToBoolean(suffixed);

                var isdefaultNetwork = xdoc.Descendants("IsDefaultNetwork").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.IsDefaultNetwork = Convert.ToBoolean(isdefaultNetwork);

                var autoclose = xdoc.Descendants("IsAutoClose").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.IsAutoClose = Convert.ToBoolean(autoclose);

                var autorun = xdoc.Descendants("IsAutoRunOnStart").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.IsAutoRunOnStart = Convert.ToBoolean(autorun);

                var issched = xdoc.Descendants("IsScheduled").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.IsScheduled = Convert.ToBoolean(issched);

                var isdaily = xdoc.Descendants("IsDaily").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.IsDaily = Convert.ToBoolean(isdaily);

                var prefix = xdoc.Descendants("CustomPrefix").Select(x => x.Attribute("value").Value).FirstOrDefault();
                settingsInfo.CustomPrefix = Convert.ToString(prefix);

                return settingsInfo;
            }
            catch (Exception ex)
            {
            }
            return new SettingsInfo();
        }


    }
}