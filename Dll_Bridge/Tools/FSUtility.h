#pragma once

//=================================================================================================
//	File: mvFSUtility.h
//
//	Created: 2020/11/9
//
//	Author: 
//
//	Purpose: 提供更進一步安全的FS操作
//
//	Platform: .NET C++ /CLR
//
//	Note:
//
//	Abbreviation:	
//
//	History:	1.	2020/11/9, created
//=================================================================================================
namespace TN
{
	namespace CPPTools
	{
		using namespace System;
		using namespace System::IO;
		using namespace System::Threading;

		public class FS
		{
		public:
			//=================================================================================================
			// Function:	DeleteFile
			//
			// Purpose:		刪除檔案
			//
			// Parameters:	string file	檔名
			//
			// Return:		Boolean			是否成功
			//
			// History:		1.	2020/11/9 	Created
			//=================================================================================================
			static Boolean DeleteFile(String^ file);

			//=================================================================================================
			// Function:	RenameFile
			//
			// Purpose:		重新命名檔案
			//
			// Parameters:	String^ srcFile			原始檔名
			//				String^ destFile		重新命名的檔名
			//				Boolean	overwrite		是否覆寫檔案
			//
			// Return:		Boolean					是否成功
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean RenameFile(String^ srcFile, String^ destFile, Boolean overwrite);

			//=================================================================================================
			// Function:	CreateFileParentPath
			//
			// Purpose:		建立檔案的所屬資料夾
			//
			// Parameters:	String^ file			檔案名稱(完整路徑)
			//
			// Return:		Boolean					是否成功
			//
			// History:		1.	2013/11/26	Created by Frank Lu
			//=================================================================================================
			static Boolean CreateFileParentPath(String^ file);


			//=================================================================================================
			// Function:	CreateFileParentPath
			//
			// Purpose:		建立檔案的所屬資料夾
			//
			// Parameters:	String^ file			檔案名稱(完整路徑)
			//
			// Return:		String^					檔案所在的資料夾
			//
			// History:		1.	2013/11/26	Created by Frank Lu
			//=================================================================================================
			static String^ GetFileParentPath(String^ file);


			//=================================================================================================
			// Function:	CopyFile
			//
			// Purpose:		複製檔案
			//
			// Parameters:	String^ srcFile			原始檔名
			//				String^ destFile		重新命名的檔名
			//				Boolean	overwrite		是否覆寫檔案
			//
			// Return:		Boolean					是否成功
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean CopyFile(String^ srcFile, String^ destFile, Boolean overwrite);


			//=================================================================================================
			// Function:	CopyDirectory
			//
			// Purpose:		複製檔案
			//
			// Parameters:	String^ srcPath			來源path
			//				String^ destPath		目標path
			//
			// Return:		void
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static void CopyDirectory(String^ src_path, String^ dest_path);


			//=================================================================================================
			// Function:	DeleteDirectory
			//
			// Purpose:		嘗試刪除資料夾及內含的檔案目錄
			//
			// Parameters:	String^ path	目標DIR
			//
			// Return:		Boolean			是否成功
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean DeleteDirectory(String^ path);


			//=================================================================================================
			// Function:	SafeMoveDirectory
			//
			// Purpose:		先Copy再刪除來源檔案,避免Move失敗時檔案遺失
			//
			// Parameters:	String^ srcPath			來源path
			//				String^ destPath		目標path
			//
			// Return:		Boolean					是否成功
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean SafeMoveDirectory(String^ src_path, String^ dest_path);


			//=================================================================================================
			// Function:	CreateEmptyDirectory
			//
			// Purpose:		建立資料夾,如果已存在則刪除它後重新建立
			//
			// Parameters:	String^ path	目標DIR
			//
			// Return:		Boolean			是否成功
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean CreateEmptyDirectory(String^ path);

			//=================================================================================================
			// Function:	GetDirectoriesNames
			//
			// Purpose:		取得資料夾內子目錄名稱
			//
			// Parameters:	String^ path	目標DIR
			//
			// Return:		Boolean			是否成功
			//
			// History:		1.	2011/09/07 	Created by Tony Tsao
			//=================================================================================================
			static array<String^>^ GetDirectoriesNames(String^ path);

			//=================================================================================================
			// Function:	GetDirectoriesNames
			//
			// Purpose:		取得資料夾內符合pattern的子目錄名稱
			//
			// Parameters:	String^ path				目標資料夾
			//				String^ search_pattern		搜尋Pattern
			//
			// Return:		Boolean						是否成功
			//
			// History:		1.	2012/06/04 	Created by Frank Lu
			//=================================================================================================
			static array<String^>^ GetDirectoriesNames(String^ path, String^ search_pattern);


			//=================================================================================================
			// Function:	GetFilesNames
			//
			// Purpose:		取得資料夾內符合pattern的子目錄名稱
			//
			// Parameters:	String^ path				目標資料夾
			//				String^ search_pattern		搜尋Pattern
			//
			// Return:		Boolean						是否成功
			//
			// History:		1.	2012/06/04 	Created by Frank Lu
			//=================================================================================================
			static array<String^>^ GetFilesNames(String^ path, String^ search_pattern);

			//=================================================================================================
			// Function:	CheckFileName
			//
			// Purpose:		確認檔案名稱是否合法
			//
			// Parameters:	String^ file_name		檔案名稱
			//
			// Return:		Boolean					是否合法
			//
			// History:		1.	2012/06/04 	Created by Frank Lu
			//=================================================================================================
			static Boolean CheckFileName(String^ file_name);

		}; // end of class FS

	}; // end namespace CPPTools
} // end of namespace TN
