using System;
using System.Collections.Generic;
using System.Text;

namespace Genetic_Algorithm
{
    public class Chromosome
    {
        static Random random = new Random();
        public List<Bit> binaryVal;
        public List<float> inputParameters;
        public float fitness;
        public float possibilityOfSelection;
        public float cumulativeProbability;

        // List<chromosome> -> One Generation
        public static List<Generation> ProducerGenerations(GeneticCreater geneticCreater)
        {
            List<Generation> generations = new List<Generation>(geneticCreater.numberOfGeneration);
            for (int i = 0; i < geneticCreater.numberOfGeneration; i++)
            {
                TurningWheelRoulette(geneticCreater);
                CrossingOperation(geneticCreater);
                // Last generation mustn't  mutation operation.
                if (i != (geneticCreater.numberOfGeneration - 1))
                    MutationOperation(geneticCreater);
                geneticCreater.NewGenerationCalculater();
                AddNewGeneration(generations, geneticCreater, i);
                geneticCreater.chromosomesWriter(true);
            }
            geneticCreater.chromosomesWriter(true);
            return generations;
        }

        // This function detect generation that have maximum fitness value.
        public static Generation GetGenerationThatHaveMaxFitness(List<Generation> generations)
        {

            Generation resGeneration = new Generation(0);
            float maxFitness = 0;
            foreach (var gen in generations)
            {
                if (gen.maxFitnessVal > maxFitness)
                {
                    maxFitness = gen.maxFitnessVal;
                    resGeneration = gen;
                }
            }
            if (GeneticCreater.writeActive)
                Console.WriteLine("Max Fitnees Generation");
            resGeneration.GenerationWriter();
            return resGeneration;
        }
        // every generation is stored in the generations list
        private static void AddNewGeneration(List<Generation> generations, GeneticCreater geneticCreater, int index)
        {
            Generation generation = new Generation(index);
            generation.chromosomes = geneticCreater.chromosomes;
            generation.maxFitnessChoromozomeIndex = geneticCreater.maxFitnessIndex;
            generation.maxFitnessVal = geneticCreater.maxFitnessVal;
            generation.sumOfFitness = geneticCreater.sumOfFitness;
            generations.Add(generation);
            generation.GenerationWriter();
        }
        /*
        Random values are generated as much as the population size. Chromosomes are matched according to these values.
             */
        private static void TurningWheelRoulette(GeneticCreater geneticCreater)
        {
            if (GeneticCreater.writeActive)
                Console.WriteLine("\n***********Turning Wheel Roulette***********\n");
            List<double> rndDoubles = GetRandomDoubles(geneticCreater.popopulationSize);
            List<int> matchList = GetMatchList(geneticCreater.chromosomes, rndDoubles);
            geneticCreater.chromosomes = chromosomesMatch(geneticCreater.chromosomes, matchList);
            geneticCreater.chromosomesWriter(false);
        }

        /*
         Random values are generated as much as the population size.
         The chromosomes in the index, which are less than the possibility of crossing, are suitable for crossing.
         The amount of chromosomes selected must be dual.
         The chromosomes are crossed from an index randomly selected in order.
         */
        private static void CrossingOperation(GeneticCreater geneticCreater)
        {
            if (GeneticCreater.writeActive)
                Console.WriteLine("\n***********Crossing Over***********\n");
            List<double> rndDoubles = GetRandomDoubles(geneticCreater.popopulationSize);
            List<int> crossIndex = new List<int>();
            for (int i = 0; i < rndDoubles.Count; i++)
            {
                if (rndDoubles[i] < geneticCreater.crossingVal)
                    crossIndex.Add(i);
            }
            if (CheckCrossingDual(ref crossIndex))
            {
                for (int i = 0; i < crossIndex.Count; i += 2)
                {
                    CrossingOver(geneticCreater, crossIndex[i], crossIndex[i + 1]);
                }
            }
        }
        /*
         For example, there are 20 chromosomes consisting of 33 bits. In total, 33 * 20 = 660 bits.
         660 random values are generated. Chromosomes in the index smaller than the mutation probability will mutate.
            For example, the selected for mutate bit is 51.
            51/33 = 1, which value is the chromosome
            51 mod 33 = 18, this value is the Ch index value.
             */
        private static void MutationOperation(GeneticCreater geneticCreater)
        {
            if (GeneticCreater.writeActive)
                Console.WriteLine("\n***********Mutation Process***********\n");
            int sumOfBitCount = (geneticCreater.chromosomeGeneSize * geneticCreater.chromosomes.Count) - 1;
            List<double> doubles = GetRandomDoubles(sumOfBitCount);
            List<int> mutationIndex = new List<int>();
            for (int i = 0; i < doubles.Count; i++)
            {
                if (doubles[i] < geneticCreater.mutationVal)
                    mutationIndex.Add(i);
            }
            foreach (var index in mutationIndex)
            {
                int modVal = index % geneticCreater.chromosomeGeneSize;
                int remaindVal = index / geneticCreater.chromosomeGeneSize;
                Bit bit;
                if (Bit.GetRandomBool())
                    bit = new Bit('1');
                else
                    bit = new Bit('0');
                if (GeneticCreater.writeActive)
                {
                    Console.WriteLine("Mutation Index : {0} \t chromosome : {1} \t Ch Index : {2}", index, remaindVal, modVal);
                    Console.WriteLine("Old Val: {0} \t New Val: {1}\n", Bit.BitToChar(geneticCreater.chromosomes[remaindVal].binaryVal[modVal]), Bit.BitToChar(bit));
                }

                geneticCreater.chromosomes[remaindVal].binaryVal[modVal].value = bit.value;

            }

        }

