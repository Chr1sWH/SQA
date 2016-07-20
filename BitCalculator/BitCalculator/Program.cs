using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string operatorX = GetOperator();

                Console.Write("Please enter the base:");
                int inputBase = 10;

                try
                {
                    inputBase = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception) { }

                int inputA = GetInput("Please enter the first number:", inputBase);

                int inputB = GetInput("Please enter the second number:", inputBase);

                int result = 0;

                String operatorOutput = "";
                switch (operatorX)
                {
                    case "1":
                        result = inputA | inputB;
                        operatorOutput = "OR";
                        break;

                    case "2":
                        result = inputA & inputB;
                        operatorOutput = "AND";
                        break;

                    case "3":
                        result = inputA ^ inputB;
                        operatorOutput = "XOR";
                        break;

                    case "4":
                        result = inputA & ~inputB;
                        operatorOutput = "NAND";
                        break;

                    case "5":
                        result = Add(inputA, inputB);
                        operatorOutput = "ADD";
                        break;

                    case "6":
                        result = Sub(inputA, inputB);
                        operatorOutput = "SUB";
                        break;

                    case "7":
                        result = BitCount((uint)inputA);
                        operatorOutput = "BitCount";
                        break;

                    case "8":
                        result = Mul(inputA, inputB);
                        operatorOutput = "MUL";
                        break;

                    case "9":
                        result = Neg(inputA);
                        operatorOutput = "NEG";
                        break;
                }


                Console.WriteLine("________________________________________________");
                Console.WriteLine("Operator: \t" + operatorOutput);
                Console.WriteLine("Number 1: \t" + ToBinary(inputA));
                Console.WriteLine("Number 2: \t" + ToBinary(inputB));

                Console.WriteLine("Result: \t" + ToBinary(result));
                Console.WriteLine("\t\t" + result);
                Console.ReadLine();

            }
        }
        public static string GetOperator()
        {

            Console.WriteLine("Select operator:");
            Console.WriteLine("0 (EXIT)");
            Console.WriteLine("1 (OR)");
            Console.WriteLine("2 (AND)");
            Console.WriteLine("3 (XOR)");
            Console.WriteLine("4 (NAND)");
            Console.WriteLine("5 (ADD)");
            Console.WriteLine("6 (SUB)");
            Console.WriteLine("7 (BitCount)");
            Console.WriteLine("8 (MUL)");
            Console.WriteLine("9 (NEG)");


           String operatorX =  Console.ReadLine();
            if (operatorX == "0")
            {
                System.Environment.Exit(0);
            }
            return operatorX;
        }
        public static string ToBinary(int value)
        {
            String result = "";
            for (int i = 0; i < 32; ++i)
            {
                bool bit = GetBinVal(value, i);
                result = (bit ? "1" : "0") + result;
            }
            return result;
        }

        public static bool GetBinVal(int value, int pos)
        {
            UInt32 temp = ((UInt32)value) >> pos;
            bool result = (temp % 2) == 1;
            return result;
        }

        public static void SetBinVal(ref int value, int pos, bool bit)
        {
            if (!bit)
                return;
            int temp = 1 << pos;
            value = value | temp;
        }

        public static int GetInput(String prompt, int inputBase)
        {
            Console.Write(prompt);
            string B = Console.ReadLine();
            int inputB = 0;
            if (B.StartsWith("-"))
            {
                inputB = Convert.ToInt32(B.Substring(1, B.Length - 1), inputBase);
                inputB = Neg(inputB);
            }
            else
            {
                inputB = Convert.ToInt32(B, inputBase);
            }
            return inputB;
        }

        private static void HalfAdder(bool bit0, bool bit1, out bool res, out bool carry)
        {
            res = bit0 ^ bit1;
            carry = bit0 & bit1;
        }

        private static void FullAdder(bool bit0, bool bit1, bool bit2, out bool res, out bool carry)
        {
            bool tempRes, tempCarry, tempCarry2;
            HalfAdder(bit0, bit1, out tempRes, out tempCarry);
            HalfAdder(bit2, tempRes, out res, out tempCarry2);
            carry = tempCarry | tempCarry2;
        }


        public static int Add(int opperandA, int opperandB, bool ignoreOverflow = true)
        {
            bool bitA = GetBinVal(opperandA, 0);
            bool bitB = GetBinVal(opperandB, 0);
            bool res, carry;
            int result = 0;
            HalfAdder(bitA, bitB, out res, out carry);
            SetBinVal(ref result, 0, res);
            for (int i = 1; i < 32; ++i)
            {
                bitA = GetBinVal(opperandA, i);
                bitB = GetBinVal(opperandB, i);
                FullAdder(bitA, bitB, carry, out res, out carry);
                SetBinVal(ref result, i, res);
            }
            if (carry && !ignoreOverflow)
            {
                throw new OverflowException();
            }
            return result;
        }

        public static int Neg(int opperand)
        {
            if (int.MinValue == opperand)
            {
                throw new OverflowException();
            }
            unchecked { 
                return (~opperand) + 1;
            };
        }

        public static int Sub(int opperandA, int opperandB)
        {
            return Add(opperandA, Neg(opperandB));
        }

        public static int Mul1(int opperandA, int opperandB)
        {
            int res = 0;
            bool sign = false;
            if (opperandB < 0)
            {
                opperandB = Neg(opperandB);
                sign = true;
            }
            if (opperandA < 0)
            {
                opperandA = Neg(opperandA);
                sign = !sign;
            }
            if (Larger(opperandB, opperandA)) Swap(ref opperandA, ref opperandB);
            for (int i = 0; i < opperandB; i++)
            {
                res = Add(opperandA, res);
            }
            return sign? Neg(res) : res;
        }

        public static int Mul(int opperandA, int opperandB)
        {
            int res = 0;
            bool sign = false;
            if (opperandB < 0)
            {
                opperandB = Neg(opperandB);
                sign = true;
            }
            if (opperandA < 0)
            {
                opperandA = Neg(opperandA);
                sign = !sign;
            }
            
            for (int i = 0; i < 31; i++)
            {
                if (GetBinVal(opperandA, i))
                {
                    res = Add(res, opperandB);
                }
                opperandB <<= 1;
            }
            return sign ? Neg(res) : res;

        }

        public static int BitCount1(uint opperand)
        {
            uint count = 0;
            while (opperand != 0)
            {
                count += opperand & 1;
                opperand >>= 1;
            }
            return (int)count;
        }

        public static int BitCount2(uint opperand)
        {
            int count = 0;
            while (opperand != 0)
            {
                uint y = opperand -1;
                opperand &= y;
                ++count;
            }
            return count;
        }
        public static bool Larger(int opperandA, int opperandB)
        {
            int tem = Sub(opperandA, opperandB);
            if (tem == 0) return false;
            return !GetBinVal(tem, 31);
        }

        public static int BitCount(uint opperand)
        {
            uint x1 = opperand & 0x55555555;
            uint x2 = (opperand >> 1) & 0x55555555;
            uint x3 = x1 + x2;
            uint x4 = x3 & 0x33333333;
            uint x5 = (x3 >> 2) & 0x33333333;
            uint x6 = x4 + x5;
            uint x7 = x6 & 0x0F0F0F0F;
            uint x8 = (x6 >> 4) & 0x0F0F0F0F;
            uint x9 = x7 + x8;
            uint x10 = x9 & 0x00FF00FF;
            uint x11 = (x9 >> 8) & 0x00FF00FF;
            uint x12 = x10 + x11;
            uint x13 = x12 & 0x0000FFFF;
            uint x14 = (x12 >> 16) & 0x0000FFFF;
            uint x15 = x13 + x14;
            return (int)x15;
        }
        static void Swap<T>(ref T x, ref T y)
        {
            T t = y;
            y = x;
            x = t;
        }
    }

}
