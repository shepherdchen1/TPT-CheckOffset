#pragma once

namespace TN
{
	namespace CPPTools
	{
		namespace Debug
		{
			using namespace System;
			using namespace System::IO;
			using namespace System::Text;
			using namespace System::Threading;


			// Class:		ExceptionDump
			// Purpose:		Exception Dump
			public ref class ExceptionDump
			{
			private:
				static Object ^Log_File_Monitor = gcnew Object();
				static String ^_dump_path = "D:\\Log";

			public:
				static String ^DateFileName;

					// Property:	TimeStampFile
					// Purpose:		取得以目前時間做為名稱的ExceptionDump檔案名稱
					// Access:		READ Only
				static String^ DateTimeFileName();
				//{
				//	get{ return GetTimeStampFile(); }
				//};

				// Property:	DefaultDumpPath
				// Purpose:		預設的DumpPath
				// Access:		READ, WRITE
				static String^ DefaultDumpPath();


				// Property:	DefaultDumpPath
				// Purpose:		預設的DumpPath
				// Access:		READ Only
				static String^ DefaultDumpFile();

				static void SaveToFile(Exception ^e, String ^context, String ^file);
			
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
				static void SaveToFile(Exception ^e, String ^file);

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
				static void SaveToDefaultFile(Exception ^e, String ^context);


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
				static void SaveToDefaultFile(Exception ^e);

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
			private:
				static String^ GetExceptionContext(Exception ^e, String ^context);
				
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
				static String^ GetTimeStampFile();



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
				static String^ GetDateStampFile();
			};
		};
	};
};