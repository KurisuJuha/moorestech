using Core.Block;
using Core.Block.BlockInventory;

namespace Game.World.Interface
{
    
    /// <summary>
    /// TODO IBlockInventoryの管理を他のクラスがするようにする
    /// </summary>
    public interface IWorldBlockDatastore
    {
        public bool AddBlock(IBlock block, int x, int y);
        public IBlock GetBlock(int x, int y);
    }
}