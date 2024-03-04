using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Utils;
using Game.SystemScheduler;
using Game.World.Event;
using Game.World.Interface.DataStore;
using Game.World.Interface.Event;

namespace Game.Systems
{
    public sealed class GameSystems
    {
        private readonly Dictionary<int, EntityReference> _blockEntities = new();
        private readonly IECSWorld _world;

        public GameSystems(IECSWorld world, SystemUpdater updater, BlockPlaceEvent blockPlaceEvent,
            BlockRemoveEvent blockRemoveEvent)
        {
            _world = world;
            blockPlaceEvent.OnBlockPlaceEvent += OnBlockPlaceEvent;
            blockRemoveEvent.OnBlockRemoveEvent += OnBlockRemoveEvent;

            updater.AddSystems(
                DestroyLogic.Update
            );
        }

        private void OnBlockPlaceEvent(BlockPlaceEventProperties properties)
        {
            var block = properties.Block;
            var type = Component.GetComponentType(block.GetType());
            var entity = _world.World.Create(type);
            _world.World.Set(entity, block);
            _blockEntities[block.EntityId] = _world.World.Reference(entity);
        }

        private void OnBlockRemoveEvent(BlockRemoveEventProperties properties)
        {
            var blockEntity = _blockEntities[properties.Block.EntityId];
            _world.World.Add(blockEntity, new Destroy());
        }
    }
}
