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
        [OneTimeSetUp]
        public async Task StartServer()
        {
            client = new ZavodClient.ZavodClient("http://localhost:5000").Unit;
            var myCookie =
                $"CfDJ8IGASaB2OJhNiYicgRiEXj9OVzNUiB1uylSFkk7VtI-Ev_SnDFXgqK21YWQQq0P81hVzLCS2AitTt2EengvtXXhB_36RLOkuaFnxDSYQvjDn3UynmMPgtUWFTjhdh-4EznhjWsvcjCZI1e1BvC-SCZgwl47V9pA9_80LT6s3_W-OAl3VkzaCrCBwstgcRjMwJyccZ8HcPxzaOQUDAJ2DzLryVAjYhNQ_KgfYq_HtwsvQElofOQAXKWzAdfJxLzgVyxdeDLpRUmmgVvkFNOD7KMY2bYHcQebkMStF0Dd1Fzbz4P0-2FBncuH2HkkgSMP_WL2y8gJrYZJ4ad28ayPuVG2oCMcD_veev4Nk_cgbbX_s3aelPjhHorFCYfO6DR4I4e8YyDgjX_QjmHxtTdB94AEhb49RX202detEdelqGS0Yr12iEZzXSEMJcSIBIqLCyz9NFGM2V0VO879Vmd1OJiObyc-LUTT4b6hidpmLLWjTTlMGtrVJro9Dkrbn9uJptEJLG2baGKaRe6AW4s28tn2sj3VR0nP62cIOy2n5yd0_-E4COpN2BwAsM2Mpc1bwh_RuVSzSB340_RcWx3pMuQNbGVSnjFO7RWG492QE411CjFK1jyR1gFSEsH2yMax7TlYIApljmiBcIi1V2IEfMT-U-QJuiGwiWOQ_VqZi_IWJu1qdS1XtysvyB1QpDJExFOFzFoTN9ddaiNOAC-Z5RBh56PWTJfpuJOUZ42RWBt1ZNapthH-r7i0vQruDrh63NSIEUBIdtCDY52PVZs1R7cbBFLzUI39GWx-8tXtlSKlcvrbANwf_QlVbPhqopR-cX3RXKsPEQgtYNsiFx76B6vu2QgWmOdNkOjkYJGmQwgle20cxYByr-ELBHaiqRw0jVQ";
            ZavodClient.ZavodClient.Client.DefaultRequestHeaders.Add("Cookie", ".AspNetCore.Cookies="+myCookie);
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(typeof(Startup)); }).Build().RunAsync();
            allUnits = await client.GetAll();
        }
        
        [Test]
        public async Task CreateUnits()
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