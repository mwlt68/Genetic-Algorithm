using System;
using System.Collections.Generic;

namespace Genetic_Algorithm
{
    public interface IGeneticAlgorithm
    {
        float ObjectiveFunction(List<float> geneValues);
    }
    public class GeneticCreater
    {
        IGeneticAlgorithm geneticAlgorithm;
        public List<Generation> generations;
        public int inputSize, popopulationSize, precision;
        public float crossingVal, mutationVal;
        public List<Input> inputList;
        public int chromosomeGeneSize = 0;
        public List<Chromosome> chromosomes;
        public float sumOfFitness;
        public float maxFitnessVal;
        public int maxFitnessIndex;
        public int numberOfGeneration;
        public static bool writeActive;
        public GeneticCreater(IGeneticAlgorithm geneticAlgorithm, List<Input> inputList, int popopulationSize, float crossingVal, float mutationVal, int numberOfGeneration, int precision = 1)
        {
            if (inputList == null)
            {
                Console.WriteLine("Error : Input list is null !");
                return;
            }
            this.numberOfGeneration = numberOfGeneration;
            this.geneticAlgorithm = geneticAlgorithm;
            inputSize = inputList.Count;
            this.inputList = inputList;
            chromosomes = new List<Chromosome>(popopulationSize);
            AlgorithmRuleValuesCheck(popopulationSize, precision, crossingVal, mutationVal);
            GetInputsBinarySize();
            CreatePopulation();
            NewGenerationCalculater();
            chromosomesWriter(true);
            generations = Chromosome.ProducerGenerations(this);
            Chromosome.GetGenerationThatHaveMaxFitness(generations);
        }
        // This function, make calculations of the new generation produced
        public void NewGenerationCalculater()
        {
            CalculateInputParametersAndFitness();
            CalculatePossibilityOfSelection();
            CalculateCumulativeProbability();
        }

        // Cumulative Probability, sum of the probability of selection of chromosomes and chromosomes before the chromosome
        private void CalculateCumulativeProbability()
        {
            for (int i = 0; i < chromosomes.Count; i++)
            {
                float cumulativeProbability = 0;
                for (int j = 0; j <= i; j++)
                {
                    cumulativeProbability += chromosomes[j].possibilityOfSelection;
                }
                chromosomes[i].cumulativeProbability = cumulativeProbability;
            }
        }

