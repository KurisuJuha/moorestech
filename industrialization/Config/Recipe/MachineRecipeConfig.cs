﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using industrialization.Config.Recipe.Data;
using industrialization.Config.Recipe.Json;
using industrialization.Item;

namespace industrialization.Config.Recipe
{
    public static class MachineRecipeConfig
    {
        
        private static IMachineRecipeData[] _recipedatas;

        public static IMachineRecipeData GetRecipeData(int id)
        {
            _recipedatas ??= MachineRecipeJsonLoad.LoadConfig();
            return _recipedatas[id];
        }

        private static Dictionary<string, IMachineRecipeData> _recipeDataCash;
        public static IMachineRecipeData GetRecipeData(int installationId, List<IItemStack> iunputItem)
        {
            _recipedatas ??= MachineRecipeJsonLoad.LoadConfig();

            if (_recipeDataCash == null)
            {
                _recipeDataCash = new Dictionary<string, IMachineRecipeData>();
                _recipedatas.ToList().ForEach(recipe =>
                {
                    _recipeDataCash.Add(
                        GetKey(recipe.InstallationId,recipe.ItemInputs.ToList()),
                        recipe);
                });
            }

            var key = GetKey(installationId, iunputItem);
            if (_recipeDataCash.ContainsKey(key))
            {
                return _recipeDataCash[key];
            }
            else
            {
                return new NullMachineRecipeData();
            }
        }
        private static string GetKey(int installationId, List<IItemStack> itemId)
        {
            var items = "";
            itemId = itemId.OrderBy(i => i.Id).ToList();
            itemId.ForEach(i =>
            {
                items = items + "_" + i.Id;
            });
            return installationId + items;
        }
    }
}