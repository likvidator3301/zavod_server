using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using ZavodServer;
using Models;
using ZavodServer.Controllers;

namespace ClientTests
{
    [TestFixture]
    public class UnitControllerTest
    {
        private List<ServerUnitDto> allUnits;
        private List<ServerUnitDto> defaultUnits;
        private UnitController unitController = new UnitController();
        
        [OneTimeSetUp]
        public void StartServer()
        {
            defaultUnits = new List<ServerUnitDto>();
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(typeof(Startup)); }).Build().RunAsync();
            defaultUnits.Add(new ServerUnitDto
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
                Type = UnitType.Chelovechik
            }); 
            defaultUnits.Add(new ServerUnitDto
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
                Type = UnitType.Warrior
            });
        }
        
        [OneTimeSetUp]
        public Task GetAll()
        {
            allUnits = unitController.GetAll().ToList<ServerUnitDto>();
        }
        
        [Test]
        public Task GetAllTest()
        {
            var list = unitController.GetAll();
            list.Should().NotBeEmpty();
        }
        
        [TestCase("55db7766-4250-4e27-a351-e5aab980258c")]
        [TestCase("7c4dbe5a-2379-4598-90b0-082c6ce48d4b")]
        public Task GetByExistIdTest(string id)
        {
            var guidId = Guid.Parse(id);
            var answer = unitController.GetUnitById(guidId);
            Assert.IsTrue(allUnits.Select(x => x).Where(x => x.Id == answer.Id).Count() > 0);
        }
        
        [TestCase("b066bb09-c9c9-49c2-be41-571602813f86")]
        [TestCase("693f731b-954b-44ea-a9c2-56c37d3c40bd")]
        public void GetByNotExistIdTest(string id)
        {
            var guidId = Guid.Parse(id);
            Assert.Throws<ArgumentException>(()=> unitController.GetUnitById(guidId));
        }
        
        [Test]
        public Task UpdateExistTest()
        {
            foreach (var unit in defaultUnits)
            {
                unit.AttackDamage++;
                unit.CurrentHp -= 10;
                var updatedUnit = unitController.UpdateUnit(unit);
                unit.Should().BeEquivalentTo(updatedUnit);
            }
        }

        [Test]
        public Task AddUnitTest()
        {
            foreach(var unit in defaultUnits)
            {
                unit.Id = Guid.NewGuid();
                var result = unitController.CreateUnit(unit);
                result.Should().BeEquivalentTo(unit);
            }
        }

        [Test]
        public async Task DeleteUnitTest()
        {
            foreach(var unit in allUnits)
            {
                var result = unitController.DeleteUnit(unit.Id);
                result.Should().BeEquivalentTo(HttpStatusCode.OK);
                allUnits.RemoveAt(0);
                return;
            }
        }
        
        [Test]
        public Task DistanceBetweenOneObjectTest()
        {
            foreach(var unit in allUnits)
            {
                var result = unitController.GetDistanceById(unit.Id, unit.Id);
                result.Should().Be(0);
            }
        }
    }
}