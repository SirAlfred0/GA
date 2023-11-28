using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAlgorithm.Model
{
    internal class DataHolder
    {
        public List<Chromosome> bestFitBeforeCrossOver;
        public List<Chromosome> bestFitBeforeMutation;
        public List<Chromosome> bestFit;


        public DataHolder()
        {
            bestFit = new List<Chromosome>();
            bestFitBeforeMutation = new List<Chromosome>();
            bestFitBeforeCrossOver = new List<Chromosome>();
        }
    }
}
