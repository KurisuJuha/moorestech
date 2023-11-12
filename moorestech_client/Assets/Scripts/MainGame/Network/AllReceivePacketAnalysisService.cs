﻿using System.Collections.Generic;
using MainGame.Network.Event;
using MainGame.Network.Receive;
using MessagePack;
using Server.Event.EventReceive;
using Server.Protocol;
using Server.Protocol.PacketResponse;
using UnityEngine;

namespace MainGame.Network
{
    public class AllReceivePacketAnalysisService
    {
        private readonly Dictionary<string, IAnalysisPacket> _analysisPackets = new();
        private int _packetCount;


        public AllReceivePacketAnalysisService(
            ReceiveChunkDataEvent receiveChunkDataEvent, ReceiveMainInventoryEvent receiveMainInventoryEvent, ReceiveCraftingInventoryEvent receiveCraftingInventoryEvent, ReceiveBlockInventoryEvent receiveBlockInventoryEvent, ReceiveGrabInventoryEvent receiveGrabInventoryEvent, ReceiveInitialHandshakeProtocol receiveInitialHandshakeProtocol, ReceiveQuestDataEvent receiveQuestDataEvent, ReceiveEntitiesDataEvent receiveEntitiesDataEvent, ReceiveBlockStateChangeEvent receiveBlockStateChangeEvent, ReceiveUpdateMapObjectEvent receiveUpdateMapObjectEvent)
        {
            _analysisPackets.Add(DummyProtocol.Tag, new ReceiveDummyProtocol());
            _analysisPackets.Add(InitialHandshakeProtocol.Tag, receiveInitialHandshakeProtocol);
            _analysisPackets.Add(PlayerCoordinateSendProtocol.ChunkDataTag, new ReceiveChunkDataProtocol(receiveChunkDataEvent));
            _analysisPackets.Add(PlayerCoordinateSendProtocol.EntityDataTag, new ReceiveEntitiesProtocol(receiveEntitiesDataEvent));
            _analysisPackets.Add(EventProtocolMessagePackBase.EventProtocolTag, new ReceiveEventProtocol(receiveChunkDataEvent, receiveMainInventoryEvent, receiveCraftingInventoryEvent, receiveBlockInventoryEvent, receiveGrabInventoryEvent, receiveBlockStateChangeEvent, receiveUpdateMapObjectEvent));
            _analysisPackets.Add(PlayerInventoryResponseProtocol.Tag, new ReceivePlayerInventoryProtocol(receiveMainInventoryEvent, receiveCraftingInventoryEvent, receiveGrabInventoryEvent));
            _analysisPackets.Add(BlockInventoryRequestProtocol.Tag, new ReceiveBlockInventoryProtocol(receiveBlockInventoryEvent));
            _analysisPackets.Add(QuestProgressRequestProtocol.Tag, new ReceiveQuestProgressProtocol(receiveQuestDataEvent));
            _analysisPackets.Add(MapObjectDestructionInformationProtocol.Tag, new ReceiveMapObjectDestructionInformationProtocol(receiveUpdateMapObjectEvent));
        }

        public void Analysis(List<byte> packet)
        {
            var tag = MessagePackSerializer.Deserialize<ProtocolMessagePackBase>(packet.ToArray()).Tag;

            //receive debug
            _packetCount++;
            if (!_analysisPackets.TryGetValue(tag, out var analyser))
            {
                Debug.LogError("Count " + _packetCount + " NotFoundTag " + tag);
                return;
            }


            //analysis packet
            analyser.Analysis(packet);
        }
    }
}