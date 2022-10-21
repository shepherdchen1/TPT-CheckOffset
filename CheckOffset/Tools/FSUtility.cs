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
namespace TN.Tools
{
	using System;
	using System.IO;
	using System.Threading;

	public class FS
	{

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
		public static Boolean DeleteFile(string file)
		{
			Int32 retry_count = 5;

			if (!File.Exists(file))
				return true;

			while (true)
			{
				try
				{
					File.Delete(file);
					break;
				}
				catch (Exception)
				{
					if (retry_count == 0)
						return false;
					else
						retry_count--;
				}

				Thread.Sleep(10);
			}

			return true;
		}


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
		public static Boolean RenameFile(string srcFile, string destFile, Boolean overwrite)
		{
			try
			{
				if (System.IO.File.Exists(destFile))
				{
					if (overwrite)
						System.IO.File.Delete(destFile);
					else
						return false;
				}

				System.IO.File.Move(srcFile, destFile);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}


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
		public static Boolean CreateFileParentPath(string file)
		{
			try
			{
				FileInfo file_info = new FileInfo(file);
				if (null == file_info || null == file_info.DirectoryName)
					return false;

				if (!Directory.Exists(file_info.DirectoryName))
					Directory.CreateDirectory(file_info.DirectoryName);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}


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
		public static string? GetFileParentPath(string file)
		{
			FileInfo file_info = new FileInfo(file);

			return file_info.DirectoryName;
		}


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
		public static Boolean CopyFile(string srcFile, string destFile, Boolean overwrite)
		{
			try
			{
				if (!System.IO.File.Exists(srcFile))
					return false;

				if (System.IO.File.Exists(destFile))
				{
					if (overwrite)
						System.IO.File.Delete(destFile);
					else
						return false;
				}

				Int32 retry_count = 5;

				while (true)
				{
					try
					{
						System.IO.File.Copy(srcFile, destFile);
						break;
					}
					catch (Exception)
					{
						if (retry_count == 0)
							return false;
						else
							retry_count--;
					}

					Thread.Sleep(10);
				}
				
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}


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
		public static void CopyDirectory(string src_path, string dest_path)
		{
			if (!Directory.Exists(dest_path))
				Directory.CreateDirectory(dest_path);

			string[] directory_entries = Directory.GetFileSystemEntries(src_path);

			for (Int32 n = 0; n < directory_entries.Length; n++)
			{
				if (Directory.Exists(directory_entries[n]))
				{
					DirectoryInfo dir = new DirectoryInfo(directory_entries[n]);

					string dest_dir_name = dest_path + "\\" + dir.Name;

					if (!Directory.Exists(dest_dir_name))
						Directory.CreateDirectory(dest_dir_name);

					CopyDirectory(directory_entries[n], dest_dir_name);
				}

				if (File.Exists(directory_entries[n]))
				{
					string file_name = Path.GetFileName(directory_entries[n]);
					string dest_file_name = dest_path + "\\" + file_name;

					File.Copy(directory_entries[n], dest_file_name, true);
				}
			}
		}


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
		public static Boolean DeleteDirectory(string path)
		{
			if (!Directory.Exists(path))
				return true;

			Int32 retry_count = 5;

			while (true)
			{
				try
				{
					Directory.Delete(path, true);
					break;
				}
				catch (Exception)
				{
					if (retry_count == 0)
						return false;
					else
						retry_count --;
				}

				Thread.Sleep(20);
			}

			return true;
		}


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
		public static Boolean SafeMoveDirectory(string src_path, string dest_path)
		{
			try
			{
				CopyDirectory(src_path, dest_path);
			}
			catch (Exception)
			{
				return false;
			}

			return DeleteDirectory(src_path);
		}


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
		public static Boolean CreateEmptyDirectory(string path)
		{
			Boolean empty = true;

			if (Directory.Exists(path))
			{
				empty = DeleteDirectory(path);
			}

			Directory.CreateDirectory(path);

			return empty;
		}


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
		public static string[] GetDirectoriesNames(string path)
		{
			if (Directory.Exists(path))
			{
				string[] dir_name_array = Directory.GetDirectories(path);
				string[] name_array = new string[dir_name_array.Length];

				for (Int32 n = 0; n < dir_name_array.Length; n++)
				{
					DirectoryInfo info = new DirectoryInfo(dir_name_array[n]);
					name_array[n] = info.Name;
				}

				return name_array;
			}
			else
			{
				return new string[0];
			}
		}


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
		public static string[] GetDirectoriesNames(string path, string search_pattern)
		{
			if (Directory.Exists(path))
			{
				string[] dir_name_array = System.IO.Directory.GetDirectories(path, search_pattern);
				string[] name_array = new string[dir_name_array.Length];

				for (Int32 n = 0; n < dir_name_array.Length; n++)
				{
					DirectoryInfo info = new DirectoryInfo(dir_name_array[n]);
					name_array[n] = info.Name;
				}

				return name_array;
			}
			else
			{
				return new string[0];
			}
		}


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
		public static string[] GetFilesNames(string path, string search_pattern)
		{
			if (Directory.Exists(path))
			{
				string[] file_name_array = Directory.GetFiles(path, search_pattern);

				return file_name_array;
			}
			else
			{
				return new string[0];
			}
		}

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
		public static Boolean CheckFileName(string file_name)
		{
			if (file_name == null)
				return false;

			if (file_name == "")
				return false;

			Char[] ary_illegal_chars = System.IO.Path.GetInvalidFileNameChars();

			if (file_name.IndexOfAny(ary_illegal_chars) != -1)
				return false;

			return true;
		}




	} // class FS

} // namespace VS
