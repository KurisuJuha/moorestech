using Game.Save.Json;
using Game.World.Interface;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server;

namespace Test.UnitTest.Game
{
    public class AssembleSaveJsonTextTest
    {
        //何もデータがない時のテスト
        [Test]
        public void NoneTest()
        {
            var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var assembleSaveJsonText = serviceProvider.GetService<AssembleSaveJsonText>();
            var json = assembleSaveJsonText.AssembleSaveJson();
            Assert.AreEqual("{\"world\":[],\"inventory\":[]}",json);
        }
        
        [Test]
        public void BlockPlacedTest()
        {
            var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var assembleSaveJsonText = serviceProvider.GetService<AssembleSaveJsonText>();
            var worldBlockDatastore = serviceProvider.GetService<IWorldBlockDatastore>();
            
            //worldBlockDatastore.AddBlock()
            
            var json = assembleSaveJsonText.AssembleSaveJson();
            Assert.AreEqual("{\"world\":[],\"inventory\":[]}",json);
        }
        
    }
}