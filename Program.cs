using System;
using OkayBoomer;

namespace init
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get ready for some suck");
            var boomer = new OkayBoomer.OkayBoomer(){};
            Console.WriteLine($"Number: {boomer.GetRandom(150)}");
        }
    }
}
