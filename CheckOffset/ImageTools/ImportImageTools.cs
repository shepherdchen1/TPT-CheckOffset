using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CheckOffset.ImageTools
{

    public class ImportImageTools
    {
        // C#:
        [DllImport("TNImageTools.dll")]
        public static extern IntPtr CreateClass();


        [DllImport("TNImageTools.dll")]
        static public extern void DisposeClass(IntPtr pClassObject);

        //IntPtr pClass = CreateClass();
        //DisposeClass(pClass);
        //pClass = IntPtr.Zero; 
        //// Always NULL out deleted objects in order to prevent a dirty pointer
    }
}
