using Core.Block.Config.Param;

namespace Core.Block.Config.LoadConfig
{
    public class BlockConfigData
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Type;
        public readonly BlockConfigParamBase Param;

        public BlockConfigData(int id, string name, string type, BlockConfigParamBase param)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.Param = param;
        }
    }
}