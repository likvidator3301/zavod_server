using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using NUnit.Framework;

namespace ZavodClient
{
    [TestFixture]
    public class UnitControllerTest
    {
        
        Process pc = new Process();
        private void StartServer()
        {
            pc.StartInfo.FileName = "cmd";
            pc.StartInfo.RedirectStandardInput = true;
            pc.StartInfo.RedirectStandardOutput = true;
            pc.StartInfo.CreateNoWindow = true;
            pc.StartInfo.UseShellExecute = false;
            pc.Start();
            pc.StandardInput.WriteLine("cd C:\\Users\\scryptan\\source\\repos\\Practice c#\\ZavodServer\\ZavodServer");
            pc.StandardInput.Flush();
            pc.StandardInput.WriteLine("dotnet run");
            pc.StandardInput.Flush();
            pc.StandardInput.Close();
        }
        
        [Test]
        public void GetAllTest()
        {
            StartServer();    
            var client = new ClientMainClass();
            var list = client.GetObjectsIds().Result;
            for (int i = 0; i < list.Count; i++)
                Console.WriteLine(list[i]);
            Assert.IsNotEmpty(list);
        }
        
        [TestCase("55db7766-4250-4e27-a351-e5aab980258c")]
        [TestCase("7c4dbe5a-2379-4598-90b0-082c6ce48d4b")]
        public void GetByExistIdTest(string id)
        {
            var guidId = Guid.Parse(id);
            StartServer();
            var allUnits = GetAll();
            var client = new ClientMainClass();
            var answer = client.GetObjectByIdAsync(guidId).Result;
            foreach (var unitDto in allUnits)
                Console.WriteLine(unitDto);
            Console.WriteLine(answer);
            Assert.IsTrue(allUnits.Select(x => x).Where(x => x.Id == answer.Id).Count() > 0);
        }
        
        [TestCase("b066bb09-c9c9-49c2-be41-571602813f86")]
        [TestCase("693f731b-954b-44ea-a9c2-56c37d3c40bd")]
        public void GetByVotExistIdTest(string id)
        {
            var guidId = Guid.Parse(id);
            StartServer();
            var client = new ClientMainClass();
            Assert.ThrowsAsync<HttpRequestException>(()=> client.GetObjectByIdAsync(guidId));
        }
        private List<UnitDto> GetAll()
        {
            StartServer();    
            var client = new ClientMainClass();
            var list = client.GetObjectsIds().Result;
            return list;
        }
    }
}