using System;
using ZavodClient;

namespace ConsoleAppDebugZavodClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main()
        {
            UnitDto unitDto = new UnitDto();
            var client = new ClientMainClass();

            Uri buff = await client.CreateObject(unitDto);
            Console.WriteLine(buff.ToString());


        }
    }
}
