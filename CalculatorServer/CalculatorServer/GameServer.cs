using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace CalculatorServer
{
    public class DividedByZeroException : Exception
    {

    }

    public class GameServer
    {
        private delegate void GameCommand(byte[] data, EndPoint sender);
        private Dictionary<byte, GameCommand> commandsTable;
        private Dictionary<EndPoint, GameClient> clientsTable;
        private IGameTransport transport;

        public void Ack(byte[] data, EndPoint sender)
        {
            if (!clientsTable.ContainsKey(sender))
            {
                return;
            }
            GameClient client = clientsTable[sender];
            uint packetId = BitConverter.ToUInt32(data, 1);
            client.Ack(packetId);
        }

        public GameServer(IGameTransport transport)
        {
            this.transport = transport;
            clientsTable = new Dictionary<EndPoint, GameClient>();
            commandsTable = new Dictionary<byte, GameCommand>();
            commandsTable[0] = CalculateAddition;
            commandsTable[3] = CalculateSubtraction;
            commandsTable[6] = CalculateMultiplication;
            commandsTable[9] = CalculateDivision;
            commandsTable[12] = CalculateGreaterNumber;
            commandsTable[15] = CalculateLowerNumber;
            commandsTable[18] = CalculateIsEqual;

            commandsTable[255] = Ack;
        }


        private void CalculateAddition(byte[] data, EndPoint sender)
        {
            GameClient newClient = new GameClient(this, sender);
            clientsTable[sender] = newClient;

            float firstValue = BitConverter.ToSingle(data, 1);
            float secondValue = BitConverter.ToSingle(data, 5);
            float sum = firstValue + secondValue;
            Console.WriteLine("{0} + {1} = {2}", firstValue, secondValue, sum);
            Packet sumResult = new Packet(0, sum);
            clientsTable[sender].Enqueue(sumResult);
        }
        private void CalculateSubtraction(byte[] data, EndPoint sender)
        {
            GameClient newClient = new GameClient(this, sender);
            clientsTable[sender] = newClient;

            float firstValue = BitConverter.ToSingle(data, 1);
            float secondValue = BitConverter.ToSingle(data, 5);
            float difference = firstValue - secondValue;
            Console.WriteLine("{0} - {1} = {2}", firstValue, secondValue, difference);
            Packet differenceResult = new Packet(0, difference);
            clientsTable[sender].Enqueue(differenceResult);
        }
        private void CalculateMultiplication(byte[] data, EndPoint sender)
        {
            GameClient newClient = new GameClient(this, sender);
            clientsTable[sender] = newClient;

            float firstValue = BitConverter.ToSingle(data, 1);
            float secondValue = BitConverter.ToSingle(data, 5);
            float product = firstValue * secondValue;
            Console.WriteLine("{0} * {1} = {2}", firstValue, secondValue, product);
            Packet productResult = new Packet(0, product);
            clientsTable[sender].Enqueue(productResult);
        }
        private void CalculateDivision(byte[] data, EndPoint sender)
        {
            GameClient newClient = new GameClient(this, sender);
            clientsTable[sender] = newClient;

            float firstValue = BitConverter.ToSingle(data, 1);
            float secondValue = BitConverter.ToSingle(data, 5);
            if(secondValue == 0)
                throw new DividedByZeroException();

            float division = firstValue / secondValue;
            Console.WriteLine("{0} / {1} = {2}", firstValue, secondValue, division);
            Packet differenceResult = new Packet(0, division);
            clientsTable[sender].Enqueue(differenceResult);
        }
        private void CalculateGreaterNumber(byte[] data, EndPoint sender)
        {
            GameClient newClient = new GameClient(this, sender);
            clientsTable[sender] = newClient;

            float firstValue = BitConverter.ToSingle(data, 1);
            float secondValue = BitConverter.ToSingle(data, 5);
            float greaterValue = firstValue > secondValue ? firstValue : secondValue;
            Console.WriteLine("{0} and {1}, {2} is the greater value", firstValue, secondValue, greaterValue);

            Packet greaterNumberResult = new Packet(0, greaterValue);
            clientsTable[sender].Enqueue(greaterNumberResult);
        }
        private void CalculateLowerNumber(byte[] data, EndPoint sender)
        {
            GameClient newClient = new GameClient(this, sender);
            clientsTable[sender] = newClient;

            float firstValue = BitConverter.ToSingle(data, 1);
            float secondValue = BitConverter.ToSingle(data, 5);
            float lowerValue = firstValue < secondValue ? firstValue : secondValue;
            Console.WriteLine("{0} and {1}, {2} is the lower value", firstValue, secondValue, lowerValue);

            Packet lowerNumberResult = new Packet(0, lowerValue);
            clientsTable[sender].Enqueue(lowerNumberResult);
        }
        private void CalculateIsEqual(byte[] data, EndPoint sender)
        {
            GameClient newClient = new GameClient(this, sender);
            clientsTable[sender] = newClient;

            float firstValue = BitConverter.ToSingle(data, 1);
            float secondValue = BitConverter.ToSingle(data, 5);
            bool areValuesEqual = firstValue == secondValue;
            if(areValuesEqual)
                Console.WriteLine("{0} and {1} are equal", firstValue, secondValue);
            else if (!areValuesEqual)
                Console.WriteLine("{0} and {1} are different values", firstValue, secondValue);

            Packet equalResult = new Packet(0, areValuesEqual);
            clientsTable[sender].Enqueue(equalResult);
        }

        public void Start()
        {
            Console.WriteLine("server started");
            while (true)
            {
                SingleStep();
            }
        }

        public void SingleStep()
        {
            EndPoint sender = transport.CreateEndPoint();
            byte[] data = transport.Recv(256, ref sender);
            if (data != null)
            {
                byte gameCommand = data[0];
                if (commandsTable.ContainsKey(gameCommand))
                {
                    commandsTable[gameCommand](data, sender);
                }
            }
            foreach (GameClient client in clientsTable.Values)
            {
                client.Process();
            }
        }

        public bool Send(Packet packet, EndPoint endPoint)
        {
            return transport.Send(packet.GetData(), endPoint);
        }

        public GameClient GetClientFromEndPoint(EndPoint endPoint)
        {
            if (clientsTable.ContainsKey(endPoint))
            {
                return clientsTable[endPoint];
            }

            return null;
        }
    }
}