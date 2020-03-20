using System;
using OkayBoomer;
using nightmare_nightmare_nightmare;

namespace init
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get ready for some suck");
            var boomer = new OkayBoomer.OkayBoomer(){};
            Console.WriteLine($"Number: {boomer.GetRandom(150)}");
            for(var x = 0; x < 100; x++)
                boomer.GetRandom();
            //var nightmares = new Nightmares(){};
        }
    }
}
