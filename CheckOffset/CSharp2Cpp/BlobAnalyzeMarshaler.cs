using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using TN.Tools.Debug;
using System.Numerics;
using static CheckOffset.For_Main.Image_Tools;
using System.Diagnostics;
using System.Drawing;

namespace CheckOffset.CSharp2Cpp
{
    public class BlobAnalyzeMarshaler : ICustomMarshaler
    //public class BlobAnalyzeMarshaler: ArrayMarshaler<Blob_Info>
    {
        public BlobAnalyzeMarshaler()
        {

        }

        public static ICustomMarshaler GetInstance(string pstrCookie)
        {
            return new BlobAnalyzeMarshaler();
        }

        public void CleanUpManagedData(object managed_data)
        {
            try
            {
                //return Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BlobAnalyzeMarshaler)));
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            //return null;
        }

        public void CleanUpNativeData(IntPtr native_ptr)
        {
            try
            {
                Marshal.FreeHGlobal(native_ptr);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            //return null;
        }

        public int GetNativeDataSize()
        {
            try
            {
                return 3337;
                //return Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BlobAnalyzeMarshaler)));
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return 0;
        }

        public object  MarshalNativeToManaged(IntPtr native_ptr)
        {
            try
            {

                ////Int32 blob_num_test = IntPtr.Size(native_ptr); // (native_ptr);
                //Int32 blob_num_test = Marshal.SizeOf<IntPtr>(native_ptr); // (native_ptr);
                //Int32 blob_num = Marshal.ReadInt32(native_ptr);
                //if (blob_num <= 0)
                //    return null;

                IntPtr cur_native_ptr = native_ptr;// + sizeof(Int32);

                int blob_num = 300;
                Blob_Info[] new_blob = new Blob_Info[blob_num];
                unsafe
                {
                    for (int blob_id = 0; blob_id < blob_num; blob_id++)
                    {
                        Blob_Info test_blob = new Blob_Info();
                        //Blob_Info* native_blob = (Blob_Info*) cur_native_ptr.ToPointer();
                        Marshal.PtrToStructure(cur_native_ptr, test_blob._blob_info);


                        test_blob._blob_info._id = *((int*) cur_native_ptr.ToPointer());
                        cur_native_ptr += sizeof(Int32);
                        test_blob._blob_info._rect_x = (int)cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int32);
                        test_blob._blob_info._rect_y = (int)cur_native_ptr.ToInt64();
                        cur_native_ptr += sizeof(Int64);
                        test_blob._blob_info._rect_width = (int)cur_native_ptr.ToInt64();
                        cur_native_ptr += sizeof(Int64);
                        test_blob._blob_info._rect_height = (int) cur_native_ptr.ToInt64();
                        cur_native_ptr += sizeof(Int64);

                        new_blob[blob_id]._blob_info._id = cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int32);
                        new_blob[blob_id]._blob_info._rect_x = cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int32);
                        new_blob[blob_id]._blob_info._rect_y = cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int32);
                        new_blob[blob_id]._blob_info._rect_width = cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int32);
                        new_blob[blob_id]._blob_info._rect_height = cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int32);

                        new_blob[blob_id]._blob_info._centeriod_x = (double) cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int64);
                        new_blob[blob_id]._blob_info._centeriod_y = (double) cur_native_ptr.ToInt32();
                        cur_native_ptr += sizeof(Int64);

                        // marshal base.
                        //Marshal.PtrToStructure(cur_native_ptr, new_blob[blob_id]._blob_info);
                        //cur_native_ptr += sizeof(Blob_Info_Base);

                        // marshal contour.
                        //int contour_corner_num = Marshal.ReadInt32(native_ptr);
                        //cur_native_ptr += sizeof(Int32);

                        //for (int corner_pt_id = 0; corner_pt_id < contour_corner_num; corner_pt_id++)
                        //{
                        //    Marshal.PtrToStructure(cur_native_ptr, new_blob[blob_id]._blob_info._contour._blob_points[corner_pt_id]);
                        //    cur_native_ptr += sizeof(Blob_Info_Contour_calPoint);
                        //}
                    }
                }

                //Blob_Info[] ManagedObject = new Blob_Info[blob_num];
                //Marshal.Copy(native_ptr + sizeof(int), ManagedObject, 0,
                //    blob_num * sizeof(Blob_Info));

                return new_blob;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return null;
        }

        public IntPtr MarshalManagedToNative(object managed_obj)
        {
            try
            {
                Blob_Info[] blob_Infos = (Blob_Info[]) managed_obj;

                //var allocatedMemory = new List<IntPtr>();

                int intPtrSize = Marshal.SizeOf(typeof(IntPtr));
                IntPtr nativeArray = Marshal.AllocHGlobal(intPtrSize * blob_Infos.GetLength(0));
                for (int i = 0; i < blob_Infos.GetLength(0); i++)
                {
                    IntPtr native_blob = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));

                    IntPtr _blob_info = IntPtr.Add(native_blob, 0);
                    _blob_info = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Blob_Info_Base)));
                    Marshal.StructureToPtr(blob_Infos[i]._blob_info, _blob_info, false);

                    IntPtr _contour = IntPtr.Add(native_blob, Marshal.SizeOf(typeof(IntPtr)) );
                    _contour = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                    Marshal.StructureToPtr(blob_Infos[i]._contour, _contour, false);

                    ////allocatedMemory.Add(native_blob);

                    Marshal.WriteIntPtr(nativeArray, i * intPtrSize, native_blob);
                }

                return nativeArray;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return IntPtr.Zero;
        }
    }
}
