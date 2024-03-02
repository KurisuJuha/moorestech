using System.Collections.Generic;
using Core.Update;
using Game.World.Interface.DataStore;
using UniRx;
using UnityEngine;

namespace Game.SystemScheduler
{
    public delegate void System(ref SystemState state);

    public sealed class SystemUpdater
    {
        private readonly IECSWorld _world;
        public readonly List<System> Systems = new();

        public SystemUpdater(IECSWorld world)
        {
            _world = world;
            GameUpdater.ECSWorldUpdateObservable.Subscribe(_ => Update());
        }

        private void Update()
        {
            Debug.Log($"Update SystemScheduler {_world}");

            var state = new SystemState();
            foreach (var system in Systems) system.Invoke(ref state);
        }

        public void AddSystems(params System[] systems)
        {
            Systems.AddRange(Systems);
        }
    }
}
