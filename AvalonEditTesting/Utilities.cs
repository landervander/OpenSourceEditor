using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AvalonEditTesting
{
    internal class Utilities
    {
        public static bool IsWindowOpen(string name)
        {
            for(int i = 0; i < App.Current.Windows.Count; i++)
            {
                string winName = App.Current.Windows[i].Name;
                Console.WriteLine(winName);
                if (winName == name)
                    return true;
                else
                    return false;
            }
            return false;
        }
    }
}
