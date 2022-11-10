#pragma once

#include "D:\\Source\\TN\\TN_Static_ImageTools\\blob_analyze\\fastBlob.h"


namespace Dll_Bridge
{
    public ref class Manaaged_calPoint {
    public:
        double x;
        double y;

        Manaaged_calPoint()
        {
            x = 0;
            y = 0;
        };
    };

    public ref class Managed_Blob_Info_Base {
    public:

        int _id;
        int _rect_x;
        int _rect_y;
        int _rect_width;
        int _rect_height;

        double _centeriod_x;
        double _centeriod_y;

    public:

        Managed_Blob_Info_Base(int id, int rect_x, int rect_y, int rect_width, int rect_height, double centeroid_x, double centeroid_y) {
            this->_id = id;
            this->_rect_x = rect_x;
            this->_rect_y = rect_y;
            this->_rect_width = rect_width;
            this->_rect_height = rect_height;

            this->_centeriod_x = centeroid_x;
            this->_centeriod_y = centeroid_y;
        }

        Managed_Blob_Info_Base()
        {
            this->_id = 0;
            this->_rect_x = 0;
            this->_rect_y = 0;
            this->_rect_width = 0;
            this->_rect_height = 0;

            this->_centeriod_x = 0;
            this->_centeriod_y = 0;
        }

        int id() {
            return _id;
        }
        int rectX() {
            return _rect_x;
        }
        int rectY() {
            return _rect_y;
        }
        int rectWidth() {
            return _rect_width;
        }
        int rectHeight() {
            return _rect_height;
        }

    };

    public ref class Managed_Blob_Info {
    public:

        Managed_Blob_Info_Base      ^_blob_info;

        array<Manaaged_calPoint^> ^_blob_points;

    public:

        Managed_Blob_Info(int id, int rect_x, int rect_y, int rect_width, int rect_height, double centeroid_x, double centeroid_y) {
            _blob_info = gcnew Managed_Blob_Info_Base();
            _blob_points = nullptr;

            _blob_info->_id = id;
            _blob_info->_rect_x = rect_x;
            _blob_info->_rect_y = rect_y;
            _blob_info->_rect_width = rect_width;
            _blob_info->_rect_height = rect_height;

            _blob_info->_centeriod_x = centeroid_x;
            _blob_info->_centeriod_y = centeroid_y;
        };

        Managed_Blob_Info()
        {
            _blob_info = gcnew Managed_Blob_Info_Base();

            _blob_info->_id = 0;
            _blob_info->_rect_x = 0;
            _blob_info->_rect_y = 0;
            _blob_info->_rect_width = 0;
            _blob_info->_rect_height = 0;

            _blob_info->_centeriod_x = 0;
            _blob_info->_centeriod_y = 0;
        }

        int id() {
            return _blob_info->_id;
        }
        int rectX() {
            return _blob_info->_rect_x;
        }
        int rectY() {
            return _blob_info->_rect_y;
        }
        int rectWidth() {
            return _blob_info->_rect_width;
        }
        int rectHeight() {
            return _blob_info->_rect_height;
        }

        void setBlobPoints(array<Manaaged_calPoint^> ^points) {
            this->_blob_points = points;
        }

        array<Manaaged_calPoint^>^ getBlobPoints() {
            return this->_blob_points;
        }

    };

 


    public ref class CImgTools_Bridge
    {
    public:
        CImgTools_Bridge();
        virtual ~CImgTools_Bridge();

        void compute(unsigned char* buffer, int width, int height);
        //void compute(ImageTool_Buffer image_tool_buffer);

        void blobInfo(std::vector<fast::blobInfo>& blob_info);
        //void blobInfo(fast::blobInfo* blob_info);

        bool Get_Blob_Res(array<Managed_Blob_Info^>^ %ret_blobs);

        bool Get_Blob_Res_Bridge(CImgTools_Bridge^ img_tool_clr, array<Managed_Blob_Info_Base^>^ %ret_blobs);

    private:
        fast::fastBlob* m_fast_blob;
    };
}
