
///VS
///
using System.Diagnostics;

namespace TN.Tools
{
	namespace Debug
	{
		using System;
		using System.IO;
		using System.Text;
		using System.Threading;

		public enum Event_Level
		{
			NotDefined
			, Normal
			, Warning
			, Error
		}

		// Class:		Log_Event
		// Purpose:		Log event
		public static class Log_Utl
		{
			//public static Log_Utl()
			//         {
			//	PathLog = "d:\\Log";
			//	_event_log_locker = new object();
			//}

			public static void Init()
            {
                PathLog = "d:\\Log";
                _event_log_locker = new object();
            }

            public static string? PathLog
            { get; set; }

			private static object? _event_log_locker;

			public static bool EnableLogStep;

			//=================================================================================================
			// Function:	Log_Event
			//
			// Purpose:		將事件寫到預設之記錄檔上
			//
			// Parameters:	Event_Level event_lv		事件類型
			//				string title			事件標題(簡要內容)
			//				string context			事件詳細內容
			//
			// Return:		void
			//
			// History:		1.	2020/11/6	Created
			//=================================================================================================
			public static void Log_Event(Event_Level event_lv, string? title, string context)
			{
				if (title == null || context == null)
					return;

				if (context == "")
					context = "No detail context.";
				else
					context.Replace("\r\n", "[NL]");

				System.DateTime date_now = System.DateTime.Now;

				// 建立資料夾
				string log_path = PathLog + string.Format("\\{0:yyyyMMdd}\\Event", date_now);

				try
				{
					if (!Directory.Exists(log_path))
						Directory.CreateDirectory(log_path);
				}
				catch (Exception e)
				{
					// 儲存Exception到檔案
					TN.Tools.Debug.ExceptionDump.SaveToFile(e, Get_Exception_DumpFile_Path());
				}

				// 建立檔案
				try
				{
					if (null != _event_log_locker )
						Monitor.Enter(_event_log_locker);


					string event_file = string.Format("{0}\\Event_{1:yyyyMMdd_HH}.log", log_path, date_now);
					StreamWriter strea_writer = new StreamWriter(event_file, true);					
					strea_writer.Write(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title + "\r\n\t");

					string strEventLr = string.Format("{0}:", event_lv);
					strea_writer.Write(strEventLr + ":" + context + "\r\n\r\n");
					strea_writer.Close();
				}
				catch (Exception e)
				{
					// 儲存Exception到檔案
					TN.Tools.Debug.ExceptionDump.SaveToFile(e, Get_Exception_DumpFile_Path());
				}
				finally
				{
                    if (null != _event_log_locker)
                        Monitor.Exit(_event_log_locker);
				}

				Trace.WriteLine(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title);
				string strEventLr_2 = string.Format("{0}:", event_lv);
				Trace.WriteLine(strEventLr_2 + ":" + context);
			}

			public static void Log_Step(string title, string context)
			{
				if (!EnableLogStep)
					return;

				if (title == null || context == null)
					return;

				if (context == "")
					context = "No detail context.";
				else
					context.Replace("\r\n", "[NL]");

				System.DateTime date_now = System.DateTime.Now;

				// 建立資料夾
				string log_path = PathLog + string.Format("\\{0:yyyyMMdd}\\Step", date_now);

				try
				{
					if (!Directory.Exists(log_path))
						Directory.CreateDirectory(log_path);
				}
				catch (Exception e)
				{
					// 儲存Exception到檔案
					TN.Tools.Debug.ExceptionDump.SaveToFile(e, Get_Exception_DumpFile_Path());
				}

				// 建立檔案
				try
				{
                    if (null != _event_log_locker)
                        Monitor.Enter(_event_log_locker);


					string event_file = string.Format("{0}\\Step_{1:yyyyMMdd_HH}.log", log_path, date_now);
					StreamWriter strea_writer = new StreamWriter(event_file, true);
					strea_writer.Write(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title + "\r\n\t");
					strea_writer.Write(context + "\r\n");
					strea_writer.Close();

///VS					VS.FreeUtility.SafeFree(strea_writer);
				}
				catch (Exception e)
				{
					// 儲存Exception到檔案
					TN.Tools.Debug.ExceptionDump.SaveToFile(e, Get_Exception_DumpFile_Path());
				}
				finally
				{
                    if (null != _event_log_locker)
                        Monitor.Exit(_event_log_locker);
				}

				Trace.WriteLine(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title);
				Trace.WriteLine(context);
			}

			private static string Get_Exception_DumpFile_Path()
			{
				System.DateTime date_now = System.DateTime.Now;

				string log_path = PathLog + string.Format("\\{0:yyyyMMdd}", date_now);

				if (Directory.Exists(log_path) == false)
					Directory.CreateDirectory(log_path);

				return log_path + string.Format("\\ExcDump_{0:yyyyMMdd}.exd", date_now);
			}
		}
	}
}