        /*
         A random index is selected.
         The two selected chromosomes cross from this index.For example,
            INPUT
         firstChromosome=100011000101101001111000001110010
         secondChromosome=111011101101110000100011111011110
         index=9
         
            RESULT 
         firstChromosome=10001100 1101110000100011111011110
         secondChromosome=111011100 101101001111000001110010
         
         */
        private static void CrossingOver(GeneticCreater geneticCreater, int firstChromosome, int secondChromosome)
        {
            int willDivideIndex = random.Next(1, geneticCreater.chromosomeGeneSize - 1);
            if (GeneticCreater.writeActive)
                Console.WriteLine("First: " + firstChromosome + " Second: " + secondChromosome + " Index: " + willDivideIndex);
            int tempIndexSize = geneticCreater.chromosomeGeneSize - willDivideIndex;
            List<Bit> tempTrack = new List<Bit>(tempIndexSize);
            for (int i = willDivideIndex; i < geneticCreater.chromosomeGeneSize; i++)
            {
                bool val = geneticCreater.chromosomes[firstChromosome].binaryVal[i].value;
                geneticCreater.chromosomes[firstChromosome].binaryVal[i].value = geneticCreater.chromosomes[secondChromosome].binaryVal[i].value;
                geneticCreater.chromosomes[secondChromosome].binaryVal[i].value = val;
            }
            string f = Bit.BitsListToString(geneticCreater.chromosomes[firstChromosome].binaryVal);
            string s = Bit.BitsListToString(geneticCreater.chromosomes[secondChromosome].binaryVal);
            if (GeneticCreater.writeActive)
            {
                Console.WriteLine("first:  " + f);
                Console.WriteLine("second: " + s);
            }

        }
        private static bool CheckCrossingDual(ref List<int> crossPosibleIndex)
        {
            if (crossPosibleIndex.Count < 2) // Crossing doesnt enought;
                return false;
            else
            {
                if (crossPosibleIndex.Count % 2 == 1) // Match is not posible.To be a match, x must be an even number.
                    crossPosibleIndex.RemoveAt(0);
                return true;
            }
        }
        private static List<double> GetRandomDoubles(int amount)
        {
            List<double> doubles = new List<double>(amount);
            for (int i = 0; i < amount; i++)
            {
                doubles.Add(random.NextDouble());
            }
            return doubles;
        }
        // The random value selected refers to the chromosome that is larger than the cumulative probability.
        private static List<int> GetMatchList(List<Chromosome> chromosomes, List<double> doubles)
        {
            List<int> result = new List<int>(doubles.Count);
            for (int i = 0; i < doubles.Count; i++)
            {
                for (int j = 0; j <= chromosomes.Count; j++)
                {
                    if (chromosomes[j].cumulativeProbability > doubles[i])
                    {
                        result.Add(j);
                        break;
                    }
                }
            }
            return result;
        }
        /* 
         * Matches chromosomes.For example,
            Old: 0   New : 7
            Old: 1   New : 16
            Old: 2   New : 11
            Old: 3   New : 4
            Old: 4   New : 12
            Old: 5   New : 13
            Old: 6   New : 11
            Old: 7   New : 17
            Old: 8   New : 8
            Old: 9   New : 5
            Old: 10  New : 19
            Old: 11  New : 13
            Old: 12  New : 6
            Old: 13  New : 8
            Old: 14  New : 11
            Old: 15  New : 0
            Old: 16  New : 12
            Old: 17  New : 17
            Old: 18  New : 3
            Old: 19  New : 9
             */
        private static List<Chromosome> chromosomesMatch(List<Chromosome> chromosomes, List<int> matchList)
        {
            if (GeneticCreater.writeActive)
                Console.WriteLine("chromosomesMatch");
            List<Chromosome> cloneChorozomes = ChoromozomesCoppier(chromosomes);
            for (int i = 0; i < chromosomes.Count; i++)
            {
                if (GeneticCreater.writeActive)
                    Console.WriteLine("Old: " + i + "\t New : " + matchList[i]);
                ChoromozomeCoppier(chromosomes[matchList[i]], cloneChorozomes[i]);
            }
            return cloneChorozomes;
        }
        // Source chromosomes values copy to destination chromosomes values
        private static List<Chromosome> ChoromozomesCoppier(List<Chromosome> refchromosome)
        {
            List<Chromosome> cloneChorozomes = new List<Chromosome>(refchromosome.Count);
            foreach (var ch in refchromosome)
            {
                Chromosome chromosome = new Chromosome();
                ChoromozomeCoppier(ch, chromosome);
                cloneChorozomes.Add(chromosome);
            }
            return cloneChorozomes;
        }
        // Source chromosome values copy to destination chromosome values
        private static void ChoromozomeCoppier(Chromosome source, Chromosome destination)
        {
            destination.binaryVal = source.binaryVal;
            destination.cumulativeProbability = source.cumulativeProbability;
            destination.fitness = source.fitness;
            destination.inputParameters = source.inputParameters;
            destination.possibilityOfSelection = source.possibilityOfSelection;
        }
    }
}
