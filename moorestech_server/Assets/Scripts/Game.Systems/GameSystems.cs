using Game.SystemScheduler;

namespace Game.Systems
{
    public sealed class GameSystems
    {
        public GameSystems(SystemUpdater updater)
        {
            updater.AddSystems(
                DestroyLogic.Update
            );
        }
    }
}
