using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Backup_Manager.Class;
using Microsoft.AspNet.SignalR;
using Backup_Manager;
using System.Threading;
using Backup_Manager.Utility;
using System.Xml;

namespace Backup_Manager.Controllers
{
    public class HomeController : Controller
    {
        public SettingsInfo settingsInfo = new SettingsInfo();
        public BackupSchedule backupSchedule = new BackupSchedule();
        public List<FileOutput> sourcesInfo = new List<FileOutput>();
        public FileOutput sources = new FileOutput();

        GlobalRepository global = new GlobalRepository();
        XML_Config xml = new XML_Config();

        public ActionResult Index()
        {
            return View();
        }

        public int Get_Current_Task_ID()
        {
            var current_id = 1;
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = currentDir + "Settings.xml";

            if (System.IO.File.Exists(path))
            {
                XDocument xdoc = XDocument.Load(path);
                current_id = Convert.ToInt32(xdoc.Descendants("Source").Select(x => x.Attribute("rowID").Value).LastOrDefault());
                current_id++;
            }
            return current_id;
        }

        public ActionResult Save_Settings(SettingsInfo setting, FileOutput task)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = currentDir + "Settings.xml";

            if (System.IO.File.Exists(path))
            {
                sourcesInfo = XML_Config.LoadXMLFileOutput(null);
                backupSchedule = XML_Config.LoadXMLBackupSchedule();
                settingsInfo = XML_Config.LoadXMLSettings();

                SettingsInfo settings = new SettingsInfo()
                {
                    IsDatedFolder = setting.IsDatedFolder,
                    IsDatedSuffix = setting.IsDatedSuffix,
                    IsDefaultNetwork = setting.IsDefaultNetwork,
                    IsAutoClose = setting.IsAutoClose,
                    IsAutoRunOnStart = setting.IsAutoRunOnStart,
                    IsScheduled = setting.IsScheduled,
                    IsDaily = setting.IsDaily,
                    Repository = null
                };
                settingsInfo = settings;

                if (task.filename != null && task.source != null)
                {
                    var qry = sourcesInfo.Where(x => x.rowID == task.rowID).SingleOrDefault();
                    if (qry == null)
                    {
                        sourcesInfo.Add(new FileOutput()
                        {
                            destination = Convert.ToString(task.destination),
                            filename = Convert.ToString(task.filename),
                            source = Convert.ToString(task.source),
                            rowID = task.rowID == null ? Get_Current_Task_ID() : task.rowID
                        });
                    }
                    else
                    {
                        qry.destination = Convert.ToString(task.destination);
                        qry.filename = Convert.ToString(task.filename);
                        qry.source = Convert.ToString(task.source);
                        qry.rowID = task.rowID == null ? Get_Current_Task_ID() : task.rowID;
                    }
                }
            }
            else
            {
                SettingsInfo settings = new SettingsInfo()
                {
                    IsDatedFolder = setting.IsDatedFolder,
                    IsDatedSuffix = setting.IsDatedSuffix,
                    IsDefaultNetwork = setting.IsDefaultNetwork,
                    IsAutoClose = setting.IsAutoClose,
                    IsAutoRunOnStart = setting.IsAutoRunOnStart,
                    IsScheduled = setting.IsScheduled,
                    IsDaily = setting.IsDaily,
                    Repository = null
                };
                settingsInfo = settings;

                var sources = new List<FileOutput>();
                if (task.destination != null)
                {
                    sources.Add(new FileOutput()
                    {
                        destination = Convert.ToString(task.destination),
                        filename = Convert.ToString(task.filename),
                        source = Convert.ToString(task.source),
                        rowID = task.rowID == null ? Get_Current_Task_ID() : task.rowID
                    });
                }
                sourcesInfo = sources;
            }
            xml.Save_XML_Settings(backupSchedule, settingsInfo, sourcesInfo);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update_Task(string task_id)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            XDocument xdoc = XDocument.Load(currentDir + "\\Settings.xml");
            var qry = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == task_id).SingleOrDefault();
            FileOutput data = new FileOutput();
            if (qry != null)
            {
                {
                    data.rowID = Convert.ToInt32(qry.Attribute("rowID").Value);
                    data.source = Convert.ToString(qry.Attribute("path").Value);
                    data.destination = Convert.ToString(qry.Attribute("destination").Value);
                    data.filename = Convert.ToString(qry.Attribute("name").Value);
                }
            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete_Task(string task_id)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = currentDir + "Settings.xml";
            XDocument xdoc = XDocument.Load(path);
            var qry = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == task_id);
            //var q = (from node in xdoc.Descendants("Source") let attr = node.Attribute("rowID") where attr.Value == task_id select node);
            //q.ToList().ForEach(x => x.Remove());
            qry.ToList().ForEach(x => x.Remove());
            xdoc.Save(path);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Select_Task(string task_id)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = currentDir + "Settings.xml";

