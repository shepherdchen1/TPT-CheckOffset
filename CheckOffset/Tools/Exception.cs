////=================================================================================================
//	File:		ExceptionDump.h
//
//	Created:	2020/11/6
//
//	Author:		
//
//	Purpose:	�NException���e�x�s���ɮ�
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
		// Purpose:		���o�H�ثe��������W�٪�ExceptionDump�ɮצW��
		// Access:		READ Only
		public static string DateFileName
		{
			get { return GetDateStampFile(); }
		}

	// Property:	TimeStampFile
	// Purpose:		���o�H�ثe�ɶ������W�٪�ExceptionDump�ɮצW��
	// Access:		READ Only
		public static string DateTimeFileName
		{
			get{ return GetTimeStampFile(); }
		}

		// Property:	DefaultDumpPath
		// Purpose:		�w�]��DumpPath
		// Access:		READ, WRITE
		public static string DefaultDumpPath
		{
			get{ return m_dump_path; }
			set{ m_dump_path = value; }
		}

		// Property:	DefaultDumpPath
		// Purpose:		�w�]��DumpPath
		// Access:		READ Only
		public static string DefaultDumpFile
		{
			get { return m_dump_path + "\\" + GetDateStampFile(); }		
		}

		//=================================================================================================
		// Function:	SaveToFile
		//
		// Purpose:		�NException���e�s���ɮפW, �p�ɮצs�b�h�H���[�覡�x�s
		//
		// Parameters:	Exception^ e		Exception
		//				string context		�B�~���x�s���
		//				string file		�x�s���ɮ�
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

				// �إߥؼи�Ƨ�
				FS.CreateFileParentPath(file);

				// ���[�Ҧ��}���ɮ�
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
		// Purpose:		�NException���e�s���ɮפW, �p�ɮצs�b�h�H���[�覡�x�s
		//
		// Parameters:	Exception e		Exception
		//				string file		�x�s���ɮ�
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
		// Purpose:		�NException���e�s��w�]���ɮפW, �p�ɮצs�b�h�H���[�覡�x�s
		//
		// Parameters:	Exception e		Exception
		//				string context		�B�~���x�s���
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
		// Purpose:		�NException���e�s��w�]���ɮפW, �p�ɮצs�b�h�H���[�覡�x�s
		//
		// Parameters:	Exception e		Exception
		//				string context		�B�~���x�s���
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
		// Purpose:		���oException����r��
		//
		// Parameters:	Exception e		Exception
		//				string context		�B�~���x�s���
		//
		// Return:		string				Exception �y�z�r��
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
			// Purpose:		���o�ɶ��W�O���ɮצW��
			//
			// Parameters:	void
			//
			// Return:		string				�ɮצW��
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
			// Purpose:		���o�ɶ��W�O���ɮצW��
			//
			// Parameters:	void
			//
			// Return:		string				�ɮצW��
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