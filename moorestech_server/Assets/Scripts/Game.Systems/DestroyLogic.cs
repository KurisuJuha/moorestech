using Arch.Core;
using Game.SystemScheduler;

namespace Game.Systems
{
    public struct Destroy
    {
    }

    public static class DestroyLogic
    {
        public static void Update(ref SystemState state)
        {
            state.World.Destroy(new QueryDescription().WithAll<Destroy>());
        }
    }
}
