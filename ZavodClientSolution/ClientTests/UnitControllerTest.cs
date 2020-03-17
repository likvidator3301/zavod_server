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
        private Unit client;
        private List<ServerUnitDto> allUnits;
        private List<DefaultServerUnitDto> defaultUnits;
        
        [OneTimeSetUp]
        public async Task StartServer()
        {
            client = new ZavodClient.ZavodClient("http://localhost:5000").Unit;
            // var token =
            //     "ya29.a0Adw1xeXFijbjqVBapbGX7fe1wICoKFwHXDm5LmyE0WJdErDY28e2EaqgweYtXKSQoDlpYL7puSgyqS6nWnNQDcHikaCGF32p4h44wyfSyQWSmSF0WIv68LIHTmUv9PykZDc3BAmYeHLaf9WTItTha9GD8O3i89hhgl8";
            // ZavodClient.ZavodClient.Client.DefaultRequestHeaders.Add("token", token);
            var userClient = new ZavodClient.ZavodClient("http://localhost:5000").User;
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(typeof(Startup)); }).Build().RunAsync();
            var a  = await userClient.GetNewAccessToken("1//0cEAPecdhSYvCCgYIARAAGAwSNwF-L9Irkwt8nZYlYX5RDE6Yq_nqdkmlQk9opHaqFI3UddInGRQ8ay-YUhcsHjCucqpsWJ5lNCE");
            
            allUnits = await client.GetAll();
            defaultUnits = await client.GetAllDefaultUnits();
        }
        
        [Test]
        public async Task CreateUnitTest()
        {
            var result = await client.CreateUnit(new CreateUnitDto{UnitType = UnitType.Warrior, Position = new Vector3{X =15, Y = 25,Z = 10}});
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
                answer.Id.Should().Be(unit.Id);
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

        [Test]
        public async Task UpdateNotExistTest()
        {
            foreach (var unit in allUnits)
            {
                unit.Id = Guid.NewGuid();
                unit.AttackDamage++;
                unit.CurrentHp -= 10;
                Func<Task> updatedUnit = async () => await client.UpdateUnit(unit);
                await updatedUnit.Should().ThrowAsync<HttpRequestException>();
            }
        }
        
        [Test]
        public async Task CreateAllDefaultUnitsTest()
        {
            foreach(var unit in defaultUnits)
            {
                var result = await client.CreateUnit(new CreateUnitDto{UnitType = unit.Type, Position = unit.UnitDto.Position});
                unit.UnitDto.Id = result.Id;
                result.Should().BeEquivalentTo(unit.UnitDto);
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
        public async Task DeleteNotExistUnitTest()
        {
            Func<Task> result = async () => await client.DeleteUnit(Guid.NewGuid());
            await result.Should().ThrowAsync<HttpRequestException>();
        }
        
        [Test]
        public async Task DistanceBetweenOneUnitTest()
        {
            foreach(var unit in allUnits)
            {
                var result = await client.GetDistanceById(unit.Id, unit.Id);
                result.Should().Be(0);
            }
        }
        
        [Test]
        public async Task DistanceBetweenUnitAndNotExistUnitTest()
        {
            foreach(var unit in allUnits)
            {
                Func<Task> result = async () => await client.GetDistanceById(unit.Id, Guid.NewGuid());
                await result.Should().ThrowAsync<HttpRequestException>();
            }
        }

        [Test]
        public async Task ZeroUnitsAttacksTest()
        {
            var result = await client.SendAttackUnits();
            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task UnitAttacksHimselfTest()
        {
            var unit = allUnits.First(x => x.CurrentHp > 0);
            client.AddUnitsToAttack(unit.Id, unit.Id);
            var result = await client.SendAttackUnits();
            var expectedResult = new ResultOfAttackDto {Id = unit.Id, Flag = true, Hp = unit.CurrentHp - unit.AttackDamage};
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task UnitWithZeroHpTryToAttackTest()
        {
            var newUnit = await client.CreateUnit(new CreateUnitDto
                {UnitType = UnitType.Warrior, Position = new Vector3(0, 0, 0)});
            newUnit.CurrentHp = 0;
            newUnit = await client.UpdateUnit(newUnit);
            client.AddUnitsToAttack(newUnit.Id, newUnit.Id);
            var result = await client.SendAttackUnits();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task ZeroUnitMovesTest()
        {
            var result = await client.SendMoveUnits();
            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task UnitMoveToNormalDistanceTest()
        {
            foreach (var unit in allUnits)
            {
                var newPosition = new Vector3(unit.Position.X + 1, unit.Position.Y + 1, unit.Position.Z + 1);
                client.AddUnitsToMove(unit.Id, newPosition);
                var result = await client.SendMoveUnits();
                result.Should().BeEmpty();
            }
        }   
        
        [Test]
        public async Task UnitMoveToLargeDistanceTest()
        {
            
            var unit = (await client.GetAll()).First(x => x.CurrentHp > 0);
            var newPosition = new Vector3(unit.Position.X + 10, unit.Position.Y + 10, unit.Position.Z + 10);
            client.AddUnitsToMove(unit.Id, newPosition);
            var result = await client.SendMoveUnits();
            result.Should().BeEquivalentTo(new MoveUnitDto{Id = unit.Id, NewPosition = unit.Position});
        }
        
        [Test]
        public async Task UnitWithZeroHpTryToMoveTest()
        {
            var newUnit = await client.CreateUnit(new CreateUnitDto
                {UnitType = UnitType.Warrior, Position = new Vector3(0, 0, 0)});
            newUnit.CurrentHp = 0;
            newUnit = await client.UpdateUnit(newUnit);
            client.AddUnitsToMove(newUnit.Id, new Vector3(1,2,3));
            var result = await client.SendMoveUnits();
            result.Should().BeEmpty();
        }
    }
}