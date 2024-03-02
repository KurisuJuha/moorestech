using Game.World.Interface.DataStore;

namespace Game.World.DataStore
{
    public class ECSWorld : IECSWorld
    {
        public Arch.Core.World World { get; }

        public ECSWorld(Arch.Core.World world)
        {
            World = world;
        }
    }
}
