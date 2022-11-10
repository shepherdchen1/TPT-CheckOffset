#include "..\\pch.h"
#include "FSUtility.h"

using namespace TN;
using namespace TN::CPPTools;

Boolean FS::DeleteFile(String ^file)
{
	Int32 retry_count = 5;

	if (!File::Exists(file))
		return true;

	while (true)
	{
		try
		{
			File::Delete(file);
			break;
		}
		catch (Exception ^e)
		{
			if (retry_count == 0)
				return false;
			else
				retry_count--;
		}

		Thread::Sleep(10);
	}

	return true;
}

Boolean FS::RenameFile(String ^srcFile, String ^destFile, Boolean overwrite)
{
	try
	{
		if (File::Exists(destFile))
		{
			if (overwrite)
				File::Delete(destFile);
			else
				return false;
		}

		File::Move(srcFile, destFile);
	}
	catch (Exception ^e)
	{
		return false;
	}

	return true;
}

Boolean FS::CreateFileParentPath(String ^file)
{
	try
	{
		FileInfo ^file_info = gcnew FileInfo(file);
		if (nullptr == file_info || nullptr == file_info->DirectoryName)
			return false;

		if (!Directory::Exists(file_info->DirectoryName))
			Directory::CreateDirectory(file_info->DirectoryName);
	}
	catch (Exception ^e)
	{
		return false;
	}

	return true;
}

String^ FS::GetFileParentPath(String ^file)
{
	FileInfo ^file_info = gcnew FileInfo(file);

	return file_info->DirectoryName;
}

Boolean FS::CopyFile(String ^srcFile, String ^destFile, Boolean overwrite)
{
	try
	{
		if (!File::Exists(srcFile))
			return false;

		if (File::Exists(destFile))
		{
			if (overwrite)
				File::Delete(destFile);
			else
				return false;
		}

		Int32 retry_count = 5;

		while (true)
		{
			try
			{
				File::Copy(srcFile, destFile);
				break;
			}
			catch (Exception ^e)
			{
				if (retry_count == 0)
					return false;
				else
					retry_count--;
			}

			Thread::Sleep(10);
		}
				
	}
	catch (Exception ^e)
	{
		return false;
	}

	return true;
}


void FS::CopyDirectory(String ^src_path, String ^dest_path)
{
	if (!Directory::Exists(dest_path))
		Directory::CreateDirectory(dest_path);

	array<String^> ^directory_entries = Directory::GetFileSystemEntries(src_path);

	for (Int32 n = 0; n < directory_entries->Length; n++)
	{
		if (Directory::Exists(directory_entries[n]))
		{
			DirectoryInfo ^dir = gcnew DirectoryInfo(directory_entries[n]);

			String ^dest_dir_name = dest_path + "\\" + dir->Name;

			if (!Directory::Exists(dest_dir_name))
				Directory::CreateDirectory(dest_dir_name);

			CopyDirectory(directory_entries[n], dest_dir_name);
		}

		if (File::Exists(directory_entries[n]))
		{
			String ^file_name = Path::GetFileName(directory_entries[n]);
			String ^dest_file_name = dest_path + "\\" + file_name;

			File::Copy(directory_entries[n], dest_file_name, true);
		}
	}
}


Boolean FS::DeleteDirectory(String ^path)
{
	if (!Directory::Exists(path))
		return true;

	Int32 retry_count = 5;

	while (true)
	{
		try
		{
			Directory::Delete(path, true);
			break;
		}
		catch (Exception ^e)
		{
			if (retry_count == 0)
				return false;
			else
				retry_count --;
		}

		Thread::Sleep(20);
	}

	return true;
}


Boolean FS::SafeMoveDirectory(String ^src_path, String ^dest_path)
{
	try
	{
		CopyDirectory(src_path, dest_path);
	}
	catch (Exception ^e)
	{
		return false;
	}

	return DeleteDirectory(src_path);
}


Boolean FS::CreateEmptyDirectory(String ^path)
{
	Boolean empty = true;

	if (Directory::Exists(path))
	{
		empty = DeleteDirectory(path);
	}

	Directory::CreateDirectory(path);

	return empty;
}

array<String^>^ FS::GetDirectoriesNames(String ^path)
{
	if (Directory::Exists(path))
	{
		array<String^> ^dir_name_array = Directory::GetDirectories(path);
		array<String^> ^name_array = gcnew array<String^>(dir_name_array->Length);

		for (Int32 n = 0; n < dir_name_array->Length; n++)
		{
			DirectoryInfo ^info = gcnew DirectoryInfo(dir_name_array[n]);
			name_array[n] = info->Name;
		}

		return name_array;
	}
	else
	{
		return gcnew array<String^>(0);
	}
}

array<String^>^ FS::GetDirectoriesNames(String ^path, String ^search_pattern)
{
	if (Directory::Exists(path))
	{
		array<String^> ^dir_name_array = Directory::GetDirectories(path, search_pattern);
		array<String^> ^name_array = gcnew array<String^>(dir_name_array->Length);

		for (Int32 n = 0; n < dir_name_array->Length; n++)
		{
			DirectoryInfo ^info = gcnew DirectoryInfo(dir_name_array[n]);
			name_array[n] = info->Name;
		}

		return name_array;
	}
	else
	{
		return gcnew array<String^>(0);
	}
}


array<String^>^ FS::GetFilesNames(String ^path, String ^search_pattern)
{
	if (Directory::Exists(path))
	{
		array<String^> ^file_name_array = Directory::GetFiles(path, search_pattern);

		return file_name_array;
	}
	else
	{
		return gcnew array<String^>(0);
	}
}

Boolean FS::CheckFileName(String ^file_name)
{
	if (file_name == nullptr)
		return false;

	if (file_name == "")
		return false;

	array<Char> ^ary_illegal_chars = Path::GetInvalidFileNameChars();

	if (file_name->IndexOfAny(ary_illegal_chars) != -1)
		return false;

	return true;
}

