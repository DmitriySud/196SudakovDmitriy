//#define DoTrace // Conditional

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Something {

    public class Program {
        static void Main(string[] args) {
            int[] m = new int[10];
            Action[] dm = new Action[10];
            for (int i = 0; i < 10; i++) {
                dm[i] = () => { m[10 - i - 1]++; };
            }
            dm[1].Invoke();
            foreach (var item in m) {
                Console.Write($"{item}, ");
            }
        }


    }
}