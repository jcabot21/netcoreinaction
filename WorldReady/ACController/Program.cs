using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ACController
{
    class Program
    {
        // static void Main(string[] args)
        // {
        //     var culture = CultureInfo.CreateSpecificCulture("es-MX");   // Alternative en-US for english

        //     // Used for formatting dates and times and for sort order
        //     Thread.CurrentThread.CurrentCulture = culture;

        //     // Used by ResourceManager to determine resources to use
        //     Thread.CurrentThread.CurrentUICulture = culture;

        //     var resources = new ResourceManager("ACController.strings", typeof(Program).GetTypeInfo().Assembly);
            
        //     Console.WriteLine($"{resources.GetString("ExhaustAirTemp")} {TempControl.ExhaustAirTemp}");
        // }

        private static void Main(string [] args)
        {
            var controller = new Controller();

            controller.Test();
        }
    }
}
