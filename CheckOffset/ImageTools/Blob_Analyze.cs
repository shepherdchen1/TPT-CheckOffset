using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using TN.ImageTools;
using TN.Tools.Debug;

namespace CheckOffset.ImageTools
{
    public class Blob_Analyze
    {
        private byte[,] _buffer;

        private int[,] _blob;
        public int[,] Blob { get => _blob; set => _blob = value; }

        public Blob_Analyze()
        {
            _buffer = new byte[0, 0];
            _blob = new int[0, 0];
        }

        public bool Blob_Detect(byte[,] buffer)
        {
            try
            {
                Clear_Result();
                _buffer = new byte[buffer.GetLength(0), buffer.GetLength(1)];
                _blob   = new int[buffer.GetLength(0), buffer.GetLength(1)];
                byte[] zero_buf = new byte[_buffer.GetLength(1)];
                for (int y = 0; y < buffer.GetLength(0); y++)
                {
                    Buffer.BlockCopy(buffer, y * buffer.GetLength(1), _buffer, y * buffer.GetLength(1), buffer.GetLength(1));
                    Buffer.BlockCopy(zero_buf, 0, _blob, y * _buffer.GetLength(0), zero_buf.GetLength(0));
                }

                if (   buffer.GetLength(0) <= 0
                    || buffer.GetLength(1) <= 0 )
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                         , $"buffer wrong!-->length:{buffer.GetLength(0)}, {buffer.GetLength(1)}");
                    return true;
                }

                int blob_id = 1;
                for(int y = 0; y < buffer.GetLength(0); y++)
                {
                    for(int x = 0; x < buffer.GetLength(1); x++)
                    {
                        Assign_Blob_ID(x, y, blob_id, out bool new_blob_found);
                        if (!new_blob_found)
                            continue;

                        blob_id++;
                    }
                }
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        private bool Assign_Blob_ID(int x, int y, int blob_id, out bool new_blob_found)
        {
            new_blob_found = false;
            try
            {
                if (y < 0 || y >= _blob.GetLength(0))
                    return true;
                if (x < 0 || x >= _blob.GetLength(1) )
                    return true;

                if (_buffer[y, x] == 0)
                    return true;

                if (_blob[y, x] > 0)
                    return true;

                _blob[y, x] = blob_id;
                new_blob_found = true;

                Assign_Blob_ID(x + 1, y,     blob_id, out bool new_child_blob_found);
                Assign_Blob_ID(x,     y + 1, blob_id, out new_child_blob_found);
                Assign_Blob_ID(x - 1, y,     blob_id, out new_child_blob_found);
                Assign_Blob_ID(x,     y - 1, blob_id, out new_child_blob_found);

                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        //private bool Modify_Blob_ID(int old_id, int new_id)
        //{
        //    try
        //    {
        //        for (int y = 0; y < _blob.GetLength(0); y++)
        //        {
        //            for (int x = 0; x < _blob.GetLength(1); x++)
        //            {
        //                if (_blob[y, x] != old_id)
        //                    continue;

        //                _blob[y, x] = new_id;
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //                       , $"Exception catched: error:{ex.Message}");
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }

        //    return false;
        //}

        private void Clear_Result()
        {
            try
            {
                _blob = new int[_buffer.GetLength(0), _buffer.GetLength(1)];

                byte[] zero_buf = new byte[_buffer.GetLength(1)];
                for (int y = 0; y < _buffer.GetLength(0); y++)
                {
                    Buffer.BlockCopy(zero_buf, 0, _blob, y * _buffer.GetLength(0), zero_buf.GetLength(0));
                }
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
