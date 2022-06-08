using EndPointMessage;
using HM2.EndPoint;
using HM2.EndPoint.Commands;
using HM2.GameSolve.Structures;
using HM2.MovableObject;
using HM2.Threads;
using HM2.Threads.Commands;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Controllers;
using WebServer.Models;
using WebServer.Tests.Secure;

namespace WebServer.Tests
{
    public class TokenTests
    {
        //const string ipAddr = "127.0.0.1";
        //const int port = 8080;
        //private List<Account> autirizedUsers = new List<Account>
        //{
        //    new Account { Email = "user1@gmail.com", Name = "User1", HashOfPassword = Hash.GetMd5Hash("11111"), Role = Roles.User },
        //    new Account { Email = "user2@gmail.com", Name = "User2", HashOfPassword = Hash.GetMd5Hash("22222"), Role = Roles.User },
        //    new Account { Email = "user3@gmail.com", Name = "User3", HashOfPassword = Hash.GetMd5Hash("33333"), Role = Roles.User },
        //};
        //private List<Account> nonAutirizedUsers = new List<Account>
        //{
        //    new Account { Email = "user1@gmail.com", Name = "User1", HashOfPassword = Hash.GetMd5Hash("11115"), Role = Roles.User },
        //    new Account { Email = "user2@gmail.com", Name = "User2", HashOfPassword = Hash.GetMd5Hash("22225"), Role = Roles.User },
        //    new Account { Email = "user3@gmail.com", Name = "User3", HashOfPassword = Hash.GetMd5Hash("33335"), Role = Roles.User },
        //};
        //[SetUp]
        //public void StartGameServer()
        //{

        //}

        //[SetUp]
        //public void CreateQueueAndRun()
        //{
        //    //�������� � ����������� �������
        //    HM2.IoCs.IoC<QueueThread>.Resolve("IoC.Registration", "Queue", new QueueThread());
        //    //�������� �������
        //    HM2.IoCs.IoC<QueueThread>.Resolve("Queue").PushCommand(new ControlCommand(HM2.IoCs.IoC<QueueThread>.Resolve("Queue").Start));
        //}

        //[TearDown]
        //public void Down()
        //{
        //    //��������� �������
        //    HM2.IoCs.IoC<QueueThread>.Resolve("Queue").PushCommand(new ControlCommand(HM2.IoCs.IoC<QueueThread>.Resolve("Queue").HardStop));

        //}
        //Task tokenServer;
        //[OneTimeSetUp]
        //public void InitTestSuite()
        //{
        //    //��������� ������ ������ �������
        //    tokenServer = new Task(() => WebServer.Program.Main(null));
        //    tokenServer.Start();
        //    Thread.Sleep(500);

        //    //��������� ������� ������
        //    EndPointNetServer endPointServer = new EndPointNetServer(ipAddr, port);
        //    //����������� ������� � IoC
        //    HM2.IoCs.IoC<EndPointNetServer>.Resolve("IoC.Registration", "Server", endPointServer);
        //    //����� �������
        //    HM2.IoCs.IoC<EndPointNetServer>.Resolve("Server").Run();
        //}

        //[OneTimeTearDown]
        //public void FinishTestSuite()
        //{
        //    //��������� ������� ������
        //    HM2.IoCs.IoC<EndPointNetServer>.Resolve("Server").Close();
        //   // WebServer.Program.Dispose();
        //}

        //[Test]
        //public void AllAlgoritmPositivTest()
        //{
        //    //��������� Http ������ ������� ��� ��������� �������������� ����
        //    string idGame = "";
        //    using (var client = new HttpClient())
        //    {
        //        const string PATHURIIDGAME = "http://localhost:5000/idgame";
        //        string lst = JsonSerializer.Serialize(autirizedUsers);
        //        string getParametersIDGAME = $"jsonListUsers={lst}";
        //        idGame = client.GetStringAsync(PATHURIIDGAME + $"?{getParametersIDGAME}").Result;
        //    }
        //    idGame = idGame.Trim(@"\""".ToCharArray());
        //    //������� ���� � ���������� ��������������� � ����� �������� ���������
        //    HM2.Games.Game game = new HM2.Games.Game();
        //    game.CreateGame(idGame, 3);

        //    //User1 ���������� ������ �� ������ jwt ������
        //    Account user1 = autirizedUsers.Find(x => x.Name == "User1");
        //    string token = ""; //� ���� ������ ����� ��������� ���������� �����
        //    using (var client = new HttpClient())
        //    {
        //        const string TOKENURL = "http://localhost:5000/token";
        //        string usr = JsonSerializer.Serialize(user1);
        //        string getParametersTOKEN = $"user={usr}&idgame={idGame}";
        //        token = client.GetStringAsync(TOKENURL + $"?{getParametersTOKEN}").Result;
        //    }
        //    token = token.Trim(@"\""".ToCharArray());
        //    //User1 ���������� ������ �� ������� ������
        //    //User1 �������� ������ ����� 1 
        //    UObject obj = HM2.IoCs.IoC<UObject>.Resolve($"game {idGame} object 1");
        //    //��������� ����� ������
        //    Vector newVect = new Vector();
        //    newVect.Shift = new Coordinats { X = 5.0, Y = 7.0 };

