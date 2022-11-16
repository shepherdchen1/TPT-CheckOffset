using CheckOffset;
using CheckOffset.ImageTools;
using CheckOffset.ProjectInspInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TN.Insp_Param;
using TN.Tools;
using TN.Tools.Debug;

//namespace CheckOffset
//{
//    internal class tnGlobal
//    {
//    }
//}


//using System.Linq;
//using System.Web;
//using System.Data;
//using System.Windows.Forms;
//using System.IO;

//using System.ComponentModel; // GetDescription  attribute
//using System.Reflection;     // GetDescription  attribute
//using System.Threading;         // monitor.
//using System.Threading.Tasks;   // monitor.


namespace CheckOffset
{
    //public enum Priority_Inv
    //{
    //    Priority_Performance    // 效率優先
    //    , Priority_Save_Space   // 貨位優先(節省空間)
    //}



    public static class tnGlobal
    {
        public static T ToObject<T>(this DataRow dataRow)
             where T : new()
        {
            T item = new T();
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                if (dataRow[column] == DBNull.Value)
                    continue;

                PropertyInfo? prop = item.GetType().GetProperty(column.ColumnName);
                if (prop != null)
                {
                    object result = Convert.ChangeType(dataRow[column], prop.PropertyType);
                    prop.SetValue(item, result, null);
                    continue;
                }
                else
                {
                    FieldInfo? fld = item.GetType().GetField(column.ColumnName);
                    if (fld != null)
                    {
                        object result = Convert.ChangeType(dataRow[column], fld.FieldType);
                        fld.SetValue(item, result);
                    }
                }
            }
            return item;
        }

        public static T Cast<T>(this DataRow dataRow) where T : new()
        {
            T item = new T();

            IEnumerable<PropertyInfo> properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                                 .Where(x => x.CanWrite);

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                if (dataRow[column] == DBNull.Value)
                {
                    continue;
                }

                PropertyInfo? property = properties.FirstOrDefault(x => column.ColumnName.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

                if (property == null)
                {
                    continue;
                }

                try
                {
                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    object? safeValue = (dataRow[column] == null) ? null : Convert.ChangeType(dataRow[column], t);

                    property.SetValue(item, safeValue, null);
                }
                catch
                {
                    throw new Exception($"The value '{dataRow[column]}' cannot be mapped to the property '{property.Name}'!");
                }

            }

            return item;
        }

        public static DS_Setting_Info Setting = new DS_Setting_Info();

        public static DS_Insp_Param Insp_Param = new DS_Insp_Param();

        public static List<DS_Detect_Pin_Info>? Detect_Infos = new List<DS_Detect_Pin_Info>();

        public static IT_Detect _IT_Detect = null;

        //public static DS_Defect_Pin_Info? Detect_Pins = null;

        // change to public for update progress
        public static BackgroundWorker BKWorker_Delete_Log = new BackgroundWorker();


        public static void Initialize()
        {
            Log_Utl.Init();

            Log_Utl.EnableLogStep = Setting.EnableLogStep;

            //if (null != Program.For_Main)
            //    Program.For_Main.Message = "initialize....";

            //Shelf_Mgr.Update_INV3_QTY(MySql);
            //if (null != Program.For_Main)
            //    Program.For_Main.Message = "initialize done.";


            ///////////////////////////////////////////////////
            // initial BKWorker

            ///////////////////////////////////////////////////
            // initial BKWorker
            BKWorker_Delete_Log.DoWork +=
                new DoWorkEventHandler(BKWorker_Delete_Log_DoWork);

            BKWorker_Delete_Log.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
                                        BKWorker_Delete_Log_RunWorkerCompleted);

            BKWorker_Delete_Log.WorkerSupportsCancellation = true;

