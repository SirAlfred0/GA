using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAlgorithm.Model
{
    internal class Neighbor: City
    {
        public double Distance { get; set; }
        public City Self { get; set; }
    }
}
