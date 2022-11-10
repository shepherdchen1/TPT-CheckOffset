
#include "..\\pch.h"
#include "Exception.h"
#include "FSUtility.h"

using namespace TN;
using namespace TN::CPPTools;
using namespace TN::CPPTools::Debug;

String^ ExceptionDump::DateTimeFileName()
{
	return GetTimeStampFile(); 
}

String^ ExceptionDump::DefaultDumpPath()
{
	return _dump_path;
}

String^ ExceptionDump::DefaultDumpFile()
{
	return _dump_path + "\\" + GetDateStampFile();
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
void ExceptionDump::SaveToFile(Exception ^e, String ^context, String ^file)
{
	IO::StreamWriter ^stream = nullptr;

	try
	{
		Monitor::Enter(Log_File_Monitor);

		// 建立目標資料夾
		FS::CreateFileParentPath(file);

		// 附加模式開啟檔案
		stream = IO::File::AppendText( file );
		stream->Write( GetExceptionContext(e, context) );
	}
	finally
	{
		Monitor::Exit(Log_File_Monitor);
		if (nullptr != stream)
		{
			stream->Close();
			delete stream;
		}
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
void ExceptionDump::SaveToFile(Exception ^e, String ^file)
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
void ExceptionDump::SaveToDefaultFile(Exception ^e, String ^context)
{
	SaveToFile(e, context, _dump_path + "\\" + GetDateStampFile());
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
void ExceptionDump::SaveToDefaultFile(Exception ^e)
{
	SaveToFile(e, _dump_path + "\\" + GetDateStampFile());
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
String^ ExceptionDump::GetExceptionContext(Exception ^e, String ^context)
{
	if (e == nullptr)
		return "null";

	Thread ^current_thd = Thread::CurrentThread;

	String^ exception_context = String::Format(
		"Date:\r\n   {0:yyyy-MM-dd HH:mm:ss:fff}\r\n\r\n" +
		"Thread:\r\n   {1}, priority {2}\r\n\r\n" +
		"Source:\r\n   {3}\r\n\r\n" +
		"Message:\r\n   {4}\r\n\r\n" +
		"Context:\r\n   {5}\r\n\r\n" +					
		"StackTrace:\r\n{6}\r\n\r\n" +
		"##############################################################################\r\n\r\n",
		System::DateTime::Now,
		current_thd->Name == nullptr ? "Unknown thread" : current_thd->Name,
		current_thd->Priority.ToString(),
		e->Source,
		e->Message,
		context,					
		e->StackTrace);

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
String^ ExceptionDump::GetTimeStampFile()
{
	return String::Format( "ExcDump_{0:yyyyMMdd_HHmmss}.exd", System::DateTime::Now );
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
String^ ExceptionDump::GetDateStampFile()
{
	return String::Format( "ExcDump_{0:yyyyMMdd}.exd", System::DateTime::Now);
}