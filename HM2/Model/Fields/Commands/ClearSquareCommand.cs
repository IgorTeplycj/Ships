using HM2.GameSolve.Interfaces;
using HM2.GameSolve.Structures;
using HM2.Model.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Model.Fields.Commands
{
    /// <summary>
    /// Проверяет принадлежность объекта к указанным квадратам и удаляет объект из квадрата, если он ему не принадлежит
    /// </summary>
    public class ClearSquareCommand : ICommand
    {
        List<Square<IMovable>> squares;
        IMovable obj;
        public ClearSquareCommand(List<Square<IMovable>> squares, IMovable obj)
        {
            this.squares = squares;
            this.obj = obj;
        }
        public void Execute()
        {
            Vector vect = obj.getPosition();
            double x = vect.PositionNow.X;
            double y = vect.PositionNow.Y;

            foreach (var item in squares.ToArray())
            {
                var check1 = (item.StartX - x) < Double.Epsilon;
                var check2 = (item.EndX - x) > -Double.Epsilon;

                var check3 = (item.StartY - y) < Double.Epsilon;
                var check4 = (item.EndY - y) > -Double.Epsilon;

                if (check1 && check2 && check3 && check4)
                {
                    
                }
                else
                {
                    item.objContainer.Remove(obj);  //Удаляем игровой объект из окрестности
                }
            }
        }
    }
}
