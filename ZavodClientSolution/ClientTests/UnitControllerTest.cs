using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using ZavodClient;
using ZavodServer;

namespace ClientTests
{
    [TestFixture]
    public class UnitControllerTest
    {
        private Units client;
        private List<UnitDto> allUnits;
        private List<UnitDto> defaultUnits;
        [OneTimeSetUp]
        public void StartServer()
        {
            defaultUnits = new List<UnitDto>();
            client = new Units("http://localhost:5000");
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(typeof(Startup)); }).Build().RunAsync();
            defaultUnits.Add(new UnitDto
            {
                Id = Guid.Parse("55db7766-4250-4e27-a351-e5aab980258c"), 
                Position = new Vector3{X = 0, Y= 0,Z=0},
                Rotation = new Vector3{X = 0, Y= 0,Z=0},
                AttackDamage = 100,
                AttackDelay = 1,
                AttackRange = 2,
                CurrentHp = 100,
                Defense = 25,
                LastAttackTime = 3,
                MaxHp = 150,
                Type = "4elik"
            }); 
            defaultUnits.Add(new UnitDto
            {
                Id = Guid.Parse("7c4dbe5a-2379-4598-90b0-082c6ce48d4b"), 
                Position = new Vector3{X = 0, Y = 1, Z = 2},
                Rotation = new Vector3{X = 10, Y = 10, Z = 10},
                AttackDamage = 150,
                AttackDelay = 3,
                AttackRange = 1,
                CurrentHp = 150,
                Defense = 25,
                LastAttackTime = 3,
                MaxHp = 200,
                Type = "warrior"
            });
        }
        
        [OneTimeSetUp]
        public async Task GetAll()
        {
            allUnits = await client.GetAll();
        }
        
        [Test]
        public async Task GetAllTest()
        {
            var list = await client.GetAll();
            list.Should().NotBeEmpty();
        }
        
        [TestCase("55db7766-4250-4e27-a351-e5aab980258c")]
        [TestCase("7c4dbe5a-2379-4598-90b0-082c6ce48d4b")]
        public async Task GetByExistIdTest(string id)
        {
            var guidId = Guid.Parse(id);
            var answer = await client.GetUnitById(guidId);
            Assert.IsTrue(allUnits.Select(x => x).Where(x => x.Id == answer.Id).Count() > 0);
        }
        
        [TestCase("b066bb09-c9c9-49c2-be41-571602813f86")]
        [TestCase("693f731b-954b-44ea-a9c2-56c37d3c40bd")]
        public void GetByNotExistIdTest(string id)
        {
            var guidId = Guid.Parse(id);
            Assert.ThrowsAsync<HttpRequestException>(()=> client.GetUnitById(guidId));
        }
        
        [Test]
        public async Task UpdateExistTest()
        {
            foreach (var unit in defaultUnits)
            {
                unit.AttackDamage++;
                unit.CurrentHp -= 10;
                var updatedUnit = await client.UpdateUnit(unit);
                unit.Should().BeEquivalentTo(updatedUnit);
            }
        }

        [Test]
        public async Task AddUnitTest()
        {
            foreach(var unit in defaultUnits)
            {
                unit.Id = Guid.NewGuid();
                var result = await client.CreateUnit(unit);
                result.Should().BeEquivalentTo(unit);
            }
        }

        [Test]
        public async Task DeleteUnitTest()
        {
            foreach(var unit in allUnits)
            {
                var result = await client.DeleteUnit(unit.Id);
                result.Should().BeEquivalentTo(HttpStatusCode.OK);
                allUnits.RemoveAt(0);
                return;
            }
        }
        
        [Test]
        public async Task DistanceBetweenOneObjectTest()
        {
            foreach(var unit in allUnits)
            {
                var result = await client.GetDistanceById(unit.Id, unit.Id);
                result.Should().Be(0);
            }
        }
    }
}