﻿using System.Collections.Generic;
using System.Linq;
using Core.Item;

namespace Core.Block.RecipeConfig.Data
{
    public class NullMachineRecipeData : IMachineRecipeData
    {
        public List<IItemStack> ItemInputs => System.Array.Empty<IItemStack>().ToList();
        public List<ItemOutput> ItemOutputs => System.Array.Empty<ItemOutput>().ToList();
        public int BlockId => BlockConst.BlockConst.NullBlockId;
        public int Time => 0;

        public bool RecipeConfirmation(List<IItemStack> inputSlot)
        {
            return false;
        }
    }
}