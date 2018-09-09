using System;
using System.Reflection;
using System.Resources;

namespace ACController
{
    class Program
    {
        static void Main(string[] args)
        {
            var resources = new ResourceManager("ACController.strings", typeof(Program).GetTypeInfo().Assembly);
            
            Console.WriteLine($"{resources.GetString("ExhaustAirTemp")} {TempControl.ExhaustAirTemp}");
        }
    }
}
