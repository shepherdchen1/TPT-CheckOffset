using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CheckOffset.ImageTools
{
    //[StructLayout(LayoutKind.Sequential)]
    //public struct ImageTool_Buffer
    //{
    //    public int Stride;
    //    public int Height;
    //    public IntPtr Data;

    //    //public ImageTool_Buffer(byte[] data, int stride) : this()
    //    public ImageTool_Buffer(IntPtr data, int stride, int height) : this()
    //    {
    //        //Data = Marshal.AllocHGlobal(data.Length);
    //        //Marshal.Copy(data, 0, Data, data.Length);
    //        //Stride = stride;
    //        //Height = data.Length / stride;
    //        Data = Marshal.AllocHGlobal(stride * height);
    //        Marshal.Copy( data.ToPointer(), 0, Data, stride * height);
    //        Stride = stride;
    //        Height = height;
    //    }
    //}

    public partial class CSharp2Cpp : Component
    {
        public CSharp2Cpp()
        {
            InitializeComponent();
        }

        public CSharp2Cpp(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