            try
            {
                if (System.IO.File.Exists(path))
                {
                    XDocument xdoc = XDocument.Load(path);
                    var qry = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == task_id).SingleOrDefault();

                    if (qry != null)
                    {
                        qry.Attribute("name").Value = qry.Attribute("name").Value;
                        qry.Attribute("path").Value = qry.Attribute("path").Value;
                        qry.Attribute("destination").Value = qry.Attribute("destination").Value;
                        qry.Attribute("selected").Value = qry.Attribute("selected").Value == "true" ? "false" : "true";
                    }
                    xdoc.Save(path);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Select_All_Task(string[] task_ids, string is_true)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = currentDir + "Settings.xml";

            try
            {
                if (System.IO.File.Exists(path))
                {
                    XDocument xdoc = XDocument.Load(path);
                    var sources = xdoc.Descendants("Source").ToList();
                    foreach (var item in sources)
                    {
                        string rowID = Convert.ToString(item.Attribute("rowID").Value);
                        var qry = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == rowID).SingleOrDefault();

                        if (qry != null)
                        {
                            qry.Attribute("name").Value = item.Attribute("name").Value;
                            qry.Attribute("path").Value = item.Attribute("path").Value;
                            qry.Attribute("destination").Value = item.Attribute("destination").Value;
                            qry.Attribute("selected").Value = is_true;
                        }
                    }
                    xdoc.Save(path);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadSettings()
        {
            var backup = XML_Config.LoadXMLBackupSchedule();
            var sources = XML_Config.LoadXMLFileOutput(null);
            var settings = XML_Config.LoadXMLSettings();

            List<object> objlist = new List<object>();
            foreach (var item in sources)
            {
                FileOutput obj1 = new FileOutput();
                {
                    obj1.selected = item.selected == null ? "false" : item.selected;
                    obj1.rowID = item.rowID;
                };
                var obj = new
                {
                    row_id = item.rowID,
                    output_filename = item.filename,
                    source_file = item.source,
                    selected = item.selected,
                    obj1 = obj1,
                    destination = item.destination,
                    IsDatedFolder = settings.IsDatedFolder,
                    IsDatedSuffix = settings.IsDatedSuffix,
                    IsDefaultNetwork = settings.IsDefaultNetwork,
                    IsScheduled = settings.IsScheduled,
                    IsDaily = settings.IsDaily,
                };
                objlist.Add(obj);
            }
            return Json(new { data = objlist }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Start_Backup(string[] task_ids)
        {
            try
            {
                var backup = XML_Config.LoadXMLBackupSchedule();
                var settings = XML_Config.LoadXMLSettings();
                var outputs = XML_Config.LoadXMLFileOutput(task_ids);

                var string_count = "";
                var countFinished = 0;
                var percent = 0;

                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                foreach (var output in outputs)
                {
                    var files = global.GetFiles(output.source);
                    await Task.Delay(1000);

                    await Task.Run(() => global.Compress(files, output));
                    await Task.Delay(1000);

                    var desttinations = new List<string>();
                    desttinations = global.PrepareDestination(output, backup, settings);

                    foreach (var dest in desttinations)
                    {
                        await global.CopyToLocation(output.source + ".zip", dest + ".zip", settings);
                    }
                    System.IO.File.Delete(output.source + ".zip");

                    await Task.Delay(1000);
                    countFinished++;
                    string_count = "Backup is complete! " + countFinished + " of " + outputs.Count();
                }
                watch.Stop();
                var execution_seconds = Get_Execution_Time((int)watch.ElapsedMilliseconds);

                for (int i = 0; i < execution_seconds; i++)
                {
                    //SIMULATING SOME TASK
                    Thread.Sleep(500);
                    //INCREMENT 1 FOR PERCENT
                    percent++;
                    //CALLING A FUNCTION THAT CALCULATES PERCENTAGE AND SENDS THE DATA TO THE CLIENT
                    Functions.SendProgress("Backup in progress...", percent, execution_seconds, string_count);
                }

                return Json(new { success = true, message = string_count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public int Get_Execution_Time(int execution_ms)
        {
            int seconds = execution_ms / 1000;
            seconds = Convert.ToInt32(Decimal.Round(seconds, 2));
            return seconds;
        }

        public async Task<ActionResult> Get_Files_Count(string[] task_ids)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            XDocument xdoc = XDocument.Load(currentDir + "\\Settings.xml");

            var sources_list = xdoc.Descendants("Source").ToList();

            var outputs = new List<FileOutput>();

            foreach (var item in task_ids)
            {
                var data = xdoc.Descendants("Source").Where(x => x.Attribute("rowID").Value == item).SingleOrDefault();
                outputs.Add(new FileOutput()
                {
                    destination = Convert.ToString(data.Attribute("destination").Value),
                    filename = Convert.ToString(data.Attribute("name").Value),
                    source = Convert.ToString(data.Attribute("path").Value),
                    rowID = Convert.ToInt32(data.Attribute("rowID").Value)
                });
            }

            List<DirectoryFile> files = new List<DirectoryFile>();
            foreach (var output in outputs)
            {
                files = global.GetFiles(output.source);
            }
            await Task.Delay(1000);
            return Json(new { count = files.Count }, JsonRequestBehavior.AllowGet);
        }

    }
}