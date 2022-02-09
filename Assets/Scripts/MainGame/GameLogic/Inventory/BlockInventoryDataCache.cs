using System.Collections.Generic;
using MainGame.GameLogic.Event;
using MainGame.Network.Interface.Receive;
using Maingame.Types;
using UnityEngine;
using IBlockInventoryUpdateEvent = MainGame.UnityView.Interface.IBlockInventoryUpdateEvent;

namespace MainGame.GameLogic.Inventory
{
    public class BlockInventoryDataCache
    {
        private readonly BlockInventoryUpdateEvent _blockInventoryView;
        public BlockInventoryDataCache(IBlockInventoryUpdateEvent blockInventoryView,IReceiveBlockInventoryUpdateEvent blockInventory)
        {
            _blockInventoryView = blockInventoryView as BlockInventoryUpdateEvent;
            blockInventory.Subscribe(OnBlockInventorySlotUpdate,OnSettingBlockInventory);
        }

        private void OnBlockInventorySlotUpdate(Vector2Int pos,int slot,int id,int count)
        {
            _blockInventoryView.OnInventoryUpdateInvoke(slot,id,count);
        }

        private void OnSettingBlockInventory(List<ItemStack> items,string uiType,params short[] uiParams)
        {
            //UIを開く
            _blockInventoryView.OnSettingInventoryInvoke(uiType,uiParams);
            //UIを更新する
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                _blockInventoryView.OnInventoryUpdateInvoke(i,item.ID,item.Count);
            }
        }
        
    }
}