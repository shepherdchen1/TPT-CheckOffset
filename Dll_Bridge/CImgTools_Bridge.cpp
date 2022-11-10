#include "pch.h"
#include "CImgTools_Bridge.h"

namespace Dll_Bridge
{
     CImgTools_Bridge::CImgTools_Bridge()
    {
        m_fast_blob = new fast::fastBlob();
    }

     CImgTools_Bridge::~CImgTools_Bridge()
     {
         if (nullptr != m_fast_blob)
         {
             delete m_fast_blob;
             m_fast_blob = nullptr;
         }
     }

    void CImgTools_Bridge::compute(unsigned char* buffer, int stride, int height)
    {
        m_fast_blob->compute(buffer, stride, height);
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
                Managed_Blob_Info^ new_blob = gcnew Managed_Blob_Info();
                new_blob->_blob_info->_id = fast_blob_info[blob_id]._id;
                new_blob->_blob_info->_rect_x = fast_blob_info[blob_id]._rect_x;
                new_blob->_blob_info->_rect_y = fast_blob_info[blob_id]._rect_y;

                new_blob->_blob_info->_rect_width = fast_blob_info[blob_id]._rect_width;
                new_blob->_blob_info->_rect_height = fast_blob_info[blob_id]._rect_height;

                new_blob->_blob_info->_centeriod_x = fast_blob_info[blob_id]._centeriod_x;
                new_blob->_blob_info->_centeriod_y = fast_blob_info[blob_id]._centeriod_y;

                blob_info[blob_id] = new_blob;
            }

            return true;
        }
        catch (System::Exception ^ex)
        {
            Log_Utl::Log_Event(Event_Level::Error, System::Reflection::MethodBase::GetCurrentMethod()->Name
                                , String::Format("Exception catched: error:{0}", ex->Message) );
            // Àx¦sException¨ìÀÉ®×
            ExceptionDump::SaveToDefaultFile(ex);
        }
    }

    bool CImgTools_Bridge::Get_Blob_Res_Bridge(CImgTools_Bridge^ img_tool_clr, array<Managed_Blob_Info_Base^>^ %ret_blobs)
    {
        if (img_tool_clr == nullptr)
        {
            return false;
        }

        array<Managed_Blob_Info^> ^blob_res;
        img_tool_clr->Get_Blob_Res(blob_res);

        ret_blobs = gcnew array<Managed_Blob_Info_Base^>(blob_res->Length);

        for (int blob_id = 0; blob_id < blob_res->Length; blob_id++)
        {
            ret_blobs[blob_id] = gcnew Managed_Blob_Info_Base();

            ret_blobs[blob_id]->_id = blob_res[blob_id]->_blob_info->_id;

            ret_blobs[blob_id]->_rect_x = blob_res[blob_id]->_blob_info->_rect_x;
            ret_blobs[blob_id]->_rect_y = blob_res[blob_id]->_blob_info->_rect_y;
            ret_blobs[blob_id]->_rect_width = blob_res[blob_id]->_blob_info->_rect_width;
            ret_blobs[blob_id]->_rect_height = blob_res[blob_id]->_blob_info->_rect_height;

            ret_blobs[blob_id]->_centeriod_x = blob_res[blob_id]->_blob_info->_centeriod_x;
            ret_blobs[blob_id]->_centeriod_y = blob_res[blob_id]->_blob_info->_centeriod_y;
        }

        return true;
    }

    bool Get_Blob_contour(CImgTools_Bridge ^img_tool_clr, int blob_id, array<Manaaged_calPoint^> ^ret_blobs)
    {
        if (img_tool_clr == nullptr)
        {
            return false;
        }

        array<Managed_Blob_Info^> ^blob_res;
        img_tool_clr->Get_Blob_Res(blob_res);

        System::Array::Resize(ret_blobs, blob_res->Length);

        for (int pt_id = 0; pt_id < blob_res[blob_id]->_blob_points->Length; pt_id++)
        {
            ret_blobs[blob_id]->x = blob_res[blob_id]->_blob_points[pt_id]->x;
            ret_blobs[blob_id]->y = blob_res[blob_id]->_blob_points[pt_id]->y;
        }

        return true;
    }
}