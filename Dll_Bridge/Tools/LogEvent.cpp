#include "..\\pch.h"
#include "LogEvent.h"
#include "Exception.h"
#include <utility>

using namespace TN::CPPTools::Debug;

#using <system.dll>						// System::Diagnostics::Trace
using namespace System::Diagnostics;

using namespace TN::CPPTools::Debug;

void Log_Utl::Init()
{
	PathLog = "d:\\Log\\Dll_Bridge\\";
	_event_log_locker = gcnew Object();
}

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
void Log_Utl::Log_Event(Event_Level event_lv, String ^title, String ^context)
{
	if (title == nullptr || context == nullptr)
		return;

	if (context == "")
		context = "No detail context.";
	else
		context->Replace("\r\n", "[NL]");

	System::DateTime date_now = System::DateTime::Now;

	// 建立資料夾
	String ^log_path = PathLog + String::Format( "\\{0:yyyyMMdd}\\Event", date_now );

	try
	{
		if (!Directory::Exists(log_path))
			Directory::CreateDirectory(log_path);
	}
	catch (Exception ^e)
	{
		// 儲存Exception到檔案
		ExceptionDump::SaveToFile(e, Get_Exception_DumpFile_Path());
	}

	// 建立檔案
	try
	{
		if (nullptr != _event_log_locker)
			Monitor::Enter(_event_log_locker);


		String ^event_file = String::Format("{0}\\Event_{1:yyyyMMdd_HH}.log", log_path, date_now );
		StreamWriter ^strea_writer = gcnew StreamWriter(event_file, true);
		strea_writer->Write(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title + "\r\n\t");

		String^ strEventLr = String::Format("{0}", static_cast<int>(event_lv)); // String::Format("{0}", event_lv);  //String::Format("{0}", event_lv);
		strea_writer->Write(strEventLr + ":" + context + "\r\n\r\n");
		strea_writer->Close();
	}
	catch (Exception ^e)
	{
		// 儲存Exception到檔案
		ExceptionDump::SaveToFile(e, Get_Exception_DumpFile_Path());
	}
	finally
	{
		if (nullptr != _event_log_locker)
			Monitor::Exit(_event_log_locker);
	}

	Trace::WriteLine(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title);
	String^ strEventLr_2 = String::Format("{0}", static_cast<int>(event_lv));
	Trace::WriteLine(strEventLr_2 + ":" + context);
}

void Log_Utl::Log_Step(String^ title, String^ context)
{
	if (!EnableLogStep)
		return;

	if (title == nullptr || context == nullptr)
		return;

	if (context == "")
		context = "No detail context.";
	else
		context->Replace("\r\n", "[NL]");

	System::DateTime date_now = System::DateTime::Now;

	// 建立資料夾
	String ^log_path = PathLog + String::Format("\\{0:yyyyMMdd}\\Step", date_now );

	try
	{
		if (!Directory::Exists(log_path))
			Directory::CreateDirectory(log_path);
	}
	catch (Exception ^e)
	{
		// 儲存Exception到檔案
		ExceptionDump::SaveToFile(e, Get_Exception_DumpFile_Path());
	}

	// 建立檔案
	try
	{
		if (nullptr != _event_log_locker)
			Monitor::Enter(_event_log_locker);


		String^ event_file = String::Format("{0}\\Step_{1:yyyyMMdd_HH}.log", log_path, date_now);
		StreamWriter ^strea_writer = gcnew StreamWriter(event_file, true);
		strea_writer->Write(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title + "\r\n\t");
		strea_writer->Write(context + "\r\n");
		strea_writer->Close();

		///VS					VS.FreeUtility.SafeFree(strea_writer);
	}
	catch (Exception ^e)
	{
		// 儲存Exception到檔案
		ExceptionDump::SaveToFile(e, Get_Exception_DumpFile_Path());
	}
	finally
	{
		if (nullptr != _event_log_locker)
			Monitor::Exit(_event_log_locker);
	}

	Trace::WriteLine(date_now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + title);
	Trace::WriteLine(context);
}

String^ Log_Utl::Get_Exception_DumpFile_Path()
{
	System::DateTime date_now = System::DateTime::Now;

	String^ log_path = PathLog + String::Format("\\{0:yyyyMMdd}", date_now);

	if (Directory::Exists(log_path) == false)
		Directory::CreateDirectory(log_path);

	return log_path + String::Format("\\ExcDump_{0:yyyyMMdd}.exd", date_now );
}

