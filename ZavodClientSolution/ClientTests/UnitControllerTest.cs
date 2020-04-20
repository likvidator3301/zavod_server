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
        private Unit unitClient;
        private Session sessionClient;
        private SessionDto sessionDto;
        private List<OutputUnitState> allUnits;
        
        [OneTimeSetUp]
        public async Task StartServer()
        {
            var mainClient = new ZavodClient.ZavodClient("http://localhost:5000");
            unitClient = mainClient.Unit;
            var userClient = mainClient.User;
            sessionClient = mainClient.Session;
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(typeof(Startup)); }).Build().RunAsync();
            await userClient.GetNewAccessToken("1//0cEAPecdhSYvCCgYIARAAGAwSNwF-L9Irkwt8nZYlYX5RDE6Yq_nqdkmlQk9opHaqFI3UddInGRQ8ay-YUhcsHjCucqpsWJ5lNCE");
            await sessionClient.CreateSession("SomeMap");
            sessionDto = (await sessionClient.GetAllSessions()).First();
            await sessionClient.EnterSessions(new EnterSessionRequest { SessionId = sessionDto.Id, Nickname = "SomeNickname"});
            allUnits = await unitClient.GetAllUnitStates();
        }

        [OneTimeTearDown]
        public async Task DeleteSession()
        {
            await sessionClient.DeleteSession(sessionDto.Id);
        }
        
        [Test]
        public async Task CreateUnitTest()
        {
            var newUnit = new InputUnitState
            {
                Position = new Vector3 {X = 15, Y = 25, Z = 10},
                Id = Guid.NewGuid(), Requisites = new Dictionary<string, string>(), Type = UnitType.Runner,
                RotationInEulerAngle = new Vector3()
            };
            await unitClient.SendUnitsState(newUnit);
            var result = await unitClient.GetUnitById(newUnit.Id);
            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task GetAllTest()
        {
            var list = await unitClient.GetAllUnitStates();
            list.Should().NotBeEmpty();
        }
        
        [Test]
        public async Task GetByExistIdTest()
        {
            foreach (var unit in allUnits)
            {
                var answer = await unitClient.GetUnitById(unit.Id);
                answer.Id.Should().Be(unit.Id);
            }
        }
        
        [Test]
        public void GetByNotExistIdTest()
        {
            for (int i = 0; i < 15; i++)
            {
                var guidId = Guid.NewGuid();
                Func<Task> gettingUnit = async () => await unitClient.GetUnitById(guidId);
                gettingUnit.Should().ThrowAsync<HttpRequestException>();
            }
        }
        
        [Test]
        public async Task UpdateExistTest()
        {
            var newUnit = new InputUnitState
            {
                Position = new Vector3 {X = 15, Y = 25, Z = 10},
                Id = Guid.NewGuid(), Requisites = new Dictionary<string, string>(), Type = UnitType.Runner,
                RotationInEulerAngle = new Vector3()
            };
            await unitClient.SendUnitsState(newUnit);
            var result = await unitClient.GetUnitById(newUnit.Id);
            result.Position += new Vector3(1,2,3);
            var updatedUnit = await unitClient.SendUnitsState(new InputUnitState{Id = result.Id, 
                Position = result.Position, Requisites = result.Requisites, RotationInEulerAngle = result.RotationInEulerAngle, Type =result.Type});
            updatedUnit.Should().BeEquivalentTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdateNotExistTest()
        {
            var newUnit = new InputUnitState
            {
                Position = new Vector3 {X = 15, Y = 25, Z = 10},
                Id = Guid.NewGuid(), Requisites = new Dictionary<string, string>(), Type = UnitType.Runner,
                RotationInEulerAngle = new Vector3()
            };
            await unitClient.SendUnitsState(newUnit);
            var result = await unitClient.GetUnitById(newUnit.Id);
            result.Id = Guid.NewGuid();
            result.Position = new Vector3(1,2,3);
            var updatedUnit = await unitClient.SendUnitsState(new InputUnitState{Id = Guid.NewGuid(), 
                Position = result.Position, Requisites = result.Requisites, RotationInEulerAngle = result.RotationInEulerAngle, Type =result.Type});
            updatedUnit.Should().BeEquivalentTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task DeleteUnitTest()
        {
            var newUnit = new InputUnitState
            {
                Position = new Vector3 {X = 15, Y = 25, Z = 10},
                Id = Guid.NewGuid(), Requisites = new Dictionary<string, string>(), Type = UnitType.Runner,
                RotationInEulerAngle = new Vector3()
            };
            await unitClient.SendUnitsState(newUnit);
            var unit = await unitClient.GetUnitById(newUnit.Id);
            var result = await unitClient.DestroyUnit(unit.Id);
            result.Should().BeEquivalentTo(HttpStatusCode.OK);
        }
        
        [Test]
        public async Task DeleteNotExistUnitTest()
        {
            Func<Task> result = async () => await unitClient.DestroyUnit(Guid.NewGuid());
            await result.Should().ThrowAsync<HttpRequestException>();
        }
        
        [Test]
        public async Task DistanceBetweenOneUnitTest()
        {
            foreach(var unit in allUnits)
            {
                var result = await unitClient.GetDistanceById(unit.Id, unit.Id);
                result.Should().Be(0);
            }
        }
        
        [Test]
        public async Task DistanceBetweenUnitAndNotExistUnitTest()
        {
            foreach(var unit in allUnits)
            {
                Func<Task> result = async () => await unitClient.GetDistanceById(unit.Id, Guid.NewGuid());
                await result.Should().ThrowAsync<HttpRequestException>();
            }
        }

        [Test]
        public async Task ZeroUnitsAttacksTest()
        {
            var result = await unitClient.SendAttackUnits();
            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task UnitAttacksHimselfTest()
        {
            var newUnit = new InputUnitState
            {
                Position = new Vector3 {X = 15, Y = 25, Z = 10},
                Id = Guid.NewGuid(), Requisites = new Dictionary<string, string>(), Type = UnitType.Runner,
                RotationInEulerAngle = new Vector3()
            };
            await unitClient.SendUnitsState(newUnit);
            var unit = await unitClient.GetUnitById(newUnit.Id);
            var result = await unitClient.AttackUnit(unit.Id, unit.Id, 5);
            var expectedResult = new ResultOfAttackDto {Id = unit.Id,  Hp = unit.Health -5};
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task UnitWithZeroHpTryToAttackTest()
        {
            var newUnit = new InputUnitState
            {
                Position = new Vector3 {X = 15, Y = 25, Z = 10},
                Id = Guid.NewGuid(), Requisites = new Dictionary<string, string>(), Type = UnitType.Runner,
                RotationInEulerAngle = new Vector3()
            };
            await unitClient.SendUnitsState(newUnit);
            var unit = await unitClient.GetUnitById(newUnit.Id);
            await unitClient.AttackUnit(unit.Id, unit.Id, unit.Health);
            var result = await unitClient.AttackUnit(unit.Id, unit.Id, 5);
            result.Should().Be(null);
        }
    }
}