using GenericAlgorithm.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAlgorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var map = new Map(10, 3);
            map.Print();
            var sga = new SGA(map, 10, .1, 1);

            Console.WriteLine("===================================================================================================================");

            Console.WriteLine("Enter Number Of Rounds For Genetic Algorithm:");

            var numberOfRounds = Int32.Parse(Console.ReadLine());
            sga.Run(numberOfRounds, 4);

            Console.WriteLine("Do you need more information?");
            Console.WriteLine("0.No");
            Console.WriteLine("1.Yes");

            var showInformation = int.Parse(Console.ReadLine()) == 1;

            if (showInformation)
                sga.ShowAlgorithmInfo();

            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();
        }
    }
}
