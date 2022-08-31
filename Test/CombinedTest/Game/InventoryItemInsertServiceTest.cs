using Game.PlayerInventory.Interface;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server.Boot;
using Server.Protocol.PacketResponse.Util;
using Test.Module.TestMod;

namespace Test.CombinedTest.Game
{
    public class InventoryItemInsertServiceTest
    {
        /// <summary>
        /// 通常のinsert処理
        /// </summary>
        [Test]
        public void InsertTest()
        {
            var (_, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create(TestModDirectory.ForUnitTestModDirectory);
            
            var mainInventory = serviceProvider.GetService<IPlayerInventoryDataStore>().GetInventoryData(0).MainOpenableInventory;
            var craftInventory = serviceProvider.GetService<IPlayerInventoryDataStore>().GetInventoryData(0).CraftingOpenableInventory;
            
            //インベントリの設定
            mainInventory.SetItem(0,1,10);
            craftInventory.SetItem(0,1,10);
            craftInventory.SetItem(2,2,10);
            
            //クラフトからメインにid 1のアイテムを移す
            InventoryItemInsertService.Insert(craftInventory,0,mainInventory,5);
           
            Assert.AreEqual(15,mainInventory.GetItem(0).Count);
            Assert.AreEqual(5,craftInventory.GetItem(0).Count);
            
            
            //id 2のアイテムをクラフトからメインに移す
            InventoryItemInsertService.Insert(craftInventory,2,mainInventory,10);
            
            Assert.AreEqual(10,mainInventory.GetItem(1).Count);
            Assert.AreEqual(0,craftInventory.GetItem(2).Count);
        }


        /// <summary>
        /// アイテムがいっぱいの時はinsertされないテスト
        /// </summary>
        [Test]
        public void FullItemInsert()
        {
            
        }
    }
}