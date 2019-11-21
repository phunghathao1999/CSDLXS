using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Infractructure.Common
{
    public class SetPresentThread
    {
        // Static method for thread a 
        public static void thread1()
        {
            for (int z = 0; z < 5; z++)
            {
                Console.WriteLine(z);
            }
        }

        // static method for thread b 
        public static void thread2()
        {
            for (int z = 0; z < 5; z++)
            {
                Console.WriteLine(z);
            }
        }
    }
}
