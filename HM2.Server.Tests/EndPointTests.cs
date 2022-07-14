using EndPointMessage;
using HM2.EndPoint;
using HM2.EndPoint.Commands;
using HM2.Games;
using HM2.GameSolve.Actions;
using HM2.GameSolve.Interfaces;
using HM2.GameSolve.Structures;
using HM2.IoCs;
using HM2.MovableObject;
using HM2.Threads;
using HM2.Threads.Commands;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HM2.Server.Tests
{
    public class EndPointTests
    {
        const string ipAddr = "127.0.0.1";
        const int port = 8080;

        [SetUp]
        public void StartServer()
        {
            EndPointNetServer endPointServer = new EndPointNetServer(ipAddr, port);
            //����������� ������� � IoC
            IoC<EndPointNetServer>.Resolve("IoC.Registration", "Server", endPointServer);
            //����� �������
            IoC<EndPointNetServer>.Resolve("Server").Run();
        }
        [SetUp]
        public void CreateGameObject()
        {
            Game game = new Game();
            //������� ��� ���� �� 10 ������� �������� � ������
            game.Create(3, 10);
        }
       [SetUp]
        public void CreateQueueAndRun()
        {
            //�������� � ����������� �������
            IoC<QueueThread>.Resolve("IoC.Registration", "Queue", new QueueThread());
            //�������� �������
            IoC<QueueThread>.Resolve("Queue").PushCommand(new ControlCommand(IoC<QueueThread>.Resolve("Queue").Start));
        }
        [TearDown]
        public void Down()
        {
            //��������� �������
            IoC<QueueThread>.Resolve("Queue").PushCommand(new ControlCommand(IoC<QueueThread>.Resolve("Queue").HardStop));
            //��������� ������
            IoC<EndPointNetServer>.Resolve("Server").Close();
        }


        [Test]
        public void MoveObjectByServer()
        {
            //�������� ������� ������ ��� ������� 3 �� ���� ����� 1. �� � ����� ��������� � ����.
            UObject obj = IoC<UObject>.Resolve($"game 1 object 3");
            Vector vector = new Vector();
            vector.Shift = new Coordinats { X = 5.0, Y = 7.0 };

            //��������� ��������� ��� �������
            Message message = new Message("1", "3", "Move line", JsonSerializer.Serialize<Vector>(vector));
            //����������� ��������� � ������
            StringBuilder serializedMessage = new StringBuilder();
            new SerializeMessageCommands(message, serializedMessage).Execute();

            //���������, ��� ������ �� ��������
            Assert.AreEqual(obj.CurrentVector.PositionNow.X, 0.0);
            Assert.AreEqual(obj.CurrentVector.PositionNow.Y, 0.0);

            //���������� ��������� ������� 
            CreateClientAndSendMessage(serializedMessage.ToString());
            //���������� ����
            Thread.Sleep(300);

            //��������� ��� ������ ������� ���� ���������
            Assert.AreEqual(obj.CurrentVector.PositionNow.X, 5.0);
            Assert.AreEqual(obj.CurrentVector.PositionNow.Y, 7.0);
        }


        void CreateClientAndSendMessage(string msg)
        {
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddr), port);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var data = Encoding.UTF8.GetBytes(msg);

            tcpSocket.Connect(tcpEndPoint);
            tcpSocket.Send(data);

            var buffer = new byte[2048];
            var size = 0;
            var answer = new StringBuilder();

            do
            {
                size = tcpSocket.Receive(buffer);
                answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (tcpSocket.Available > 0);

            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();
        }
    }
}