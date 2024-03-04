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
        public readonly List<System> Systems = new();
        private SystemState _state;

        public SystemUpdater(IECSWorld world)
        {
            _state = new SystemState(world.World);
            GameUpdater.ECSWorldUpdateObservable.Subscribe(_ => Update());
        }

        private void Update()
        {
            Debug.Log("Update SystemScheduler");

            foreach (var system in Systems) system.Invoke(ref _state);
        }

        public void AddSystems(params System[] systems)
        {
            Systems.AddRange(Systems);
        }
    }
}
