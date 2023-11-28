
using GenericAlgorithm.Extentions;
using GenericAlgorithm.Model;
using GenericAlgorithm.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericAlgorithm
{
    internal class SGA
    {
        private int _popSize;
        private double _pm;
        private double _pc;
        private Map _map;
        private List<Chromosome> chromosomes;
        private List<Chromosome> rouletteWheelChromosomes;
        private DataHolder tournamentDataHolder;
        private DataHolder rouletteDataHolder;

        public SGA(Map map, int popSize, double mutationProbability, double crossOverProbability) 
        {
            if(popSize % 2 != 0)
            {
                throw new Exception("Popsize must be even!!");
            }

            _popSize = popSize;
            _map = map;
            _pc = crossOverProbability;
            _pm = mutationProbability;
            Init();
        }

        private void Init()
        {
           CreateInitialPopulations();
        }

        private void CreateInitialPopulations()
        {
            chromosomes = new List<Chromosome>();
            rouletteWheelChromosomes = new List<Chromosome>();

            rouletteDataHolder = new DataHolder();
            tournamentDataHolder = new DataHolder();


            for (int i = 0; i < _popSize; i++)
            {
                var Chromosome = new Chromosome();      
                Chromosome.Random(_map.Cities.Select(c => c.Id).ToArray());
                chromosomes.Add(Chromosome);
            }

            rouletteWheelChromosomes = chromosomes;
        }

        public void Run(int numberOfRounds, int tournament)
        {
            if(tournament > _popSize)
            {
                throw new Exception("tournament selection size is bigger than pop size");
            }

            for(int i = 0;i < numberOfRounds;i++)
            {
               var tournamentParents = TournamentParentSelection(tournament);
               var rouletteWheelParents = RouletteParentSelection();


                rouletteDataHolder.bestFitBeforeCrossOver.Add(rouletteWheelParents.BestFit());
                tournamentDataHolder.bestFitBeforeCrossOver.Add(tournamentParents.BestFit());

                var tournamentOffsprings = CrossOver(tournamentParents);
                var rouletteOffsprings = CrossOver(rouletteWheelParents);

                rouletteDataHolder.bestFitBeforeMutation.Add(rouletteOffsprings.BestFit());
                tournamentDataHolder.bestFitBeforeMutation.Add(tournamentOffsprings.BestFit());

                tournamentOffsprings = Mutation(tournamentOffsprings);
                rouletteOffsprings = Mutation(rouletteOffsprings);

                rouletteDataHolder.bestFit.Add(rouletteOffsprings.BestFit());
                tournamentDataHolder.bestFit.Add(tournamentOffsprings.BestFit());

                chromosomes = tournamentOffsprings;
                rouletteWheelChromosomes = rouletteOffsprings;
            }

            Console.WriteLine("===================================================================");
            Console.WriteLine("Tournament Final Answer: " + chromosomes.BestFit().Genes.AsString());
            Console.WriteLine("Roulette Wheel Final Answer: " + rouletteWheelChromosomes.BestFit().Genes.AsString());
            Console.WriteLine("===================================================================");
        }

        private List<Chromosome> TournamentParentSelection(int tournament)
        {
            var parentList = new List<Chromosome>();

            do
            {
                var tournamentSelectedParents = new List<Chromosome>();

                while (tournamentSelectedParents.Count < tournament)
                {
                    var random = new Random(Guid.NewGuid().GetHashCode()).Next(0, chromosomes.Count);

                    tournamentSelectedParents.Add(chromosomes[random]);
                }

                var selectedParent = tournamentSelectedParents.Select(sp => new
                {
                    self = sp,
                    fitness = sp.Fitness(),
                }).OrderBy(sp => sp.fitness).Reverse().ToList()[0];

                parentList.Add(selectedParent.self);

            } while(parentList.Count < _popSize);

            return parentList;
        }

        private List<Chromosome> RouletteParentSelection()
        {
            var parentList = new List<Chromosome>();
            var sumFitness = 0d;

            foreach(var chromosome in rouletteWheelChromosomes)
            {
                sumFitness += chromosome.Fitness();
            }

            var chromosomeChances = new List<ChromosomeRouletteChance>();

            for(var i = 0; i < rouletteWheelChromosomes.Count; i++)
            {
                var from = i == 0 ? 0 : chromosomeChances[i - 1].To;

                var chromosomeChance = new ChromosomeRouletteChance()
                {
                    Chromosome = rouletteWheelChromosomes[i],
                    Form = from,
                    To = from + (rouletteWheelChromosomes[i].Fitness() / sumFitness)
                };

                chromosomeChances.Add(chromosomeChance);
            }


            for(var i = 0; i < _popSize; i++)
            {
                var random = new Random(Guid.NewGuid().GetHashCode());
                var randomNumber = random.NextDouble();

                var selectedParent = chromosomeChances.Find(c => c.Form <= randomNumber && c.To > randomNumber).Chromosome;

                parentList.Add(selectedParent);
            }

            return parentList;
        }

        private List<Chromosome> CrossOver(List<Chromosome> parents)
        {
            var offsprings = new List<Chromosome>();
            

            for (int i = 0; i < parents.Count; i+=2)
            {
                var random = new Random(Guid.NewGuid().GetHashCode());
                if (random.NextDouble() <= _pc)
                {
                    var offspring1 = parents[i].Combine(parents[i + 1]);
                    var offspring2 = parents[i + 1].Combine(parents[i]);

                    offsprings.Add(offspring1);
                    offsprings.Add(offspring2);
                }else
                {
                    offsprings.Add(parents[i]);
                    offsprings.Add(parents[i+1]);
                }

            }

            return offsprings;
        }

        private List<Chromosome> Mutation(List<Chromosome> offsprings)
        {
            

            offsprings.ForEach(offSpring => {
                for(int i = 0; i < offSpring.Genes.Count;i++)
                {
                    var random = new Random(Guid.NewGuid().GetHashCode());
                    if (random.NextDouble() <= _pm)
                    {
                        offSpring.Mutate(i);
                    }
                }
            });

            return offsprings;
        }

        public void ShowAlgorithmInfo()
        {
            Console.WriteLine();
            for (int i = 0; i < tournamentDataHolder.bestFit.Count; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Stage " + (i + 1).ToString() + ": ");


                Console.WriteLine("Tournament: ");

                Console.WriteLine("Best fit in Parent Selection: " + tournamentDataHolder.bestFitBeforeCrossOver[i].Genes.AsString());
                Console.WriteLine("Fitness: " + tournamentDataHolder.bestFitBeforeCrossOver[i].Fitness().ToString());

                Console.WriteLine("Best fit AfterCrossOver: " + tournamentDataHolder.bestFitBeforeMutation[i].Genes.AsString());
                Console.WriteLine("Fitness: " + tournamentDataHolder.bestFitBeforeMutation[i].Fitness().ToString());

                Console.WriteLine("Best fit After Muttation: " + tournamentDataHolder.bestFit[i].Genes.AsString());
                Console.WriteLine("Fitness: " + tournamentDataHolder.bestFit[i].Fitness().ToString());


                Console.WriteLine("Roulette: ");
                Console.WriteLine("Best fit in Parent Selection: " + rouletteDataHolder.bestFitBeforeCrossOver[i].Genes.AsString());
                Console.WriteLine("Fitness: " + rouletteDataHolder.bestFitBeforeCrossOver[i].Fitness().ToString());
                Console.WriteLine("Best fit AfterCrossOver: " + rouletteDataHolder.bestFitBeforeMutation[i].Genes.AsString());
                Console.WriteLine("Fitness: " + rouletteDataHolder.bestFitBeforeMutation[i].Fitness().ToString());
                Console.WriteLine("Best fit After Muttation: " + rouletteDataHolder.bestFit[i].Genes.AsString());
                Console.WriteLine("Fitness: " + rouletteDataHolder.bestFit[i].Fitness().ToString());
                Console.WriteLine();
            }
            Console.WriteLine();

            ChartManager.CreateChart(tournamentDataHolder, rouletteDataHolder);
        }

    }
}
