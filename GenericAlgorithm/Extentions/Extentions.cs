using GenericAlgorithm.Model;
using GenericAlgorithm.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAlgorithm.Extentions
{
    internal static class Extentions
    {
        public static City MostEastern(this List<City> self)
        {
            var max = self.Max(c => c.X);
            return self.Find(c => c.X == max);
        }

        public static City SouthernMost(this List<City> self)
        {
            var max = self.Max(c => c.Y);
            return self.Find(c => c.Y == max);
        }

        public static Neighbor[] Neighbors(this List<City> self, City city, int range, int scale)
        {
            var neighbors = new List<Neighbor>();

            self.ForEach(c => {
                if(c != city)
                {
                    var distance = Math.Floor(Math.Sqrt(Math.Pow((c.X - city.X), 2) + Math.Pow((c.Y - city.Y), 2)));
                    if(distance <= range)
                    {
                        var neighbor = new Neighbor()
                        {
                            Self = city,
                            Id = c.Id,
                            Name = c.Name,
                            X = c.X,
                            Y = c.Y,
                            Distance = distance * scale,
                            Color = c.Color
                        };
                        neighbors.Add(neighbor);
                    }
                }
            });
            
            return neighbors.ToArray();
        }

        public static List<Distance> Distance(this List<City> self, int range, int scale)
        {
            var distances = new List<Distance>();
            for(int i = 0; i < self.Count; i++)
            {
                for(int j = i+1; j < self.Count; j++)
                {
                    var distance = Math.Floor(Math.Sqrt(Math.Pow((self[i].X - self[j].X), 2) + Math.Pow((self[i].Y - self[j].Y), 2)));

                    var d = new Distance()
                    {
                        SourceId = self[i].Id,
                        DestinationId = self[j].Id,
                        DirectDistance = distance * scale,
                    };

                    if (distance > range)
                    {
                        d.DirectDistance = 100 * (distance - range) * scale;
                    }

                    distances.Add(d);
                }
            }

            return distances;
        }

        public static Chromosome Combine(this Chromosome self, Chromosome cromosome)
        {
            var offspring = new Chromosome();

            var random = new Random(Guid.NewGuid().GetHashCode());
            var firstIndex = random.Next(0, self.Genes.Count);
            var lastIndex = random.Next(1, self.Genes.Count - firstIndex);

            offspring.Insert(self.Genes.GetRange(firstIndex, lastIndex).ToArray(), firstIndex);

            var offspringPointer = (firstIndex + lastIndex) % offspring.Genes.Count;
            var cromosomePointer = (firstIndex + lastIndex) % cromosome.Genes.Count;

            while (offspring.Genes.Contains(0))
            {
                if (!offspring.Genes.Contains(cromosome.Genes[cromosomePointer]))
                {
                    offspring.Insert(cromosome.Genes[cromosomePointer], offspringPointer);

                    offspringPointer = (offspringPointer + 1) % offspring.Genes.Count;
                }

                cromosomePointer = (cromosomePointer + 1) % cromosome.Genes.Count;
            }

            return offspring;
        }

        public static Chromosome BestFit(this List<Chromosome> self)
        {
            var fitnessList = new List<double>();

            self.ForEach(c => {
                fitnessList.Add(c.Fitness());
            });

            var maxFit = fitnessList.Max();

            return self.Find(c => c.Fitness() == maxFit);
        }

        public static string AsString(this List<int> genes)
        {
            var p = "";
            genes.ForEach(g => p+= g.ToString() + ',');
            return p;
        }
    }
}
