﻿using System.Collections.Generic;
using MainGame.Basic;
using MainGame.Network.Event;
using MainGame.UnityView.UI.Inventory.View;
using VContainer.Unity;

namespace MainGame.GameLogic.Inventory
{
    //IInitializableがないとDIコンテナ作成時にインスタンスが生成されないので実装しておく
    public class PlayerInventoryDataCache : IInitializable
    {
        private readonly PlayerInventoryItemView _playerInventoryItemView;
        private List<ItemStack> _items = new(new ItemStack[PlayerInventoryConstant.MainInventorySize]);
        
        public PlayerInventoryDataCache(PlayerInventoryUpdateEvent playerInventoryUpdateEvent,PlayerInventoryItemView playerInventoryItemView)
        {
            playerInventoryUpdateEvent.Subscribe(UpdateInventory,UpdateSlotInventory);
            _playerInventoryItemView = playerInventoryItemView;
        }

        public void UpdateInventory(OnPlayerInventoryUpdateProperties properties)
        {
            _items = properties.ItemStacks;
            //イベントの発火
            for (int i = 0; i < _items.Count; i++)
            {
                //viewのUIにインベントリが更新されたことを通知する
                _playerInventoryItemView.OnInventoryUpdate(i,_items[i].ID,_items[i].Count);
            }
        }

        public void UpdateSlotInventory(OnPlayerInventorySlotUpdateProperties properties)
        {
            var s = properties.SlotId;
            _items[s] = properties.ItemStack;
            //イベントの発火
            
            //viewのUIにインベントリが更新されたことを通知する
            _playerInventoryItemView.OnInventoryUpdate(s,_items[s].ID,_items[s].Count);
        }
        
        public ItemStack GetItemStack(int slot)
        {
            return _items[slot];
        }

        public void Initialize() { }
    }
}