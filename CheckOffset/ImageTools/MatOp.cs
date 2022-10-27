using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.Tools.Debug;

namespace TN.ImageTools
{
    public partial class MatOp
    {
        public static bool Multiply(int[,] src_mat1, int[,] src_mat2, out int[,] dest_mat )
        {
            dest_mat = new int[0,0];
            try
            {
                if (   src_mat1.GetLength(1) != src_mat2.GetLength(0) ) 
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"element num wrong! src_mat1:{src_mat1.GetLength(0)} * {src_mat1.GetLength(1)}  src_mat2:{src_mat2.GetLength(0)} * {src_mat2.GetLength(1)}");
                    return false;
                }

                dest_mat = new int[src_mat1.GetLength(0), src_mat2.GetLength(1)];
                for(int x = 0; x < src_mat1.GetLength(0); x++)
                {
                    for(int y = 0; y < src_mat2.GetLength(1); y++)
                    {
                        dest_mat[x, y] = 0;
                        for ( int element_id = 0; element_id < src_mat1.GetLength(1); element_id++)
                        {
                            dest_mat[x, y] += src_mat1[x, element_id] * src_mat2[element_id, y];
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            dest_mat = new int[0, 0];
            return false;
        }
    }
}
