using GenericAlgorithm.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAlgorithm
{
    internal class Chromosome
    {
        public List<int> Genes { get; private set; }

        public Chromosome()
        {
            Genes = new List<int>();

            for (int i = 0; i < Map.CityLength; i++)
            {
                Genes.Add(0);
            }
        }

        public void Random(int[] geneValues)
        {
            Genes = new List<int>();

            do
            {
                var randomNum = new Random(Guid.NewGuid().GetHashCode()).Next(0, geneValues.Length);

                if(Genes.Find(g => g == geneValues[randomNum]) == 0)
                {
                    Genes.Add(geneValues[randomNum]);
                }

            } while(Genes.Count != geneValues.Length);

        }

        public void Insert(int[] geneValues, int from = 0)
        {

            for (int i = from; i < Map.CityLength; i++)
            {
                if(geneValues.Length > i - from)
                {
                    Genes[i] = geneValues[i - from];
                }
            }
        }

        public void Insert(int geneValue, int index = 0)
        {
            Genes[index] = geneValue;
        }

        public double Fitness()
        {
            var distances = Map.Distances;
            var distance = 0d;

            for (int i = 0; i < Genes.Count;i++)
            {
                var dis = distances.SingleOrDefault(d => 
                (d.SourceId == Genes[i] && d.DestinationId == Genes[(i + 1) % Genes.Count]) || 
                (d.SourceId == Genes[(i + 1) % Genes.Count] && d.DestinationId == Genes[i]))
                    .DirectDistance;

                distance += dis;
            }

            var totalDistances = 0d;
            Map.Distances.ToList().ForEach(d => totalDistances += d.DirectDistance);

            return totalDistances / distance;
        }

        public void Mutate(int at)
        {
            int randomNum;
            do
            {
                var random = new Random(Guid.NewGuid().GetHashCode());
                randomNum = random.Next(0, Genes.Count);
            }
            while (randomNum == at);

            (Genes[randomNum], Genes[at]) = (Genes[at], Genes[randomNum]);
        }
    }
}
