﻿using System.Collections.Generic;
using MainGame.Basic;
using MainGame.Model.Network.Event;
using MainGame.UnityView.UI.Inventory.Control;
using VContainer.Unity;

namespace MainGame.Presenter.Inventory
{
    //IInitializableがないとDIコンテナ作成時にインスタンスが生成されないので実装しておく
    public class MainInventoryDataCache : IInitializable
    {
        private readonly PlayerInventoryViewModelController _playerInventoryViewModel;
        
        public MainInventoryDataCache(MainInventoryUpdateEvent mainInventoryUpdateEvent,PlayerInventoryViewModelController playerInventoryViewModel)
        {
            _playerInventoryViewModel = playerInventoryViewModel;
            mainInventoryUpdateEvent.OnMainInventoryUpdateEvent +=UpdateInventory;
            mainInventoryUpdateEvent.OnMainInventorySlotUpdateEvent +=UpdateSlotInventory;
        }

        public void UpdateInventory(MainInventoryUpdateProperties properties)
        {
            for (int i = 0; i < properties.ItemStacks.Count; i++)
            {
                var id = properties.ItemStacks[i].ID;
                var count = properties.ItemStacks[i].Count;
                var slot = i;
                //View側を更新する
                _playerInventoryViewModel.SetItem(slot,id,count);
            }
        }

        public void UpdateSlotInventory(MainInventorySlotUpdateProperties properties)
        {
            
            //View側を更新する
            _playerInventoryViewModel.SetItem(properties.SlotId,properties.ItemStack.ID,properties.ItemStack.Count);
        }
        public void Initialize() { }
    }
}