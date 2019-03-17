using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CalculatorServer
{
    public class GameClient
    {
        private EndPoint endPoint;

        private Queue<Packet> sendQueue;

        private Dictionary<uint, Packet> ackTable;
        public uint ackTableCount { get { return (uint)ackTable.Count; } }

        private GameServer server;

        public GameClient(GameServer server, EndPoint endPoint)
        {
            this.server = server;
            this.endPoint = endPoint;
            sendQueue = new Queue<Packet>();
            ackTable = new Dictionary<uint, Packet>();
        }

        public void Process()
        {
            int packetsInQueue = sendQueue.Count;
            for (int i = 0; i < packetsInQueue; i++)
            {
                Packet packet = sendQueue.Dequeue();
                if (server.Send(packet, endPoint))
                {
                    if (packet.NeedAck)
                    {
                        ackTable[packet.Id] = packet;
                    }
                }
            }
        }

        public void Ack(uint packetId)
        {
            if (ackTable.ContainsKey(packetId))
            {
                ackTable.Remove(packetId);
            }
        }

        public void Enqueue(Packet packet)
        {
            sendQueue.Enqueue(packet);
        }

        public override string ToString()
        {
            return endPoint.ToString();
        }
    }
}