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
using Models;
using NUnit.Framework;
using ZavodClient;
using ZavodServer;

namespace ClientTests
{
    [TestFixture]
    public class UnitControllerTest
    {
        private Units client;
        private List<ServerUnitDto> allUnits;
        [OneTimeSetUp]
        public async Task StartServer()
        {
            client = new Units("http://localhost:5000");
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(typeof(Startup)); }).Build().RunAsync();
            allUnits = await client.GetAll();
        }
        
        [Test]
        public async Task CreateUnits()
        {
            var result = await client.CreateUnit(UnitType.Warrior, new Vector3{X =15, Y = 25,Z = 10});
            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task GetAllTest()
        {
            var list = await client.GetAll();
            list.Should().NotBeEmpty();
        }
        
        [Test]
        public async Task GetByExistIdTest()
        {
            foreach (var unit in allUnits)
            {
                var answer = await client.GetUnitById(unit.Id);
                allUnits.Where(x => x.Id == answer.Id).Count().Should().BeGreaterThan(0);
            }
        }
        
        [Test]
        public void GetByNotExistIdTest()
        {
            for (int i = 0; i < 15; i++)
            {
                var guidId = Guid.NewGuid();
                Func<Task> gettingUnit = async () => await client.GetUnitById(guidId);
                gettingUnit.Should().ThrowAsync<HttpRequestException>();
            }
        }
        
        [Test]
        public async Task UpdateExistTest()
        {
            foreach (var unit in allUnits)
            {
                unit.AttackDamage++;
                unit.CurrentHp -= 10;
                var updatedUnit = await client.UpdateUnit(unit);
                unit.Should().BeEquivalentTo(updatedUnit);
            }
        }

//        [Test]
//        public async Task AddUnitTest()
//        {
//            foreach(var unit in defaultUnits)
//            {
////                unit.Id = Guid.NewGuid();
//                var result = await client.CreateUnit(UnitType.Warrior);
////                result.Should().BeEquivalentTo(unit);
//            }
//        }

//        [Test]
//        public async Task DeleteUnitTest()
//        {
//            foreach(var unit in allUnits)
//            {
//                var result = await client.DeleteUnit(unit.Id);
//                result.Should().BeEquivalentTo(HttpStatusCode.OK);
//                allUnits.RemoveAt(0);
//                return;
//            }
//        }
        
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