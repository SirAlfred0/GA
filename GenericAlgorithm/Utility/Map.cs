using ExcelDataReader;
using GenericAlgorithm.Extentions;
using GenericAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAlgorithm.Utility
{
    internal class Map
    {

        private int _scale;
        private int _neighborhoodRange;

        public List<City> Cities { get; private set; }
        public static Distance[] Distances { get; private set; }

        public static int CityLength { get; private set; }

        public Map(int scale, int neighborhoodRange)
        {
            _scale = scale;
            _neighborhoodRange = neighborhoodRange;
            Init();
        }

        private void Init()
        {
            Cities = new List<City>();
            var filePath = @"./Data/cities.xlsx";
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            var city = new City()
                            {
                                Id = Int32.Parse(reader.GetDouble(0).ToString()),
                                Name = reader.GetString(1),
                                X = reader.GetDouble(2),
                                Y = reader.GetDouble(3),
                                Color = reader.GetString(4)
                            };

                            Cities.Add(city);
                        }
                    } while (reader.NextResult());
                }
            }

            Distances = Cities.Distance(_neighborhoodRange, _scale).ToArray();
            CityLength = Cities.Count;
        }

        public void Print()
        {
            //Console.WriteLine(color.ToString() + "♦");     
            var easternCity = Cities.MostEastern();
            var southernCity = Cities.SouthernMost();

            for(int i = 0; i <= southernCity.Y; i++)
            {
                for(int j = 0; j <= easternCity.X; j++)
                {
                    var city = Cities.Find(c => c.X == j && c.Y == i);
                    if(city != null)
                    {
                        var neighbors = Cities.Neighbors(city, _neighborhoodRange, _scale);

                        var neighborsString = " (";

                        for(int k = 0; k < neighbors.Length;k++)
                        {
                            neighborsString += neighbors[k].Name + '=' + neighbors[k].Distance.ToString();

                            if(k != neighbors.Length - 1) neighborsString += ",";
                        }
                        neighborsString += ')';

                        ConsoleColor.TryParse(city.Color, out ConsoleColor color);
                        Console.ForegroundColor = color;
                        Console.Write(city.Id.ToString() + '.' + city.Name);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("\t");
                    }
                }
                Console.Write("\n\n");
            }
            Console.Write("==========================================================================\n");

            foreach(var city in Cities)
            {
                var neighbors = Cities.Neighbors(city, _neighborhoodRange, _scale);

                ConsoleColor.TryParse(city.Color, out ConsoleColor color);
                Console.ForegroundColor = color;
                Console.Write(city.Id.ToString() + '.' + city.Name.ToString());
                Console.ResetColor();
                Console.Write(" (");

                for (int k = 0; k < neighbors.Length; k++)
                {
                    ConsoleColor.TryParse(neighbors[k].Color, out ConsoleColor neighborColor);
                    Console.ForegroundColor = neighborColor;
                    Console.Write(neighbors[k].Name + '=' + neighbors[k].Distance.ToString());
                    Console.ResetColor();

                    if (k != neighbors.Length - 1) Console.Write(',');
                }

                Console.WriteLine(") ");
            }

            Console.Write("\n");
        }





    }
}
