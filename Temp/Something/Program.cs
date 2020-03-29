//#define DoTrace // Conditional

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Something {
    interface IMyInterface1 {
        void M1();
    }

    interface IMyInterface2 {
        void M1();
    }

    class myClass : IMyInterface1, IMyInterface2 {
        void IMyInterface1.M1() {
            Console.WriteLine(1);
        }

        void IMyInterface2.M1() {
            Console.WriteLine(2);
        }
        private void M2() {
            Console.WriteLine(3);
        }
    }

    public class Program {
        static void Main(string[] args) {
            myClass mc = new myClass();
            var t = mc.GetType();

            foreach (var item in t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
                Console.WriteLine(item.Name);
                try {
                    item.Invoke(mc, new object[] { });
                    Console.WriteLine("Success\n");
                }
                catch {
                    Console.WriteLine("Неудача...\n");
                }
            }

            //var m = t.GetMethod("M2", BindingFlags.Instance | BindingFlags.NonPublic);
            //m.Invoke(mc, new object[] { });
        }


    }
}