        // Possibility Of Selection ,  dividing the chromosome fitness value by the total fitness value.
        private void CalculatePossibilityOfSelection()
        {
            sumOfFitness = 0;
            foreach (var chromosome in chromosomes)
            {
                sumOfFitness += chromosome.fitness;
            }
            foreach (var chromosome in chromosomes)
            {
                chromosome.possibilityOfSelection = chromosome.fitness / sumOfFitness;
            }
        }
        // This function , create first generation.
        private void CreatePopulation()
        {
            int counter = 0;
            while (counter < popopulationSize)
            {
                Chromosome chromosome = new Chromosome();
                chromosome.inputParameters = new List<float>(inputList.Count);
                chromosome.binaryVal = new List<Bit>(chromosomeGeneSize);
                foreach (var input in inputList)
                {
                    List<Bit> rndBits = Bit.RandomchromosomeCreate(input.delegeteBitCount);
                    chromosome.binaryVal.AddRange(rndBits);
                }
                chromosomes.Add(chromosome);
                counter++;
            }

        }
        /*
         
         1- shred  choromosome with input bit delegate
         000101110001101000-111110100000100  =>  x1 bits = 000101110001101000   x2 bits =111110100000100
         2- convert binary to float 
         for X1 => 000101110001101000 binary value equal to -1,6374 
         3- calculate finess with function
         =>// Xi = ai + decima(1001…….0012)(bi - ai) / 2mi - 1

            Also, compare fitness value for detect maximum fitness value.
             */
        private void CalculateInputParametersAndFitness()
        {
            maxFitnessVal = 0;
            maxFitnessIndex = 0;
            int counter = 0;
            foreach (var chromosome in chromosomes)
            {
                int bitDividerIndex = 0;
                chromosome.inputParameters.Clear();
                chromosome.inputParameters = new List<float>(inputList.Count);
                foreach (var input in inputList)
                {
                    List<Bit> bits = chromosomeShredder(chromosome.binaryVal, bitDividerIndex, input.delegeteBitCount);
                    bitDividerIndex += input.delegeteBitCount;
                    // Xi = ai + decima(1001…….0012)(bi - ai) / 2mi - 1
                    int decimalVal = Bit.BinaryToDecimalConvert(bits);
                    float topRightRes = input.maxValue - input.minValue;
                    float supRes = (float)(Math.Pow(2, input.delegeteBitCount) - 1);
                    float res = input.minValue + (decimalVal) * (topRightRes) / (supRes);
                    chromosome.inputParameters.Add(res);
                }
                chromosome.fitness = geneticAlgorithm.ObjectiveFunction(chromosome.inputParameters);
                if (chromosome.fitness > maxFitnessVal)
                {
                    maxFitnessVal = chromosome.fitness;
                    maxFitnessIndex = counter;
                }
                counter++;
            }
        }
        private List<Bit> chromosomeShredder(List<Bit> bits, int startIndex, int bitAmount)
        {
            List<Bit> res = new List<Bit>(bitAmount);
            for (int i = 0; i < bitAmount; i++)
            {
                res.Add(bits[startIndex + i]);
            }
            return res;
        }
        // Chromosomes values write to console 
        public void chromosomesWriter(bool isFull)
        {
            if (!writeActive)
                return;
            int i = 0;
            Console.WriteLine("\nIndex \t\t|\t\t Binary \t|\t Fitness  \t|\t Parameters Value\n\n");
            foreach (var chromosome in chromosomes)
            {
                Console.WriteLine("Ch: " + i + "\t\t" + Bit.BitsListToString(chromosome.binaryVal) + "\t" +
                    String.Format("{0:0.0000}", chromosome.fitness) + "\t\t" +
                     "\t" + GetInputParameters(chromosome.inputParameters));
                i++;
            }
            if (isFull)
            {
                i = 1;
                Console.WriteLine("\nIndex\t\t" + "Possibility Of Selection \t|\t Cumulative Probability \n\n");
                foreach (var chromosome in chromosomes)
                {
                    Console.WriteLine("Ch: " + i + "\t\t" + String.Format("{0:0.0000}", chromosome.possibilityOfSelection) + "\t\t\t\t\t" +
                        String.Format("{0:0.0000}", chromosome.cumulativeProbability));
                    i++;
                }
            }
        }
        private string GetInputParameters(List<float> ip)
        {
            string res = "";
            foreach (var par in ip)
            {
                res += String.Format("{0:0.0000}", par);
                res += "\t\t";
            }
            return res;
        }
        /*
         This function calculate, How many bit does it need to delegate  input float value.
         For example, X1 input float between [-3 , 12.1] And precision is 1000.
         Value range = [12.1 - (-3)] * 1000 = 151000
         pow(2,17) < 151000 <pow (2,18)
         so, we need to delegate 18 bits.
             */
        private void GetInputsBinarySize()
        {
            int generalSum = 0;
            foreach (var input in inputList)
            {
                input.delegeteBitCount = Bit.FindBinaryBitAmount(input, precision);
                generalSum += input.delegeteBitCount;
                if (writeActive)
                    Console.WriteLine(input.name + " delegete with " + input.delegeteBitCount + " bits");
            }
            chromosomeGeneSize = generalSum;
            if (writeActive)
                Console.WriteLine("chromosome have " + chromosomeGeneSize + "gene");
        }
        // Check algorithm values.
        private void AlgorithmRuleValuesCheck(int popopulationSize, int precision, float crossingVal, float mutationVal)
        {
            this.popopulationSize = (int)ValueCheck((float)popopulationSize, "popopulationSize");
            this.precision = (int)ValueCheck((float)precision, "precision");
            this.crossingVal = ValueCheck(crossingVal, "crossingVal");
            this.mutationVal = ValueCheck(mutationVal, "mutationVal");
        }
        private float ValueCheck(float val, string err)
        {
            if (val > 0)
            {
                return val;
            }
            else
            {
                Console.WriteLine("Error : Invalid value : " + err + " !");
                return 0;
            }
        }

    }
    /*
     This class store input value,
     For example, X1 input float between [-3 , 12.1]
     input name is X1
     minValue is -3 
     maxValue is 12.1 
     delegeteBitCount is 18   How can calculate this value. Look  GetInputsBinarySize function.
         */
    public class Input
    {
        public string name = "";
        public float minValue = 0, maxValue = 0;
        public int delegeteBitCount = 0;
        public Input(string name, float minValue, float maxValue)
        {
            if (maxValue <= minValue)
            {
                Console.WriteLine("Error : Input minimum cannot greater or equal  than maximum !");
                return;
            }
            this.name = name;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}
