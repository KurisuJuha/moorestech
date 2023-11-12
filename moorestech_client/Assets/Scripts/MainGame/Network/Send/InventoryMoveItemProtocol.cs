using System.Linq;
using MainGame.Network.Settings;
using MessagePack;
using Server.Protocol.PacketResponse;
using Server.Protocol.PacketResponse.Util.InventoryMoveUtil;

namespace MainGame.Network.Send
{
    public class InventoryMoveItemProtocol
    {
        private readonly int _playerId;
        private readonly ISocketSender _socketSender;

        public InventoryMoveItemProtocol(PlayerConnectionSetting playerConnectionSetting, ISocketSender socketSender)
        {
            _socketSender = socketSender;
            _playerId = playerConnectionSetting.PlayerId;
        }

        public void Send(int count, ItemMoveType itemMoveType, FromItemMoveInventoryInfo fromInventory, ToItemMoveInventoryInfo toInventory)
        {
            _socketSender.Send(MessagePackSerializer.Serialize(new InventoryItemMoveProtocolMessagePack(
                _playerId, count, itemMoveType, fromInventory, toInventory)).ToList());
        }
    }
}