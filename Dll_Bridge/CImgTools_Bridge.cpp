#include "pch.h"
#include "CImgTools_Bridge.h"

using namespace System;

namespace Dll_Bridge
{
     CImgTools_Bridge::CImgTools_Bridge()
    {
         Log_Utl::Init();
         Log_Utl::PathLog = "d:\\Log\\Dll_Bridge\\";
         ExceptionDump::_dump_path = "d:\\Log\\Dll_Bridge\\";

        m_fast_blob = new fast::fastBlob();
    }

     CImgTools_Bridge::~CImgTools_Bridge()
     {
         try
         {
             if (nullptr != m_fast_blob)
             {
                 delete m_fast_blob;
                 m_fast_blob = nullptr;
             }
         }
         catch (Exception^ ex)
         {
             Log_Utl::Log_Event(Event_Level::Error, System::Reflection::MethodBase::GetCurrentMethod()->Name
                 , String::Format("Exception catched: error:{0}", ex->Message));
             // 儲存Exception到檔案
             ExceptionDump::SaveToDefaultFile(ex);
         }
     }

    void CImgTools_Bridge::compute(unsigned char* buffer, int stride, int height)
    {
        try
        {
            m_fast_blob->compute(buffer, stride, height);
        }
        catch (Exception^ ex)
        {
            Log_Utl::Log_Event(Event_Level::Error, System::Reflection::MethodBase::GetCurrentMethod()->Name
                , String::Format("Exception catched: error:{0}", ex->Message));
            // 儲存Exception到檔案
            ExceptionDump::SaveToDefaultFile(ex);
        }
    }

    void CImgTools_Bridge::blobInfo(std::vector<fast::blobInfo>& blob_info)
    {
        blob_info = m_fast_blob->blobInfo();
    }

    bool CImgTools_Bridge::Get_Blob_Res( array<Managed_Blob_Info^>^ %blob_info)
    {
        try
        {
            std::vector<fast::blobInfo> fast_blob_info = m_fast_blob->blobInfo();

            blob_info = gcnew array< Managed_Blob_Info^>(fast_blob_info.size());
            for (int blob_id = 0; blob_id < fast_blob_info.size(); blob_id++)
            {
                blob_info[blob_id] = gcnew Managed_Blob_Info();

                // assign base.
                blob_info[blob_id]->_blob_info->Assign_Value( fast_blob_info[blob_id] );

                // assing contour.
                blob_info[blob_id]->_blob_points = gcnew array<Manaaged_calPoint^>(fast_blob_info[blob_id]._blob_points.size());
                for ( int pt_id = 0; pt_id < blob_info[blob_id]->_blob_points->Length; pt_id++ )
                {
                    blob_info[blob_id]->_blob_points[pt_id] = gcnew Manaaged_calPoint();
                    blob_info[blob_id]->_blob_points[pt_id]->Assign_Value( fast_blob_info[blob_id]._blob_points[pt_id] );
                }
            }

            return true;
        }
        catch (Exception ^ex)
        {
            Log_Utl::Log_Event(Event_Level::Error, System::Reflection::MethodBase::GetCurrentMethod()->Name
                                , String::Format("Exception catched: error:{0}", ex->Message) );
            // 儲存Exception到檔案
            ExceptionDump::SaveToDefaultFile(ex);
        }

        return false;
    }

    bool CImgTools_Bridge::Get_Blob_Res_Bridge(CImgTools_Bridge^ img_tool_clr, array<Managed_Blob_Info_Base^>^ %ret_blobs)
    {
        try
        {
            if (img_tool_clr == nullptr)
            {
                return false;
            }

            //img_tool_clr->Get_Blob_Res(ret_blobs);

            array<Managed_Blob_Info^>^ blob_res;
            img_tool_clr->Get_Blob_Res(blob_res);

            ret_blobs = gcnew array<Managed_Blob_Info_Base^>(blob_res->Length);

            for (int blob_id = 0; blob_id < blob_res->Length; blob_id++)
            {
                ret_blobs[blob_id] = gcnew Managed_Blob_Info_Base();

                ret_blobs[blob_id] = blob_res[blob_id]->_blob_info;
            }

            return true;
        }
        catch (Exception^ ex)
        {
            Log_Utl::Log_Event(Event_Level::Error, System::Reflection::MethodBase::GetCurrentMethod()->Name
                , String::Format("Exception catched: error:{0}", ex->Message));
            // 儲存Exception到檔案
            ExceptionDump::SaveToDefaultFile(ex);
        }
        return false;
    }

    bool Get_Blob_contour(CImgTools_Bridge ^img_tool_clr, int blob_id, array<Manaaged_calPoint^> ^ret_blobs)
    {
        try
        {
            if (img_tool_clr == nullptr)
            {
                return false;
            }

            array<Managed_Blob_Info^>^ blob_res;
            img_tool_clr->Get_Blob_Res(blob_res);

            System::Array::Resize(ret_blobs, blob_res->Length);

            for (int pt_id = 0; pt_id < blob_res[blob_id]->_blob_points->Length; pt_id++)
            {
                ret_blobs[blob_id]->x = blob_res[blob_id]->_blob_points[pt_id]->x;
                ret_blobs[blob_id]->y = blob_res[blob_id]->_blob_points[pt_id]->y;
            }

            return true;
        }
        catch (Exception^ ex)
        {
            Log_Utl::Log_Event(Event_Level::Error, System::Reflection::MethodBase::GetCurrentMethod()->Name
                , String::Format("Exception catched: error:{0}", ex->Message));
            // 儲存Exception到檔案
            ExceptionDump::SaveToDefaultFile(ex);
        }
        return false;
    }
}