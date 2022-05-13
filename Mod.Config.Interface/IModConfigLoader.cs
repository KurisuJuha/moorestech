using System.Collections.Generic;
using Core.Block.Config.LoadConfig;
using Core.Block.RecipeConfig;
using Core.Item.Config;
using Game.Crafting.Config;

namespace Mod.Config.Interface
{
    public interface IModConfigLoader
    {
        LoadConfigContainer LoadModConfig(string modDirectory);
    }

    public class LoadConfigContainer
    {
        public readonly List<ItemConfigData> ItemConfigs;
        public readonly List<BlockConfigData> BlockConfigs;
        public readonly List<MachineRecipeConfig> MachineRecipeConfigs;
        public readonly List<CraftConfig> CraftConfigs;

        public LoadConfigContainer(List<ItemConfigData> itemConfigs, List<BlockConfigData> blockConfigs, List<MachineRecipeConfig> machineRecipeConfigs, List<CraftConfig> craftConfigs)
        {
            ItemConfigs = itemConfigs;
            BlockConfigs = blockConfigs;
            MachineRecipeConfigs = machineRecipeConfigs;
            CraftConfigs = craftConfigs;
        }
    }
}