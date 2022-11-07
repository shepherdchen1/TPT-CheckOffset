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
    public class BlobAnalyzeMarshaler: ICustomMarshaler
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

        public int GetNativeDataSize()
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

            return 0;
        }

        public object  MarshalNativeToManaged(IntPtr native_ptr)
        {
            try
            {

                //Int32 blob_num_test = IntPtr.Size(native_ptr); // (native_ptr);
                Int32 blob_num_test = Marshal.SizeOf<IntPtr>(native_ptr); // (native_ptr);
                Int32 blob_num = Marshal.ReadInt32(native_ptr);
                if (blob_num <= 0)
                    return null;

                IntPtr cur_native_ptr = native_ptr + sizeof(Int32);

                Blob_Info[] new_blob = new Blob_Info[blob_num];
                unsafe
                {
                    //for (int blob_id = 0; blob_id < blob_num; blob_id++)
                    //{
                    //    // marshal base.
                    //    Marshal.PtrToStructure(cur_native_ptr, new_blob[blob_id]._blob_info);
                    //    cur_native_ptr += sizeof(Blob_Info_Base);

                    //    // marshal contour.
                    //    int contour_corner_num = Marshal.ReadInt32(native_ptr);
                    //    cur_native_ptr += sizeof(Int32);

                    //    for(int corner_pt_id = 0; corner_pt_id < contour_corner_num; corner_pt_id++)
                    //    {
                    //        Marshal.PtrToStructure(cur_native_ptr, new_blob[blob_id]._contour._blob_points[corner_pt_id]);
                    //        cur_native_ptr += sizeof(Blob_Info_Contour_calPoint);
                    //    }
                    //}
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
                List<Blob_Info> blob_Infos = (List<Blob_Info>)managed_obj;

                var allocatedMemory = new List<IntPtr>();

                int intPtrSize = Marshal.SizeOf(typeof(IntPtr));
                IntPtr nativeArray = Marshal.AllocHGlobal(intPtrSize * blob_Infos.Count);
                for (int i = 0; i < blob_Infos.Count; i++)
                {
                    IntPtr native_blob = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Blob_Info)));
                    allocatedMemory.Add(native_blob);
                    Marshal.StructureToPtr(blob_Infos[i], native_blob, false);

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
