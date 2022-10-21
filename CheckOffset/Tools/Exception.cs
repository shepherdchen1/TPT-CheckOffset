////=================================================================================================
//	File:		ExceptionDump.h
//
//	Created:	2020/11/6
//
//	Author:		
//
//	Purpose:	將Exception內容儲存到檔案
//
//	Platform:	DotNet C++/CLR pure (.Net framework 2.0 or above)
//
//	Note: 		
//
//	Abbreviation:	
//
//	History:	1.	2020/11/6, created 
//=================================================================================================

namespace TN.Tools
{
	namespace Debug
	{
		using System;
		using System.Text;
		using System.Threading;


		// Class:		ExceptionDump
		// Purpose:		Exception Dump
		public class ExceptionDump
		{
			private static object Log_File_Monitor = new object();
			private static string m_dump_path = "D:\\Log";
		
		// Property:	TimeStampFile
		// Purpose:		取得以目前日期做為名稱的ExceptionDump檔案名稱
		// Access:		READ Only
		public static string DateFileName
		{
			get { return GetDateStampFile(); }
		}

	// Property:	TimeStampFile
	// Purpose:		取得以目前時間做為名稱的ExceptionDump檔案名稱
	// Access:		READ Only
		public static string DateTimeFileName
		{
			get{ return GetTimeStampFile(); }
		}

		// Property:	DefaultDumpPath
		// Purpose:		預設的DumpPath
		// Access:		READ, WRITE
		public static string DefaultDumpPath
		{
			get{ return m_dump_path; }
			set{ m_dump_path = value; }
		}

		// Property:	DefaultDumpPath
		// Purpose:		預設的DumpPath
		// Access:		READ Only
		public static string DefaultDumpFile
		{
			get { return m_dump_path + "\\" + GetDateStampFile(); }		
		}

		//=================================================================================================
		// Function:	SaveToFile
		//
		// Purpose:		將Exception內容存到檔案上, 如檔案存在則以附加方式儲存
		//
		// Parameters:	Exception^ e		Exception
		//				string context		額外的儲存資料
		//				string file		儲存之檔案
		//
		// Return:		void
		//
		//	History:	1.	2020/11/6, created 
		//=================================================================================================
		public static void SaveToFile(Exception e, string context, string file)
		{
			System.IO.StreamWriter? stream = null;

			try
			{
				Monitor.Enter(Log_File_Monitor);

				// 建立目標資料夾
				FS.CreateFileParentPath(file);

				// 附加模式開啟檔案
				stream = System.IO.File.AppendText( file );
				stream.Write( GetExceptionContext(e, context) );
			}
			finally
			{
				Monitor.Exit(Log_File_Monitor);
				if (null != stream)
					stream.Close();
			}
		}


		//=================================================================================================
		// Function:	SaveToFile
		//
		// Purpose:		將Exception內容存到檔案上, 如檔案存在則以附加方式儲存
		//
		// Parameters:	Exception e		Exception
		//				string file		儲存之檔案
		//
		// Return:		void
		//
		//	History:	1.	2020/11/6, created 
		//=================================================================================================
		public static void SaveToFile(Exception e, string file)
		{
			SaveToFile(e, "None", file);
		}

		//=================================================================================================
		// Function:	SaveToDefaultFile
		//
		// Purpose:		將Exception內容存到預設之檔案上, 如檔案存在則以附加方式儲存
		//
		// Parameters:	Exception e		Exception
		//				string context		額外的儲存資料
		//
		// Return:		void
		//
		//	History:	1.	2020/11/6, created 
		//=================================================================================================
		public static void SaveToDefaultFile(Exception e, string context)
		{
			SaveToFile(e, context, m_dump_path + "\\" + GetDateStampFile());
		}


		//=================================================================================================
		// Function:	SaveToFile
		//
		// Purpose:		將Exception內容存到預設之檔案上, 如檔案存在則以附加方式儲存
		//
		// Parameters:	Exception e		Exception
		//				string context		額外的儲存資料
		//
		// Return:		void
		//
		//	History:	1.	2020/11/6, created 
		//=================================================================================================
		public static void SaveToDefaultFile(Exception e)
		{
			SaveToFile(e, m_dump_path + "\\" + GetDateStampFile());
		}

		//=================================================================================================
		// Function:	GetExceptionContext
		//
		// Purpose:		取得Exception完整字串
		//
		// Parameters:	Exception e		Exception
		//				string context		額外的儲存資料
		//
		// Return:		string				Exception 描述字串
		//
		//	History:	1.	2020/11/6, created 
		//=================================================================================================
		private static string GetExceptionContext(Exception e, string context)
		{
			if (e == null)
				return "null";

			Thread current_thd = Thread.CurrentThread;

			string exception_context = string.Format(
				"Date:\r\n   {0:yyyy-MM-dd HH:mm:ss:fff}\r\n\r\n" +
				"Thread:\r\n   {1}, priority {2}\r\n\r\n" +
				"Source:\r\n   {3}\r\n\r\n" +
				"Message:\r\n   {4}\r\n\r\n" +
				"Context:\r\n   {5}\r\n\r\n" +					
				"StackTrace:\r\n{6}\r\n\r\n" +
				"##############################################################################\r\n\r\n",
				System.DateTime.Now,
				current_thd.Name == null ? "Unknown thread" : current_thd.Name,
				current_thd.Priority.ToString(),
				e.Source,
				e.Message,
				context,					
				e.StackTrace);

			return exception_context;
		}


			//=================================================================================================
			// Function:	GetTimeStampFile
			//
			// Purpose:		取得時間戳記的檔案名稱
			//
			// Parameters:	void
			//
			// Return:		string				檔案名稱
			//
			//	History:	1.	2020/11/6, created 
			//=================================================================================================
			static string GetTimeStampFile()
			{
				return string.Format("ExcDump_{0:yyyyMMdd_HHmmss}.exd", System.DateTime.Now);
			}


			//=================================================================================================
			// Function:	GetDateStampFile
			//
			// Purpose:		取得時間戳記的檔案名稱
			//
			// Parameters:	void
			//
			// Return:		string				檔案名稱
			//
			//	History:	1.	2020/11/6, created 
			//=================================================================================================
			static string GetDateStampFile()
			{
				return string.Format("ExcDump_{0:yyyyMMdd}.exd", System.DateTime.Now);
			}

		};


	}

}