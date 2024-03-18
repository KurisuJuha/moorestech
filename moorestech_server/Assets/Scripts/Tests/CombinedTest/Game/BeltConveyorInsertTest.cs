using System;
using Core.Update;
using Game.Block.Base;
using Game.Block.Blocks.Chest;
using Game.Block.Interface;
using Game.World.Interface.DataStore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server.Boot;
using Tests.Module.TestMod;
using UnityEngine;

namespace Tests.CombinedTest.Game
{
    public class BeltConveyorInsertTest
    {
        //2つのアイテムがチェストから出されてベルトコンベアに入り、全てチェストに入るテスト
        [Test]
        public void TwoItemIoTest()
        {
            var (_, saveServiceProvider) = new MoorestechServerDiContainerGenerator().Create(TestModDirectory.ForUnitTestModDirectory);
            GameUpdater.ResetUpdate();
            
            var worldBlockDatastore = saveServiceProvider.GetService<IWorldBlockDatastore>();
            var blockFactory = saveServiceProvider.GetService<IBlockFactory>();

            var inputChest = (VanillaChest)blockFactory.Create(UnitTestModBlockId.ChestId, 1);
            var beltConveyor = blockFactory.Create(UnitTestModBlockId.BeltConveyorId, 2);
            var outputChest = (VanillaChest)blockFactory.Create(UnitTestModBlockId.ChestId, 3);

            //それぞれを設置
            worldBlockDatastore.AddBlock(inputChest, new Vector3Int(0, 0,0), BlockDirection.North);
            worldBlockDatastore.AddBlock(beltConveyor, new Vector3Int(0,0, 1), BlockDirection.North);
            worldBlockDatastore.AddBlock(outputChest, new Vector3Int(0, 0,2), BlockDirection.North);

            //インプットチェストにアイテムを2つ入れる
            inputChest.SetItem(0, 1, 2);

            //ベルトコンベアのアイテムが出てから入るまでの6秒間アップデートする
            var now = DateTime.Now;
            while (DateTime.Now - now < TimeSpan.FromSeconds(5)) GameUpdater.UpdateWithWait();

            //アイテムが出ているか確認
            Assert.AreEqual(0, inputChest.GetItem(0).Count);
            //アイテムが入っているか確認
            Assert.AreEqual(2, outputChest.GetItem(0).Count);
        }
    }
}