﻿using System.Collections.Generic;
using MainGame.Network.Interface;
using MainGame.Network.Interface.Send;
using MainGame.Network.Util;

namespace MainGame.Network.Send
{
    public class RequestPlayerInventoryProtocol : IRequestPlayerInventoryProtocol
    {
        private const short ProtocolId = 4;
        private readonly ISocket _socket;

        public RequestPlayerInventoryProtocol(ISocket socket)
        {
            _socket = socket;
        }
        
        public void Send(int playerId)
        {
            var packet = new List<byte>();
            
            packet.AddRange(ToByteList.Convert(ProtocolId));
            packet.AddRange(ToByteList.Convert(playerId));
        }
    }
}