using System;
using System.Collections.Generic;
using System.Text;

namespace Genetic_Algorithm
{
    public class Bit
    {
        private static Random random = new Random();
        public bool value;
        public Bit(char val)
        {
            if (val == '0')
                this.value = false;
            else if (val == '1')
                this.value = true;
            else
                Console.WriteLine("Error : Bit value is invalid !");
        }
        public static int BinaryToDecimalConvert(string binaryString)
        {
            return Convert.ToInt32(binaryString, 2);
        }
        public static int BinaryToDecimalConvert(List<Bit> bits)
        {
            String binaryString = BitsListToString(bits);
            return Convert.ToInt32(binaryString, 2);
        }
        public static List<Bit> DecimalToBinaryConvert(int value)
        {
            string result = "";
            while (value > 1)
            {
                int remainder = value % 2;
                result = Convert.ToString(remainder) + result;
                value /= 2;
            }
            result = Convert.ToString(value) + result;
            return Bit.StringToBitList(result);
        }
        public static List<Bit> StringToBitList(string str)
        {
            List<Bit> bits = new List<Bit>(str.Length);
            foreach (var c in str)
            {
                if (c == '0' || c == '1')
                    bits.Add(new Bit(c));
                else
                    Console.WriteLine("Error : Bit value is invalid !");
            }
            return bits;
        }
        public static string BitsListToString(List<Bit> bits)
        {
            string resultBits = "";
            foreach (var bit in bits)
            {
                resultBits += BitToChar(bit);
            }
            return resultBits;
        }
        public static char BitToChar(Bit bit)
        {
            if (bit.value)
                return '1';
            else
                return '0';
        }
        public static int FindBinaryBitAmount(Input input, int precision)
        {
            int res = 0;
            int mathRes = (int)((input.maxValue - input.minValue) * precision);
            while (mathRes > Math.Pow(2, res))
            {
                res++;
            }
            return res;
        }
        public static List<Bit> RandomchromosomeCreate(int size)
        {
            int counter = 0;
            List<Bit> bits = new List<Bit>(size);
            while (counter < size)
            {
                if (GetRandomBool())
                    bits.Add(new Bit('1'));
                else
                    bits.Add(new Bit('0'));
                counter++;
            }
            return bits;
        }
        public static bool GetRandomBool()
        {
            if (random.NextDouble() >= 0.5)
                return true;
            else
                return false;
        }
    }
}
