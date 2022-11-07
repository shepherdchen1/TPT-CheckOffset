using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CheckOffset.For_Main.Image_Tools;
using static CheckOffset.For_Main;
using TN.Tools.Debug;
using Export_Dll;
using System.Runtime.InteropServices;

namespace CheckOffset.CSharp2Cpp
{
    public class BlobAnalyzeAdapter
    {
        public BlobAnalyzeAdapter()
        { }

        public bool Get_Blob_Res(Image_Tools_CLR img_tool_clr, ref Blob_Infos blob_infos )
        {
            try
            {
                Blob_Infos blob_res = new Blob_Infos();

                var cpparray = Image_Tools.Get_Blob_Res(img_tool_clr);

                //[return: MarshalAs(UnmanagedType.I1)]
                //private static extern bool TestLong([In][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LongMarshaler))] object lValue)

                    ;
                var test_data_2 = Marshal.ReadInt32(cpparray, 2 * sizeof(int));
                Console.WriteLine(test_data_2); // 55

                Image_Tools.Get_Blob_Res(img_tool_clr, ref blob_res);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }
    }
}
