using System;
using System.Collections.Generic;
using NUnit.Framework;
using Server.PacketHandle;
using Server.Util;

namespace Test.CombinedTest.Server.PacketTest.Event
{
    public class BlockPlaceEventPacketTest
    {
        //ブロックを設置しなかった時何も返ってこないテスト
        [Test]
        public void DontBlockPlaceTest()
        {
            var response = PacketResponseCreator.GetPacketResponse(EventRequestData(0));
            Assert.AreEqual(response.Count,0);
        }
        
        //ブロックを0個以上設置した時にブロック設置イベントが返ってくるテスト
        [Test]
        public void OneBlockPlaceEvent()
        {
            var random = new Random(1410);
            for (int i = 0; i < 100; i++)
            {
                var blocks = new List<Block>();
                var cnt = random.Next(0, 20);
                for (int j = 0; j < cnt; j++)
                {
                    var x = random.Next(-10000, 10000);
                    var y = random.Next(-10000, 10000);
                    var id = random.Next(1, 1000);
                    blocks.Add(new Block(x,y,id));
                    BlockPlace(x, y, id);
                }
                
                var response = PacketResponseCreator.GetPacketResponse(EventRequestData(0));
                Assert.AreEqual(response.Count,cnt);
                foreach (var r in response)
                {
                    var b = AnalysisResponsePacket(r);
                    for (int j = 0; j < blocks.Count; j++)
                    {
                        if (b.Equals(blocks[i]))
                        {
                            blocks.RemoveAt(i);
                            j = 0;
                        }
                    }
                }
                Assert.AreEqual(0,blocks.Count);
                
                //パケットを送ったので次は何も返ってこない
                response = PacketResponseCreator.GetPacketResponse(EventRequestData(0));
                Assert.AreEqual(0,response.Count);
            }
        }
        
        Block AnalysisResponsePacket(byte[] payload)
        {
            var b = new ByteArrayEnumerator(payload);
            b.MoveNextToGetShort();
            b.MoveNextToGetShort();
            var x = b.MoveNextToGetInt();
            var y = b.MoveNextToGetInt();
            var id = b.MoveNextToGetInt();
            return new Block(x,y,id);
        }
        void BlockPlace(int x,int y,int id)
        {
            var bytes = new List<byte>();
            bytes.AddRange(ByteArrayConverter.ToByteArray((short)1));
            bytes.AddRange(ByteArrayConverter.ToByteArray(id));
            bytes.AddRange(ByteArrayConverter.ToByteArray((short)0));
            bytes.AddRange(ByteArrayConverter.ToByteArray(x));
            bytes.AddRange(ByteArrayConverter.ToByteArray(y));
            bytes.AddRange(ByteArrayConverter.ToByteArray(0));
            bytes.AddRange(ByteArrayConverter.ToByteArray(0));
            PacketResponseCreator.GetPacketResponse(bytes.ToArray());
        }

        byte[] EventRequestData(int plyaerID)
        {
            var payload = new List<byte>();
            payload.AddRange(ByteArrayConverter.ToByteArray((short)4));
            payload.AddRange(ByteArrayConverter.ToByteArray(plyaerID));
            return payload.ToArray();
        }

        class Block
        {
            public readonly int X;
            public readonly int Y;
            public readonly int id;

            public Block(int x, int y, int id)
            {
                X = x;
                Y = y;
                this.id = id;
            }

            public override bool Equals(object? obj)
            {
                if (obj == null) return false;
                if (obj is not Block) return false;
                var b = obj as Block;
                return 
                    b.id == id &&
                    b.X == X &&
                    b.Y == Y;
            }
        }
    }
}