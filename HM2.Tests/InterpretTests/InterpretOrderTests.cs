using HM2.GameSolve.Interfaces;
using HM2.GameSolve.Structures;
using HM2.IoCs;
using HM2.MovableObject;
using HM2.Tests.AdapterTests;
using NUnit.Framework;
using Ships.Interpret.Interfaces;
using Ships.Interpret.Parse;
using System;

namespace HM2.Tests.InterpretTests
{
    class InterpretOrderTests
    {
        [SetUp]
        public void Setup()
        {
            //регистрация адаптера и его методов
            new AutoGenerationAdapter().Setup();

            //Регистрация метода, возвращающего интерпретированную команду для перемещения игрового объекта
            Func<IMovable, string, ICommand> moving = (o, param) =>
            {
                Vector newVect = new Vector();
                newVect.PositionNow = new Coordinats { X = Convert.ToDouble(param.Split(',')[0]), Y = Convert.ToDouble(param.Split(',')[1]) };
                Ships.Interpret.Parse.Commands.MoveCommand move = new Ships.Interpret.Parse.Commands.MoveCommand(o, newVect);
                return move;
            };
            IoC<Func<IMovable, string, ICommand>>.Resolve("IoC.Registration", "setPosition", moving);

            //Регистрация метода, возвращающего интерпретированную команду для поворота игрового объекта
            Func<IMovable, string, ICommand> rot = (o, param) =>
            {
                Vector newVect = new Vector();
                newVect.PositionNow = new Coordinats { X = o.getPosition().PositionNow.X, Y = o.getPosition().PositionNow.Y };
                newVect.AngularVelosity = Convert.ToInt32(param.Split(',')[0]);
                newVect.Direction = Convert.ToInt32(param.Split(',')[1]);
                newVect.DirectionNumber = Convert.ToInt32(param.Split(',')[2]);
                o.setPosition(newVect);

                Ships.Interpret.Parse.Commands.RotateCommand rotateCommand = new Ships.Interpret.Parse.Commands.RotateCommand(o);
                return rotateCommand;
            };
            IoC<Func<IMovable, string, ICommand>>.Resolve("IoC.Registration", "rotate", rot);

            //Создание игрового объекта
            Vector vect = new Vector();
            vect.PositionNow = new Coordinats { X = 5.0, Y = 6.0 };
            vect.DirectionNumber = 24;
            vect.Direction = 5;
            vect.AngularVelosity = 2;
            UObject obj = new UObject(vect);
            //Создание экземпляра адаптера
            IMovable movableAdapter = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj);

            //Регистрация игрового объекта с идентиикатором 548
            IoC<IMovable>.Resolve("Registration", "548", movableAdapter);
        }

        [Test]
        public void GameObjectInterpretTest()
        {
            string order = @"{  id: 548,
                                action: setPosition,
                                params: (10, -2)
                                }";

            //вызываем интерпретатор приказа, регистрирующий в IoC игровой объект из приказа
            IInterpret gameObjInterpret = new GameObject(order);
            gameObjInterpret.Interpret();

            //Получаем объект, указанный в приказе
            IMovable orderObj = IoC<IMovable>.Resolve("orderUObject");

            Assert.IsNotNull(orderObj);
            //Проверяем координаты объекта
            Assert.AreEqual(orderObj.getPosition().PositionNow.X, 5.0);
            Assert.AreEqual(orderObj.getPosition().PositionNow.Y, 6.0);
        }
        [Test]
        public void CommandMoveInterpretTest()
        {
            string order = @"{  id: 548,
                                action: setPosition,
                                params: (10, -2)
                                }";

            //вызываем интерпретатор приказа, регистрирующий в IoC игровой объект из приказа
           new GameObject(order).Interpret();
            //Вызываем интерпретатор приказа, регистрирующий команду из приказа
           new CommandParse(order).Interpret();

            //Получаем объект, указанный в приказе
            IMovable orderObj = IoC<IMovable>.Resolve("orderUObject");
            //Получаем команду, указанную в приказе
            var orderCommand = IoC<Func<IMovable, string, ICommand>>.Resolve("orderCommand");

            //Парсим параметры приказа
            new ParamsParse(order).Interpret();
            //Получаем параметры приказа
            string _params = IoC<string>.Resolve("orderParams");

            //проверяем, что игровой объект еще не переместился
            Assert.AreEqual(orderObj.getPosition().PositionNow.X, 5.0);
            Assert.AreEqual(orderObj.getPosition().PositionNow.Y, 6.0);

            //Вызываем интерпретированную из приказа команду
            orderCommand.Invoke(orderObj, _params).Execute();

            //проверяем перемещение объекта
            Assert.AreEqual(orderObj.getPosition().PositionNow.X, 10.0);
            Assert.AreEqual(orderObj.getPosition().PositionNow.Y, -2.0);
        }
        [Test]
        public void CommandRotateInterpretTest()
        {
            string order = @"{  id: 548,
                                action: rotate,
                                params: (10, 15, 24)
                                }";

            //вызываем интерпретатор приказа, регистрирующий в IoC игровой объект из приказа
            new GameObject(order).Interpret();
            //Вызываем интерпретатор приказа, регистрирующий команду из приказа
            new CommandParse(order).Interpret();

            //Получаем объект, указанный в приказе
            IMovable orderObj = IoC<IMovable>.Resolve("orderUObject");
            //Получаем команду, указанную в приказе
            var orderCommand = IoC<Func<IMovable, string, ICommand>>.Resolve("orderCommand");

            //Парсим параметры приказа
            new ParamsParse(order).Interpret();
            //Получаем параметры приказа
            string _params = IoC<string>.Resolve("orderParams");

            Assert.AreEqual(orderObj.getPosition().AngularVelosity, 2);
            Assert.AreEqual(orderObj.getPosition().Direction, 5);
            Assert.AreEqual(orderObj.getPosition().DirectionNumber, 24);

            orderCommand.Invoke(orderObj, _params).Execute();

            Assert.AreEqual(orderObj.getPosition().AngularVelosity, 10);
            Assert.AreEqual(orderObj.getPosition().Direction, 1);
            Assert.AreEqual(orderObj.getPosition().DirectionNumber, 24);
        }

    }
}
                                                                                    