            BKWorker_Delete_Log.RunWorkerAsync();

        }

        public static void Uninitialize()
        {
            BKWorker_Delete_Log.CancelAsync();
        }

        public static string? GetDescription<T>(T value)
        {
            if (null == value)
                return "";

            var type = value.GetType();
            FieldInfo? fi = value.GetType().GetField( value.ToString() ?? "");
            if (null == fi)
                return "";

            DescriptionAttribute[]? attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }


        //////
        /// delete log
        private static void BKWorker_Delete_Log_DoWork(object? sender,
            DoWorkEventArgs e)
        {
            Log_Utl.Log_Event(Event_Level.Normal, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                , $"BKWorker_Delete_Log_DoWork start");

            // Get the BackgroundWorker that raised this event.
            BackgroundWorker? worker = sender as BackgroundWorker;
            if (worker == null)
                return;

            try
            {
                while (!worker.CancellationPending)
                {
                    try
                    {
                        // delete d:\log\date_ path.
                        Delete_Log_File(Log_Utl.PathLog);

                        // delete d:\log\date_ path.
                        Delete_Log_Dir(Log_Utl.PathLog);
                    }
                    catch (Exception ex_dir)
                    {
                        // 儲存Exception到檔案
                        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex_dir);
                    }

                    // 一天刪一次Log
                    System.Threading.Thread.Sleep(86400 * 1000);
                    //                    string log_path = Log_Utl.PathLog + $"\\{0:yyyyMMdd}\\Step", date_now);
                }

                Log_Utl.Log_Event(Event_Level.Normal, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                   , $"BKWorker_Delete_Log_DoWork quit");

            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
            }
        }

        private static bool Delete_Log_Dir(string? PathLog)
        {
            try
            {
                if ( null == PathLog )
                {
                    return false;
                }

                // delete d:\log\date_ path.
                if (PathLog.Length <= 4)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                       , $"Prevent from delete root folder: PathLog:{PathLog}");
                    return false;
                }

                List<string> dirs = new List<string>(Directory.EnumerateDirectories(PathLog));
                DateTime valid_date = DateTime.Now;
                valid_date = valid_date.AddDays(-30);
                foreach (string dir in dirs)
                {
                    DirectoryInfo dir_info = new DirectoryInfo(dir);
                    if (dir_info.Name.Length < 8)
                        continue;

                    int.TryParse(dir_info.Name.Substring(0, 4), out int year);
                    int.TryParse(dir_info.Name.Substring(4, 2), out int month);
                    int.TryParse(dir_info.Name.Substring(6, 2), out int date);

                    if (0 == year || 0 == month || 0 == date)
                        continue;

                    DateTime dir_date = new DateTime(year, month, date);

                    if (dir_date < valid_date)
                    {
                        if (dir.Length <= 6)
                            continue;

                        Directory.Delete(dir, true);
                    }
                }

                if (null == Log_Utl.PathLog)
                    return false;

                // delete d:\log\date_ path.
                List<string> files = new List<string>(Directory.EnumerateFiles(Log_Utl.PathLog));
                foreach (string file in files)
                {
                    FileInfo file_info = new FileInfo(file);
                    if (file_info.Name.Length < 16)
                        continue;

                    // ExcDump_20201126
                    int.TryParse(file_info.Name.Substring(8, 4), out int year);
                    int.TryParse(file_info.Name.Substring(12, 2), out int month);
                    int.TryParse(file_info.Name.Substring(14, 2), out int date);

                    if (0 == year || 0 == month || 0 == date)
                        continue;

                    DateTime dir_date = new DateTime(year, month, date);

                    if (dir_date < valid_date)
                    {
                        if (file.Length <= 6)
                            continue;

                        File.Delete(file);
                    }
                }

                Log_Utl.Log_Event(Event_Level.Normal, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                   , $"BKWorker_Delete_Log_DoWork quit");
                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
            }

            return false;
        }

        private static bool Delete_Log_File(string? PathLog)
        {
            try
            {
                if (null == PathLog)
                    return false;

                if (PathLog.Length <= 4)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                       , $"Prevent from delete root folder: PathLog:{PathLog}");
                    return false;
                }

                List<string> files = new List<string>(Directory.EnumerateFiles(PathLog));
                DateTime valid_date = DateTime.Now;
                valid_date = valid_date.AddDays(-30);
                foreach (string file in files)
                {
                    FileInfo file_info = new FileInfo(file);
                    if (file_info.Name.Length < 16)
                        continue;

                    // ExcDump_20201126
                    int.TryParse(file_info.Name.Substring(8, 4), out int year);
                    int.TryParse(file_info.Name.Substring(12, 2), out int month);
                    int.TryParse(file_info.Name.Substring(14, 2), out int date);

                    if (0 == year || 0 == month || 0 == date)
                        continue;

                    DateTime dir_date = new DateTime(year, month, date);

                    if (dir_date < valid_date)
                    {
                        if (file.Length <= 6)
                            continue;

                        File.Delete(file);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
            }

            return false;
        }

        private static void BKWorker_Delete_Log_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //            this.progressBar1.Value = e.ProgressPercentage;

            // Get the BackgroundWorker that raised this event.
            BackgroundWorker? worker = sender as BackgroundWorker;
            if (null == worker)
                return;

            try
            {
                //if (null != Program.For_Main)
                //    Program.For_Main.Message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + (string)e.UserState;

                //Log_Utl.Log_Event(Event_Level.Normal, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                //                                   , $"BKWorker_Job quit"));

            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
            }
        }

        // This event handler deals with the results of the
        // background operation.
        private static void BKWorker_Delete_Log_RunWorkerCompleted(
            object? sender, RunWorkerCompletedEventArgs e)
        {
            Log_Utl.Log_Event(Event_Level.Normal, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                , $"BKWorker_DoWork End");

            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                //                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
            }
            else
            {
            }
        }

    } // end of     public static class tnGlobal

    /// <summary>
    /// 環境設定
    /// </summary>
    public class DS_Setting_Info
    {
        public bool EnableLogStep = true;              // 是否輸出Log\Step.

        public DS_Setting_Info()
        {
            EnableLogStep = true;
        }
    }

    public class DS_Insp_Param
    {
        //public Rectangle Detect_Rect = new Rectangle(0, 0, 0, 0);              // 是否輸出Log\Step.

        public DS_Insp_Param_Pin Insp_Param_Pin = new DS_Insp_Param_Pin();

        //private int _Min_Pin_WH = 10;

        //private EN_Insp_Tol_Dir _Insp_Tol_Dir = EN_Insp_Tol_Dir.EN_Insp_Tol_None;

        //public DS_Insp_Result Detect_Insp_Result = new DS_Insp_Result();

        public DS_Insp_Param()
        {
             Insp_Param_Pin = new DS_Insp_Param_Pin();
            //Detect_Rect = new Rectangle(0, 0, 0, 0);
            //Detect_Insp_param = new DS_Insp_Param_Pin();
            //Detect_Insp_Result = new DS_Insp_Result();
        }
    }

    public class DS_Detect_Pin_Info
    {
        private EN_Insp_Tol_Dir _Insp_Tol_Dir = EN_Insp_Tol_Dir.EN_Insp_Tol_None;


        public Rectangle Detect_Rect = new Rectangle(0, 0, 0, 0);              // 是否輸出Log\Step.

        public DS_Insp_Result Detect_Insp_Result = new DS_Insp_Result();

        public EN_Insp_Tol_Dir Insp_Tol_Dir { get => _Insp_Tol_Dir; set => _Insp_Tol_Dir = value; }

        public DS_Detect_Pin_Info()
        {
            Detect_Rect = new Rectangle(0, 0, 0, 0);
            _Insp_Tol_Dir = EN_Insp_Tol_Dir.EN_Insp_Tol_None;
            Detect_Insp_Result = new DS_Insp_Result();
        }
    }
} // end of namespace CheckOffset