        //    //��������� ��������� ��� �������
        //    Message message = new Message(idGame, "1", "Move line", JsonSerializer.Serialize<Vector>(newVect));
        //    //����������� ��������� � ������
        //    StringBuilder serializedMessage = new StringBuilder();
        //    new SerializeMessageCommands(message, serializedMessage).Execute();

        //    //���������, ��� ������ �� ��������
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.X, 0.0);
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.Y, 0.0);

        //    //���������� ������� � ������� �� ������� ������ (�����, ����� �� ��������� ������ � �� ��������� ��� ���� ��������� http ������,
        //    //� �������� �������� ����������� ������ ��������������). �� ������� ������� ����������� �����,
        //    //����������� � ������������ ��������� � �������� �� EndPoints ������ ������� ����
        //    using (var client = new HttpClient())
        //    {
        //        const string URL = "http://localhost:5000/command";
        //        string param = $"token={token}&message={serializedMessage}";
        //        var result = client.GetStringAsync(URL + $"?{param}").Result;
        //    }

        //    //���������� ����
        //    Thread.Sleep(500);

        //    //��������� ��� ������ ������� ���� ���������
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.X, 5.0);
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.Y, 7.0);
        //}

        //[Test]
        //public void InvalidTokenTest()
        //{
        //    //��������� Http ������ ������� ��� ��������� �������������� ����
        //    string idGame = "";
        //    using (var client = new HttpClient())
        //    {
        //        const string PATHURIIDGAME = "http://localhost:5000/idgame";
        //        string lst = JsonSerializer.Serialize(autirizedUsers);
        //        string getParametersIDGAME = $"jsonListUsers={lst}";
        //        idGame = client.GetStringAsync(PATHURIIDGAME + $"?{getParametersIDGAME}").Result;
        //    }
        //    idGame = idGame.Trim(@"\""".ToCharArray());
        //    //������� ���� � ���������� ��������������� � ����� �������� ���������
        //    HM2.Games.Game game = new HM2.Games.Game();
        //    game.CreateGame(idGame, 3);

        //    //User1 ���������� ������ �� ������ jwt ������
        //    Account user1 = autirizedUsers.Find(x => x.Name == "User1");
        //    string tokenUser1 = ""; //� ���� ������ ����� ��������� ���������� �����
        //    using (var client = new HttpClient())
        //    {
        //        const string TOKENURL = "http://localhost:5000/token";
        //        string usr = JsonSerializer.Serialize(user1);
        //        string getParametersTOKEN = $"user={usr}&idgame={idGame}";
        //        tokenUser1 = client.GetStringAsync(TOKENURL + $"?{getParametersTOKEN}").Result;
        //    }
        //    tokenUser1 = tokenUser1.Trim(@"\""".ToCharArray());

        //    //User1 ���������� ������ �� ������� ������
        //    //User1 �������� ������ ����� 1 
        //    UObject obj = HM2.IoCs.IoC<UObject>.Resolve($"game {idGame} object 1");
        //    //��������� ����� ������
        //    Vector newVect = new Vector();
        //    newVect.Shift = new Coordinats { X = 5.0, Y = 7.0 };

        //    //��������� ��������� ��� �������
        //    Message message = new Message(idGame, "1", "Move line", JsonSerializer.Serialize<Vector>(newVect));
        //    //����������� ��������� � ������
        //    StringBuilder serializedMessage = new StringBuilder();
        //    new SerializeMessageCommands(message, serializedMessage).Execute();

        //    //���������, ��� ������ �� ��������
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.X, 0.0);
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.Y, 0.0);

        //    //���������� ������� � ������� �� ������� ������ (�����, ����� �� ��������� ������ � �� ��������� ��� ���� ��������� http ������,
        //    //� �������� �������� ����������� ������ ��������������). �� ������� ������� ����������� �����,
        //    //����������� � ������������ ��������� � �������� �� EndPoints ������ ������� ����
        //    using (var client = new HttpClient())
        //    {
        //        const string URL = "http://localhost:5000/command";
        //        tokenUser1 += "i"; //������ ���� ����������
        //        string param = $"token={tokenUser1}&message={serializedMessage}";

        //        try
        //        {
        //            var result = client.GetStringAsync(URL + $"?{param}").Result;
        //            Assert.Fail();
        //        }
        //        catch (System.AggregateException ex)
        //        {

        //        }
        //    }

        //    //��������� ��� ������ ������� ���� ���������
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.X, 0.0);
        //    Assert.AreEqual(obj.CurrentVector.PositionNow.Y, 0.0);
        //}
    }
}