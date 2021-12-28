﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Block.BlockFactory;
using Core.Block.BlockInventory;
using Core.Block.Machine;
using Core.Block.RecipeConfig;
using Core.Electric;
using Core.Item;
using Core.Item.Config;
using Core.Update;
using Core.Util;
using NUnit.Framework;
using Test.CombinedTest.Core.Generate;
using Test.Module;
using Test.Module.TestConfig;
using Test.Util;

namespace Test.CombinedTest.Core
{
    public class MachineIoTest
    {
        private readonly ItemStackFactory _itemStackFactory = new(new TestItemConfig());
        private BlockFactory _blockFactory;
        private NormalMachine CreateMachine(int id)
        {
            if (_blockFactory == null)
            {
                _blockFactory = new BlockFactory(new AllMachineBlockConfig(),new VanillaIBlockTemplates(new TestMachineRecipeConfig(_itemStackFactory),_itemStackFactory));
            }
            var machine = _blockFactory.Create(id, IntId.NewIntId()) as NormalMachine;
            return machine;
        }
        private NormalMachine CreateMachine(int id,IBlockInventory inventory)
        {
            var machine = CreateMachine(id);
            machine.AddConnector(inventory);
            
            return machine;
        }
        
        
        [TestCase(new int[1]{1}, new int[1]{1})]
        [TestCase(new int[2]{100,101}, new int[2]{10,10})]
        [TestCase(new int[3]{10,11,15}, new int[3]{10,5,8})]
        public void MachineInputTest(int[] id,int[] count)
        {
            var machine = CreateMachine(4);
            var items = new List<IItemStack>();
            for (int i = 0; i < id.Length; i++)
            {
                items.Add(_itemStackFactory.Create(id[i], count[i]));
            }

            foreach (var item in items)
            {
                machine.InsertItem(item);
            }
            
            
            var loadNormalMachineRunProcess = (NormalMachineRunProcess)machineType.GetField("_normalMachineRunProcess",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(loadMachine);
            Assert.AreEqual((double)300,(double)loadNormalMachineRunProcess.RemainingMillSecond);
            
            Assert.AreEqual(items.ToArray(), machine.InputSlotWithoutEmptyItemStack.ToArray());   
        }

        [TestCase( new int[3] {0, 5, 1}, new int[3] {10, 5, 8})]
        [TestCase( new int[2] {1, 0}, new int[2] {10, 5})]
        [TestCase( new int[2] {0, 0}, new int[2] {10, 5})]
        public void MachineInputNullItemTest(int[] id,int[] count)
        {
            var machine = CreateMachine(4);
            var items = new List<IItemStack>();
            for (int i = 0; i < id.Length; i++)
            {
                items.Add(_itemStackFactory.Create(id[i], count[i]));
            }

            foreach (var item in items)
            {
                machine.InsertItem(item);
            }
        }
        
        [TestCase(new int[2]{1,1}, new int[2]{1,1}, new int[1]{1}, new int[1]{2})]
        [TestCase(new int[2]{3,1}, new int[2]{1,1}, new int[2]{1,3}, new int[2]{1,1})]
        public void MachineAddInputTest(int[] id,int[] count,int[] ansid,int[] anscount)
        {
            
            var machine = CreateMachine(4);
            
            for (int i = 0; i < id.Length; i++)
            {
                machine.InsertItem(_itemStackFactory.Create(id[i], count[i]));
            }

            var ansItem = new List<IItemStack>();
            for (int i = 0; i < ansid.Length; i++)
            {
                ansItem.Add(_itemStackFactory.Create(ansid[i],anscount[i]));
            }

            for (int i = 0; i < ansItem.Count; i++)
            {
                var m = machine.InputSlotWithoutEmptyItemStack;
                Console.WriteLine(m[i].ToString()+" "+ansItem[i].ToString());
                Assert.AreEqual(ansItem[i],m[i]);
            }
            
        }
        
        //アイテムが通常通り処理されるかのテスト
        [Test]
        public void ItemProcessingAndChangeConnectorTest()
        {
            int seed = 2119350917;
            int recipeNum = 20;
            var recipes = MachineIoGenerate.MachineIoTestCase(RecipeGenerate.MakeRecipe(seed,recipeNum), seed);
            
            
            var machineList = new List<NormalMachine>();
            var MaxDateTime = DateTime.Now;
            
            //機械の作成とアイテムの挿入
            foreach (var m in recipes)
            {
                var machine = CreateMachine(m.installtionId);
                

                foreach (var minput in m.input)
                {
                    machine.InsertItem(_itemStackFactory.Create(minput.Id,minput.Count));
                }

                var electrical = new ElectricSegment();
                electrical.AddBlockElectric(machine);
                electrical.AddGenerator(new TestPowerGenerator(1000,0));
                
                machineList.Add(machine);
                
                DateTime endTime = DateTime.Now.AddMilliseconds(m.time*m.CraftCnt);
                if (endTime.CompareTo(MaxDateTime) == 1)
                {
                    MaxDateTime = endTime;
                }
            }
            
            //最大クラフト時間を超過するまでクラフトする
            while (MaxDateTime.AddSeconds(0.2).CompareTo(DateTime.Now) == 1)
            {
                GameUpdate.Update();
            }
            
            //検証その1
            for (int i = 0; i < machineList.Count; i++)
            {
                Console.WriteLine(i);
                var machine = machineList[i];
                var machineIoTest = recipes[i];
                
                Assert.False(machine.OutputSlotWithoutEmptyItemStack.Count <= 0);

                for (int j = 0; j < machine.OutputSlotWithoutEmptyItemStack.Count; j++)
                {
                    Assert.AreEqual(machineIoTest.output[j],machine.OutputSlotWithoutEmptyItemStack[j]);
                }
                
                var inputRemainder = machineIoTest.inputRemainder.Where(i => i.Count != 0).ToList();
                inputRemainder.Sort((a, b) => a.Id - b.Id);
                for (int j = 0; j < machine.InputSlotWithoutEmptyItemStack.Count; j++)
                {
                    Assert.True(inputRemainder[j].Equals(machine.InputSlotWithoutEmptyItemStack[j]));
                }
            }
            
            var dummyBlockList = new List<DummyBlockInventory>();
            //コネクターを変える
            for (int i = 0; i < recipes.Length; i++)
            {
                var dummy = new DummyBlockInventory(recipes[i].output.Count);
                machineList[i].AddConnector(dummy);
                dummyBlockList.Add(dummy);
            }
            GameUpdate.Update();
            //検証その2
            for (int i = 0; i < machineList.Count; i++)
            {
                Console.WriteLine(i);
                var machine = machineList[i];
                var dummy = dummyBlockList[i];
                var machineIoTest = recipes[i];
                
                Assert.True(machine.OutputSlotWithoutEmptyItemStack.Count == 0);

                for (int j = 0; j < machineIoTest.output.Count; j++)
                {
                    Assert.True(machineIoTest.output[j].Equals(dummy.InsertedItems[j]));
                }
                
                var inputRemainder = machineIoTest.inputRemainder.Where(i => i.Count != 0).ToList();
                inputRemainder.Sort((a, b) => a.Id - b.Id);
                for (int j = 0; j < inputRemainder.Count; j++)
                {
                    Assert.True(inputRemainder[j].Equals(machine.InputSlotWithoutEmptyItemStack[j]));
                }
            }
        }
        
        //アイテムが通常通り処理されるかのテスト
        [Test]
        public void ItemProcessingOutputTest()
        {
            int seed = 2119350917;
            int recipeNum = 20;
            var recipes = MachineIoGenerate.MachineIoTestCase(RecipeGenerate.MakeRecipe(seed,recipeNum), seed);
            
            
            var machineList = new List<NormalMachine>();
            var dummyBlockList = new List<DummyBlockInventory>();
            var MaxDateTime = DateTime.Now;
            
            //機械の作成とアイテムの挿入
            foreach (var m in recipes)
            {
                var connect = new DummyBlockInventory(m.output.Count);
                var machine = CreateMachine(m.installtionId,connect);

                foreach (var minput in m.input)
                {
                    machine.InsertItem(_itemStackFactory.Create(minput.Id,minput.Count));
                }

                var electrical = new ElectricSegment();
                electrical.AddBlockElectric(machine);
                electrical.AddGenerator(new TestPowerGenerator(1000,0));
                
                dummyBlockList.Add(connect);
                machineList.Add(machine);
                
                DateTime endTime = DateTime.Now.AddMilliseconds(m.time*m.CraftCnt);
                if (endTime.CompareTo(MaxDateTime) == 1)
                {
                    MaxDateTime = endTime;
                }
            }
            
            //最大クラフト時間を超過するまでクラフトする
            while (MaxDateTime.AddSeconds(0.2).CompareTo(DateTime.Now) == 1)
            {
                GameUpdate.Update();
            }
            
            //検証
            for (int i = 0; i < machineList.Count; i++)
            {
                Console.WriteLine(i);
                var machine = machineList[i];
                var connect = dummyBlockList[i];
                var machineIoTest = recipes[i];
                
                var output = connect.InsertedItems;
                Assert.False(output.Count <= 0);

                for (int j = 0; j < output.Count; j++)
                {
                    Assert.True(machineIoTest.output[j].Equals(output[j]));
                }
                
                var inputRemainder = machineIoTest.inputRemainder.Where(i => i.Count != 0).ToList();
                inputRemainder.Sort((a, b) => a.Id - b.Id);
                for (int j = 0; j < machine.InputSlotWithoutEmptyItemStack.Count; j++)
                {
                    Assert.True(inputRemainder[j].Equals(machine.InputSlotWithoutEmptyItemStack[j]));
                }
            }
        }
    }
}