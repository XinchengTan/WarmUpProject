using System;
using System.Collections.Generic;
using System.Text;

namespace Producer
{
    class RandomStringProgram
    {
        static void Main(string[] args)
        {
            RandomGenerator generator = new RandomGenerator();
            string str = generator.RandomString(10, 4);
            Console.WriteLine($"4 Random Strings of 10 chars is {str}");

            Console.ReadKey();
        }
    }

    public class RandomGenerator
    {
        public string RandomString(int sizeForChar, int sizeForList)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            var stringList = new List<string>();
            char ch;
            for (int i = 0; i < sizeForList; i++)
            {
                for (int j = 0; j < sizeForChar; j++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                stringList.Add(builder.ToString());
                builder.Remove(0, builder.Length);
            }

            var res = String.Join(", ", stringList.ToArray());
            return res.ToString();
        }


    }
}
