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
					// Purpose:		���o�H�ثe�ɶ������W�٪�ExceptionDump�ɮצW��
					// Access:		READ Only
				static String^ DateTimeFileName();
				//{
				//	get{ return GetTimeStampFile(); }
				//};

				// Property:	DefaultDumpPath
				// Purpose:		�w�]��DumpPath
				// Access:		READ, WRITE
				static String^ DefaultDumpPath();


				// Property:	DefaultDumpPath
				// Purpose:		�w�]��DumpPath
				// Access:		READ Only
				static String^ DefaultDumpFile();

				static void SaveToFile(Exception ^e, String ^context, String ^file);
			
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
				static void SaveToFile(Exception ^e, String ^file);

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
				static void SaveToDefaultFile(Exception ^e, String ^context);


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
				static void SaveToDefaultFile(Exception ^e);

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
			private:
				static String^ GetExceptionContext(Exception ^e, String ^context);
				
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
				static String^ GetTimeStampFile();



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
				static String^ GetDateStampFile();
			};
		};
	};
};