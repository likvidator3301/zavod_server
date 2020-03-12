using System;
using Models;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ZavodClient.ZavodClient("http://localhost:5000").User; 
            // client.OnRegisterSuccessful += testing;
            // var result = client.GetUser().Result;
            // Console.WriteLine(result.Email);
            Console.ReadKey();
        }  
        
        private static void testing(ServerUserDto a)
        {
            Console.WriteLine(a.Email);
        }
    }
}