#pragma once

//=================================================================================================
//	File: mvFSUtility.h
//
//	Created: 2020/11/9
//
//	Author: 
//
//	Purpose: ���ѧ�i�@�B�w����FS�ާ@
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
			// Purpose:		�R���ɮ�
			//
			// Parameters:	string file	�ɦW
			//
			// Return:		Boolean			�O�_���\
			//
			// History:		1.	2020/11/9 	Created
			//=================================================================================================
			static Boolean DeleteFile(String^ file);

			//=================================================================================================
			// Function:	RenameFile
			//
			// Purpose:		���s�R�W�ɮ�
			//
			// Parameters:	String^ srcFile			��l�ɦW
			//				String^ destFile		���s�R�W���ɦW
			//				Boolean	overwrite		�O�_�мg�ɮ�
			//
			// Return:		Boolean					�O�_���\
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean RenameFile(String^ srcFile, String^ destFile, Boolean overwrite);

			//=================================================================================================
			// Function:	CreateFileParentPath
			//
			// Purpose:		�إ��ɮת����ݸ�Ƨ�
			//
			// Parameters:	String^ file			�ɮצW��(������|)
			//
			// Return:		Boolean					�O�_���\
			//
			// History:		1.	2013/11/26	Created by Frank Lu
			//=================================================================================================
			static Boolean CreateFileParentPath(String^ file);


			//=================================================================================================
			// Function:	CreateFileParentPath
			//
			// Purpose:		�إ��ɮת����ݸ�Ƨ�
			//
			// Parameters:	String^ file			�ɮצW��(������|)
			//
			// Return:		String^					�ɮשҦb����Ƨ�
			//
			// History:		1.	2013/11/26	Created by Frank Lu
			//=================================================================================================
			static String^ GetFileParentPath(String^ file);


			//=================================================================================================
			// Function:	CopyFile
			//
			// Purpose:		�ƻs�ɮ�
			//
			// Parameters:	String^ srcFile			��l�ɦW
			//				String^ destFile		���s�R�W���ɦW
			//				Boolean	overwrite		�O�_�мg�ɮ�
			//
			// Return:		Boolean					�O�_���\
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean CopyFile(String^ srcFile, String^ destFile, Boolean overwrite);


			//=================================================================================================
			// Function:	CopyDirectory
			//
			// Purpose:		�ƻs�ɮ�
			//
			// Parameters:	String^ srcPath			�ӷ�path
			//				String^ destPath		�ؼ�path
			//
			// Return:		void
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static void CopyDirectory(String^ src_path, String^ dest_path);


			//=================================================================================================
			// Function:	DeleteDirectory
			//
			// Purpose:		���էR����Ƨ��Τ��t���ɮץؿ�
			//
			// Parameters:	String^ path	�ؼ�DIR
			//
			// Return:		Boolean			�O�_���\
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean DeleteDirectory(String^ path);


			//=================================================================================================
			// Function:	SafeMoveDirectory
			//
			// Purpose:		��Copy�A�R���ӷ��ɮ�,�קKMove���Ѯ��ɮ׿�
			//
			// Parameters:	String^ srcPath			�ӷ�path
			//				String^ destPath		�ؼ�path
			//
			// Return:		Boolean					�O�_���\
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean SafeMoveDirectory(String^ src_path, String^ dest_path);


			//=================================================================================================
			// Function:	CreateEmptyDirectory
			//
			// Purpose:		�إ߸�Ƨ�,�p�G�w�s�b�h�R�����᭫�s�إ�
			//
			// Parameters:	String^ path	�ؼ�DIR
			//
			// Return:		Boolean			�O�_���\
			//
			// History:		1.	2010/05/26 	Created by Frank Lu
			//=================================================================================================
			static Boolean CreateEmptyDirectory(String^ path);

			//=================================================================================================
			// Function:	GetDirectoriesNames
			//
			// Purpose:		���o��Ƨ����l�ؿ��W��
			//
			// Parameters:	String^ path	�ؼ�DIR
			//
			// Return:		Boolean			�O�_���\
			//
			// History:		1.	2011/09/07 	Created by Tony Tsao
			//=================================================================================================
			static array<String^>^ GetDirectoriesNames(String^ path);

			//=================================================================================================
			// Function:	GetDirectoriesNames
			//
			// Purpose:		���o��Ƨ����ŦXpattern���l�ؿ��W��
			//
			// Parameters:	String^ path				�ؼи�Ƨ�
			//				String^ search_pattern		�j�MPattern
			//
			// Return:		Boolean						�O�_���\
			//
			// History:		1.	2012/06/04 	Created by Frank Lu
			//=================================================================================================
			static array<String^>^ GetDirectoriesNames(String^ path, String^ search_pattern);


			//=================================================================================================
			// Function:	GetFilesNames
			//
			// Purpose:		���o��Ƨ����ŦXpattern���l�ؿ��W��
			//
			// Parameters:	String^ path				�ؼи�Ƨ�
			//				String^ search_pattern		�j�MPattern
			//
			// Return:		Boolean						�O�_���\
			//
			// History:		1.	2012/06/04 	Created by Frank Lu
			//=================================================================================================
			static array<String^>^ GetFilesNames(String^ path, String^ search_pattern);

			//=================================================================================================
			// Function:	CheckFileName
			//
			// Purpose:		�T�{�ɮצW�٬O�_�X�k
			//
			// Parameters:	String^ file_name		�ɮצW��
			//
			// Return:		Boolean					�O�_�X�k
			//
			// History:		1.	2012/06/04 	Created by Frank Lu
			//=================================================================================================
			static Boolean CheckFileName(String^ file_name);

		}; // end of class FS

	}; // end namespace CPPTools
} // end of namespace TN
