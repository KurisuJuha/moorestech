using Core.Update;
using Game.World.Interface.DataStore;
using UniRx;
using UnityEngine;

namespace Game.World.DataStore
{
    public class ECSWorld : IECSWorld
    {
        public Arch.Core.World World { get; }

        public ECSWorld(Arch.Core.World world)
        {
            World = world;
            GameUpdater.ECSWorldUpdateObservable.Subscribe(_ => Update());
        }

        private void Update()
        {
            Debug.Log("ECS World Update");
        }
    }
}
