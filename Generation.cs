using System;
using System.Collections.Generic;


namespace Genetic_Algorithm
{
    public class Generation
    {
        public int generationIndex;
        public List<Chromosome> chromosomes;
        public float sumOfFitness;
        public float maxFitnessVal;
        public int maxFitnessChoromozomeIndex;

        public Generation(int generationIndex)
        {
            this.generationIndex = generationIndex;
        }
        public void GenerationWriter()
        {
            if (!GeneticCreater.writeActive)
                return;
            Console.WriteLine("Index :\t {0} \t Sum Of Fitness :\t {1} \tMaximum Fitness:\t {2} ", generationIndex, sumOfFitness, maxFitnessVal);
        }
    }
}
