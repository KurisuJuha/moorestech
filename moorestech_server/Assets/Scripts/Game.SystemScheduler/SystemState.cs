namespace Game.SystemScheduler
{
    public readonly struct SystemState
    {
        public readonly Arch.Core.World World;

        public SystemState(Arch.Core.World world)
        {
            World = world;
        }
    }
}
