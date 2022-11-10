#pragma once

namespace TN
{
	namespace CPPTools
	{
		namespace Debug
		{
//#include <string>
//			using namespace std;
//#include <mutex>
			using namespace System;
			using namespace System::IO;
			//using System.Text;
			using namespace System::Threading;


			public enum Event_Level
			{
				NotDefined
				, Normal
				, Warning
				, Error
			};

			// Class:		Log_Event
			// Purpose:		Log event
			public ref class Log_Utl
			{
				//public static Log_Utl()
				//         {
				//	PathLog = "d:\\Log";
				//	_event_log_locker = new object();
				//}

			public:
				static void Init();

				static String^ PathLog;
				//{ get; set; }

			private:
				static Object^ _event_log_locker;

				static String^ Get_Exception_DumpFile_Path();
			public:
				static bool EnableLogStep;

				static void Log_Event(Event_Level event_lv, String^ title, String^ context);

				static void Log_Step(String^ title, String^ context);
			};
		}
	}
}

