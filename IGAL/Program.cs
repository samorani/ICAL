using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using KP;
using CCP;

namespace IGAL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //DataTableTest();
            //CCP_Main.CCPMain();
            KP_Main.KPMain();
        }

        private static void DataTableTest()
        {
            Test.DataTest dt = new Test.DataTest();
            dt.TestExpander();
        }
    }
}
