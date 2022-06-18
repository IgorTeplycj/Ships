﻿using HM2.Exceptions;
using HM2.GameSolve.Interfaces;
using HM2.GameSolve.Structures;
using HM2.IoCs;
using HM2.Model.Fields;
using HM2.Model.Fields.Commands;
using HM2.Model.Fields.Interfaces;
using HM2.MovableObject;
using HM2.Tests.Mock;
using NUnit.Framework;
using Ships.Model.Fields.Commands;
using Ships.Model.Fields.Commands.MacroCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM2.Tests.FieldsTests
{
    public class FieldsTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Func<UObject, Vector> getVect = (o) =>
            {
                //просто возвращаем текущий вектор объекта
                return o.CurrentVector;
            };
            IoC<Func<UObject, Vector>>.Resolve("IoC.Registration", "getPosition", getVect);

            //Регистрация зависимости
            Func<UObject, Vector> getVel = (o) =>
            {
                //просто возвращаем текущий вектор объекта
                return o.CurrentVector;
            };
            IoC<Func<UObject, Vector>>.Resolve("IoC.Registration", "getVelocity", getVel);


            Func<UObject, Vector, Vector> setVect = (o, v) =>
            {
                o.CurrentVector = v;
                return o.CurrentVector;
            };
            IoC<Func<UObject, Vector, Vector>>.Resolve("IoC.Registration", "setPosition", setVect);

            //Регистрация метода void Finish()
            Action<UObject> finish = (o) =>
            {
                o.Dispose();
            };
            IoC<Action<UObject>>.Resolve("IoC.Registration", "Finish", finish);

            //Регистрация адаптера
            Func<UObject, IMovable> getAdapter = (o) =>
            {
                IMovable movableAdapter = new HM2.AutoGenerated.MovableAdapter(o);
                return movableAdapter;
            };
            IoC<Func<UObject, IMovable>>.Resolve("IoC.Registration", "UObjectAdapter", getAdapter);
        }
        /// <summary>
        /// Тест разбиения главного поля на квадраты
        /// </summary>
        [Test]
        public void CreateAndSplitFieldsTest()
        {
            double discrete = 0;
            Square<Square<IAction>> mainField = new Square<Square<IAction>>(0.0, 1000.0, 0, 1000.0);    //Создание главного поля

            List<Square<IAction>> quares = new List<Square<IAction>>();
            //Создание команды разбиения главного поля на квадраты
            //Здесь два последних параметра это число квадратов на которое разбиваем поле по X и по Y
            new SplitFieldCommand<Square<IAction>, IAction>(mainField, quares, 5, 5).Execute();

            mainField.objContainer.AddRange(quares);    //Добавление квадратов в контейнер главного поля

            Assert.AreEqual(mainField.objContainer.Count, 25); //проверка что число квадратов поля 5х5 = 25

            //Проверка что квадрат с указанными координатами существует
            var item1 = mainField.objContainer.Find(c => c.StartX == 200 + discrete && c.EndX == 400.0 && c.StartY == 0.0 && c.EndY == 200.0);
            Assert.IsNotNull(item1);

            //Проверка что квадрат с указанными координатами существует
            var item2 = mainField.objContainer.Find(c => c.StartX == 400 + discrete && c.EndX == 600.0 && c.StartY == 200 + discrete && c.EndY == 400.0);
            Assert.IsNotNull(item2);

            //Проверка что квадрат с указанными координатами не существует
            var item3 = mainField.objContainer.Find(c => c.StartX == 400 && c.EndX == 600.0 && c.StartY == 200 && c.EndY == 405.0);
            Assert.IsNull(item3);
        }

        /// <summary>
        /// Тест проверки команды распределения игровых лбъектов по квадратам поля, при условии, что объект находится не на границе квадрата
        /// </summary>
        [Test]
        public void DistributoinCommandTest_OneObjectInsideOneQuare()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect = new Vector();
            vect.PositionNow = new Coordinats { X = 200.1, Y = 201.0 };
            UObject obj = new UObject(vect);
            //Создание экземпляра адаптера
            IMovable objAdapter = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj);

            ObjectDistributionCommand objectDistributionCommand = new ObjectDistributionCommand(mainField, objAdapter);
            objectDistributionCommand.Execute();

            //перебираем массив и ищем квадрат в котором один игровой объект
            foreach (var item in mainField.objContainer)
            {
                if (item.objContainer.Count == 1)
                {
                    if (item.StartX == 200.0 && item.EndX == 400.0 && item.StartY == 200.0 && item.EndY == 400.0)
                    {
                        //если параметры квадрата соответствуют необходимым, то ничего не делаем
                    }
                    else
                    {
                        Assert.Fail(); //если параметры квадрата не соответствуют необходимым, т.е. объект находится не в свое  квадрате,
                                       //то выбрасываем ошибку
                    }
                }
                else if (item.objContainer.Count > 1)
                {
                    Assert.Fail();
                }
                else
                {

                }
            }
        }
        [Test]
        public void DistributoinCommandTest_OneObjectInsideTwoQuare()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect = new Vector();
            vect.PositionNow = new Coordinats { X = 200.0, Y = 201.0 };
            UObject obj = new UObject(vect);
            //Создание экземпляра адаптера
            IMovable objAdapter = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj);

            ObjectDistributionCommand objectDistributionCommand = new ObjectDistributionCommand(mainField, objAdapter);
            objectDistributionCommand.Execute();

            //перебираем массив и ищем квадрат в котором один игровой объект
            foreach (var item in mainField.objContainer)
            {
                if (item.objContainer.Count == 1)
                {
                    if (item.StartX == 0.0 && item.EndX == 200.0 && item.StartY == 200.0 && item.EndY == 400.0)
                    {
                        //если параметры квадрата соответствуют необходимым, то ничего не делаем
                    }
                    else if (item.StartX == 200.0 && item.EndX == 400.0 && item.StartY == 200.0 && item.EndY == 400.0)
                    {
                        //если параметры квадрата соответствуют необходимым, то ничего не делаем
                    }
                    else
                    {
                        Assert.Fail(); //если параметры квадрата не соответствуют необходимым, т.е. объект находится не в свое  квадрате,
                                       //то выбрасываем ошибку
                    }
                }
                else if (item.objContainer.Count > 1)
                {
                    Assert.Fail();
                }
                else
                {

                }
            }
        }
        /// <summary>
        /// Тест команды, определяющей окрестности с игроывм объектом, при условии принадлежности объекта к одной окрестности.
        /// </summary>
        [Test]
        public void FindOneSquareCommandTest()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect = new Vector();
            vect.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj = new UObject(vect);
            //Создание экземпляра адаптера
            IMovable gameObj = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj);

            ObjectDistributionCommand objectDistributionCommand = new ObjectDistributionCommand(mainField, gameObj);
            objectDistributionCommand.Execute();

            List<Square<IMovable>> squaresWithObject = new List<Square<IMovable>>(); //В этом списке будут хранится квадраты (окрестности) в которых найден игровой объект
            new FindSquareCommand<IMovable>(mainField, gameObj, squaresWithObject).Execute();

            Assert.AreEqual(squaresWithObject.Count, 1);

            var square = squaresWithObject.Last();

            Assert.AreEqual(square.StartX, 200.0);
            Assert.AreEqual(square.EndX, 400.0);
            Assert.AreEqual(square.StartY, 200.0);
            Assert.AreEqual(square.EndY, 400.0);
        }
        /// <summary>
        /// Тест команды, определяющей окрестности с игроывм объектом, при условии принадлежности объекта нескольким окрестностям.
        /// </summary>
        [Test]
        public void FindTwoSquareCommandTest()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect = new Vector();
            vect.PositionNow = new Coordinats { X = 201.0, Y = 200.0 };
            UObject obj = new UObject(vect);
            //Создание экземпляра адаптера
            IMovable gameObj = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj);

            ObjectDistributionCommand objectDistributionCommand = new ObjectDistributionCommand(mainField, gameObj);
            objectDistributionCommand.Execute();

            List<Square<IMovable>> squaresWithObject = new List<Square<IMovable>>(); //В этом списке будут хранится квадраты (окрестности) в которых найден игровой объект
            new FindSquareCommand<IMovable>(mainField, gameObj, squaresWithObject).Execute();

            Assert.AreEqual(squaresWithObject.Count, 2);

            var sq1 = squaresWithObject.Find(q => q.StartX == 200 && q.EndX == 400 && q.StartY == 200 && q.EndY == 400);
            Assert.IsNotNull(sq1);

            var sq2 = squaresWithObject.Find(q => q.StartX == 200 && q.EndX == 400 && q.StartY == 0 && q.EndY == 200);
            Assert.IsNotNull(sq2);
        }
        /// <summary>
        /// Тест команды очистки окрестности при условии, что объект из окрестности не выходил и должен сохранится в своей окрестности
        /// </summary>
        [Test]
        public void ClearSquareCommandTest1()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect = new Vector();
            vect.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj = new UObject(vect);
            //Создание экземпляра адаптера
            IMovable gameObj = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj);

            ObjectDistributionCommand objectDistributionCommand = new ObjectDistributionCommand(mainField, gameObj);
            objectDistributionCommand.Execute();

            List<Square<IMovable>> squaresWithObject = new List<Square<IMovable>>(); //В этом списке будут хранится квадраты (окрестности) в которых найден игровой объект
            new FindSquareCommand<IMovable>(mainField, gameObj, squaresWithObject).Execute();

            new ClearSquareCommand(squaresWithObject, gameObj).Execute();

            Assert.AreEqual(squaresWithObject.Last().objContainer.Count, 1);    //Проверяем, что объект не был удален из окрестности

            var square = squaresWithObject.Last();

            Assert.AreEqual(square.StartX, 200.0);
            Assert.AreEqual(square.EndX, 400.0);
            Assert.AreEqual(square.StartY, 200.0);
            Assert.AreEqual(square.EndY, 400.0);
        }
        /// <summary>
        /// Тест команды очистки окрестности при условии, что объект вышел из окрестности и вошел в новую
        /// </summary>
        [Test]
        public void ClearSquareCommandTest2()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect = new Vector();
            vect.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj = new UObject(vect);
            //Создание экземпляра адаптера
            IMovable gameObj = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj);

            ObjectDistributionCommand objectDistributionCommand = new ObjectDistributionCommand(mainField, gameObj);
            objectDistributionCommand.Execute();

            List<Square<IMovable>> squaresWithObject = new List<Square<IMovable>>(); //В этом списке будут хранится квадраты (окрестности) в которых найден игровой объект
            new FindSquareCommand<IMovable>(mainField, gameObj, squaresWithObject).Execute();

            Vector vectNew = new Vector();
            vectNew.PositionNow = new Coordinats { X = 401.0, Y = 401.0 };
            gameObj.setPosition(vectNew);

            new ClearSquareCommand(squaresWithObject, gameObj).Execute();

            Assert.AreEqual(squaresWithObject.Last().objContainer.Count, 0);  //Проверяем, что объект был удален из старой окрестности
        }
        /// <summary>
        /// Тест команды проверки коллизии при условии, что имеется два сталквающихся объекта
        /// </summary>
        [Test]
        public void CheckCollisionCommandTest1()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect1 = new Vector();
            vect1.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj1 = new UObject(vect1);
            //Создание экземпляра адаптера
            IMovable gameObj1 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj1);

            Vector vect2 = new Vector();
            vect2.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj2 = new UObject(vect2);
            //Создание экземпляра адаптера
            IMovable gameObj2 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj2);

            Vector vect3 = new Vector();
            vect3.PositionNow = new Coordinats { X = 201.2, Y = 201.0 };
            UObject obj3 = new UObject(vect3);
            //Создание экземпляра адаптера
            IMovable gameObj3 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj3);

            new ObjectDistributionCommand(mainField, gameObj1).Execute(); //вызов команды распределения игрового объекта 1 на игровом поле
            new ObjectDistributionCommand(mainField, gameObj2).Execute(); //вызов команды распределения игрового объекта 2 на игровом поле
            new ObjectDistributionCommand(mainField, gameObj3).Execute(); //вызов команды распределения игрового объекта 2 на игровом поле

            List<Square<IMovable>> squaresWithObject = new List<Square<IMovable>>(); //В этом списке будут хранится квадраты (окрестности) в которых найден игровой объект 1
            new FindSquareCommand<IMovable>(mainField, gameObj1, squaresWithObject).Execute();

            try
            {
                new CheckCollisionCommand(squaresWithObject, gameObj1).Execute(); //вызываем команду определения коллизий
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetHashCode() != new CollisionCommandException("").GetType().GetHashCode())
                {
                    Assert.Fail();
                }
            }
        }
        /// <summary>
        /// Тест команды проверки коллизии при условии, что сталкивающихся объектов нет
        /// </summary>
        [Test]
        public void CheckCollisionCommandTest2()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect1 = new Vector();
            vect1.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj1 = new UObject(vect1);
            //Создание экземпляра адаптера
            IMovable gameObj1 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj1);

            Vector vect2 = new Vector();
            vect2.PositionNow = new Coordinats { X = 201.1, Y = 201.0 };
            UObject obj2 = new UObject(vect2);
            //Создание экземпляра адаптера
            IMovable gameObj2 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj2);

            new ObjectDistributionCommand(mainField, gameObj1).Execute(); //вызов команды распределения игрового объекта 1 на игровом поле
            new ObjectDistributionCommand(mainField, gameObj2).Execute(); //вызов команды распределения игрового объекта 2 на игровом поле

            List<Square<IMovable>> squaresWithObject = new List<Square<IMovable>>();  //В этом списке будут хранится квадраты (окрестности) в которых найден игровой объект 1
            new FindSquareCommand<IMovable>(mainField, gameObj1, squaresWithObject).Execute();

            new CheckCollisionCommand(squaresWithObject, gameObj1).Execute(); //вызываем команду определения коллизий
        }
        /// <summary>
        /// Тест макрокоманды обновления поля и проверки столкновения объекта с существующими в новом квадрате, при условии что столкновения не было
        /// </summary>
        [Test]
        public void UpdateFieldMacroCommandTest1()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect1 = new Vector();
            vect1.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj1 = new UObject(vect1);
            //Создание экземпляра адаптера
            IMovable gameObj1 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj1);

            Vector vect2 = new Vector();
            vect2.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj2 = new UObject(vect2);
            //Создание экземпляра адаптера
            IMovable gameObj2 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj2);

            Vector vect3 = new Vector();
            vect3.PositionNow = new Coordinats { X = 201.2, Y = 201.0 };
            UObject obj3 = new UObject(vect3);
            //Создание экземпляра адаптера
            IMovable gameObj3 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj3);

            Vector vect4 = new Vector();
            vect4.PositionNow = new Coordinats { X = 401.2, Y = 401.5 };
            UObject obj4 = new UObject(vect4);
            //Создание экземпляра адаптера
            IMovable gameObj4 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj4);

            new ObjectDistributionCommand(mainField, gameObj1).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj2).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj3).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj4).Execute(); //вызов команды распределния объектов по окрестностям

            Vector vectNew = new Vector();
            vectNew.PositionNow = new Coordinats { X = 401.2, Y = 401.7 };
            gameObj1.setPosition(vectNew);//перемещаем объект в новую окрестность

            var quare = mainField.objContainer.Find(q => q.StartX == 200 && q.EndX == 400 && q.StartY == 200 && q.EndY == 400);
            Assert.IsTrue(quare.objContainer.Contains(gameObj1));   //проверяем что игровой объект не был  удален из старой окрестности, из которой он вышел

            new UpdateFieldCommand(mainField, gameObj1).Execute(); //Вызов макрокоманды обновления игрового поля

            Assert.IsFalse(quare.objContainer.Contains(gameObj1));  //проверяем что игровой объект был удален из старой окрестности, из которой он вышел

            var quareNew = mainField.objContainer.Find(q => q.StartX == 400 && q.EndX == 600 && q.StartY == 400 && q.EndY == 600); //Получаем окрестность, в которой должен появиться игровой объект

            Assert.IsTrue(quareNew.objContainer.Contains(gameObj1)); //Проверяем что объект появился в новой окрестности
            Assert.IsTrue(quareNew.objContainer.Contains(gameObj4)); //Проверяем что в этой же окрестности уже имеется другой объект
        }
        /// <summary>
        /// Тест макрокоманды обновления поля и проверки столкновения объекта с существующими в новом квадрате, при условии что объекты в новой окрестности столкнулись
        /// </summary>
        [Test]
        public void UpdateFieldMacroCommandTest2()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect1 = new Vector();
            vect1.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj1 = new UObject(vect1);
            //Создание экземпляра адаптера
            IMovable gameObj1 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj1);

            Vector vect2 = new Vector();
            vect2.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj2 = new UObject(vect2);
            //Создание экземпляра адаптера
            IMovable gameObj2 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj2);

            Vector vect3 = new Vector();
            vect3.PositionNow = new Coordinats { X = 201.2, Y = 201.0 };
            UObject obj3 = new UObject(vect3);
            //Создание экземпляра адаптера
            IMovable gameObj3 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj3);

            Vector vect4 = new Vector();
            vect4.PositionNow = new Coordinats { X = 401.2, Y = 401.5 };
            UObject obj4 = new UObject(vect4);
            //Создание экземпляра адаптера
            IMovable gameObj4 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj4);

            new ObjectDistributionCommand(mainField, gameObj1).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj2).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj3).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj4).Execute(); //вызов команды распределния объектов по окрестностям

            Vector vectNew = new Vector();
            vectNew.PositionNow = new Coordinats { X = 401.2, Y = 401.5 };
            gameObj1.setPosition(vectNew);//перемещаем объект в новую окрестность

            try
            {
                new UpdateFieldCommand(mainField, gameObj1).Execute(); //Вызов макрокоманды обновления игрового поля
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetHashCode() != new CollisionCommandException("").GetType().GetHashCode())
                {
                    Assert.Fail();
                }
            }
        }
        /// <summary>
        /// Тест макрокоманды обновления поля и проверки столкновения объекта с существующими в новом квадрате, при условии что объекты в новой окрестности столкнулись, 
        /// а сам движимый объект находился на границе двух окрестностей
        /// </summary>
        [Test]
        public void UpdateFieldMacroCommandTest3()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect1 = new Vector();
            vect1.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj1 = new UObject(vect1);
            //Создание экземпляра адаптера
            IMovable gameObj1 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj1);

            Vector vect2 = new Vector();
            vect2.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj2 = new UObject(vect2);
            //Создание экземпляра адаптера
            IMovable gameObj2 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj2);

            Vector vect3 = new Vector();
            vect3.PositionNow = new Coordinats { X = 201.2, Y = 201.0 };
            UObject obj3 = new UObject(vect3);
            //Создание экземпляра адаптера
            IMovable gameObj3 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj3);

            Vector vect4 = new Vector();
            vect4.PositionNow = new Coordinats { X = 401.2, Y = 400.0 };
            UObject obj4 = new UObject(vect4);
            //Создание экземпляра адаптера
            IMovable gameObj4 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj4);

            new ObjectDistributionCommand(mainField, gameObj1).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj2).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj3).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj4).Execute(); //вызов команды распределния объектов по окрестностям

            Vector vectNew = new Vector();
            vectNew.PositionNow = new Coordinats { X = 401.2, Y = 400.0 }; 
            gameObj1.setPosition(vectNew); //перемещаем объект в новую окрестность

            try
            {
                new UpdateFieldCommand(mainField, gameObj1).Execute(); //Вызов макрокоманды обновления игрового поля
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetHashCode() != new CollisionCommandException("").GetType().GetHashCode())
                {
                    Assert.Fail();
                }
            }
        }
        /// <summary>
        /// Тест макрокоманды обновления поля и проверки столкновения объекта с существующими, при условии, что движимый объект двигался в пределах одной окрестности
        /// </summary>
        [Test]
        public void UpdateFieldMacroCommandTest4()
        {
            //СОЗДАНИЕ ГЛАВНОГО ПОЛЯ
            List<Square<IMovable>> quares = new List<Square<IMovable>>();
            Square<Square<IMovable>> mainField = new Square<Square<IMovable>>(0.0, 1000.0, 0, 1000.0);
            new SplitFieldCommand<Square<IMovable>, IMovable>(mainField, quares, 5, 5).Execute();
            mainField.objContainer.AddRange(quares);

            Vector vect1 = new Vector();
            vect1.PositionNow = new Coordinats { X = 201.0, Y = 201.0 };
            UObject obj1 = new UObject(vect1);
            //Создание экземпляра адаптера
            IMovable gameObj1 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj1);

            Vector vect2 = new Vector();
            vect2.PositionNow = new Coordinats { X = 300.0, Y = 225.0 };
            UObject obj2 = new UObject(vect2);
            //Создание экземпляра адаптера
            IMovable gameObj2 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj2);

            Vector vect3 = new Vector();
            vect3.PositionNow = new Coordinats { X = 201.2, Y = 201.0 };
            UObject obj3 = new UObject(vect3);
            //Создание экземпляра адаптера
            IMovable gameObj3 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj3);

            Vector vect4 = new Vector();
            vect4.PositionNow = new Coordinats { X = 401.2, Y = 400.0 };
            UObject obj4 = new UObject(vect4);
            //Создание экземпляра адаптера
            IMovable gameObj4 = IoC<Func<UObject, IMovable>>.Resolve("UObjectAdapter").Invoke(obj4);

            new ObjectDistributionCommand(mainField, gameObj1).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj2).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj3).Execute(); //вызов команды распределния объектов по окрестностям
            new ObjectDistributionCommand(mainField, gameObj4).Execute(); //вызов команды распределния объектов по окрестностям

            Vector vectNew = new Vector();
            vectNew.PositionNow = new Coordinats { X = 300.0, Y = 225.0 };
            gameObj1.setPosition(vectNew);

            try
            {
                new UpdateFieldCommand(mainField, gameObj1).Execute(); //Вызов макрокоманды обновления игрового поля
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetHashCode() != new CollisionCommandException("").GetType().GetHashCode())
                {
                    Assert.Fail();
                }
            }
        }
    }
}