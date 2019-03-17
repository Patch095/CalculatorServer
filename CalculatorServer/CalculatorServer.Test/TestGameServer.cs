using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CalculatorServer.Test
{
    public class TestGameServer
    {
        public TestGameServer()
        {
        }

        private FakeTransport transport;
        private GameServer server;

        //Called before every [Test]
        [SetUp]
        public void SetUp()
        {
            transport = new FakeTransport();
            server = new GameServer(transport);
        }

        //sum
        [Test]
        public void AdditionTestGreenLight()
        {
            Packet numbers = new Packet(0, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "sumNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.EqualTo(5.0f));
        }
        [Test]
        public void AdditionTestRedLight()
        {
            Packet numbers = new Packet(0, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "sumNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.Not.EqualTo(3.0f));
        }

        //sub
        [Test]
        public void SubtractionTestGreenLight()
        {
            Packet numbers = new Packet(3, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "subNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.EqualTo(1.0f));
        }
        [Test]
        public void SubtractionTestRedLight()
        {
            Packet numbers = new Packet(3, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "subNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.Not.EqualTo(3.0f));
        }

        //product
        [Test]
        public void MultiplicationTestGreenLight()
        {
            Packet numbers = new Packet(6, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "productNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.EqualTo(6.0f));
        }
        [Test]
        public void MultiplicationTestRedLight()
        {
            Packet numbers = new Packet(6, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "productNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.Not.EqualTo(3.0f));
        }

        //difference
        [Test]
        public void DivisionTestGreenLight()
        {
            Packet numbers = new Packet(9, 6.0f, 2.0f);
            transport.ClientEnqueue(numbers, "diffNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.EqualTo(3.0f));
        }
        [Test]
        public void DivisionTestRedLight()
        {
            Packet numbers = new Packet(9, 6.0f, 2.0f);
            transport.ClientEnqueue(numbers, "diffNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.Not.EqualTo(6.0f));
        }
        [Test]
        public void DivisionZeroExceptionTestRedLight()
        {
            Packet numbers = new Packet(9, 6.0f, 0.0f);
            transport.ClientEnqueue(numbers, "numbers", 0);
            //server.SingleStep();

            //FakeData result = transport.ClientDequeue();
            Assert.That(() => server.SingleStep(), Throws.InstanceOf<DividedByZeroException>());
        }

        //greater
        [Test]
        public void GetGreaterValueTestGreenLight()
        {
            Packet numbers = new Packet(12, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "confrontGreaterNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.EqualTo(3.0f));
        }
        [Test]
        public void GetGreaterValueTestRedLight()
        {
            Packet numbers = new Packet(12, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "confrontGreaterNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.Not.EqualTo(2.0f));
        }

        //lower
        [Test]
        public void GetLowerValueTestGreenLight()
        {
            Packet numbers = new Packet(15, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "confrontLowerNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.EqualTo(2.0f));
        }
        [Test]
        public void GetLowerValueTestRedLight()
        {
            Packet numbers = new Packet(15, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "confrontLowerNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            float resultValue = BitConverter.ToSingle(result.data, 1);
            Assert.That(resultValue, Is.Not.EqualTo(3.0f));
        }

        //equal
        [Test]
        public void EqualValuesTestGreenLight()
        {
            Packet numbers = new Packet(18, 3.0f, 3.0f);
            transport.ClientEnqueue(numbers, "confrontEqualsNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            bool resultValue = BitConverter.ToBoolean(result.data, 1);
            Assert.That(resultValue, Is.EqualTo(true));
        }
        [Test]
        public void EqualValuesTestRedLight()
        {
            Packet numbers = new Packet(18, 3.0f, 2.0f);
            transport.ClientEnqueue(numbers, "confrontEqualsNumbers", 0);
            server.SingleStep();

            FakeData result = transport.ClientDequeue();
            bool resultValue = BitConverter.ToBoolean(result.data, 1);
            Assert.That(resultValue, Is.Not.EqualTo(true));
        }
    }
}

//Move test debuged
//Test easier to read with less code line and variables removal with their values