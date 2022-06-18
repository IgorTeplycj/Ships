using HM2.Exceptions;
using HM2.GameSolve.Interfaces;
using HM2.Model.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Model.Fields.Commands
{
    public class CheckCollisionCommand : ICommand
    {
        List<Square<IMovable>> squaresWithObj;
        IMovable obj;
        public CheckCollisionCommand(List<Square<IMovable>> squaresWithObj, IMovable obj)
        {
            this.squaresWithObj = squaresWithObj;
            this.obj = obj;
        }
        public void Execute()
        {
            foreach (var square in squaresWithObj)
            {
                foreach (var movable in square.objContainer)
                {
                    if (obj.GetHashCode() != movable.GetHashCode()) //проверяем что переданным объект не является самим собой в переданном квадрате (окрестности)
                    {
                        bool collisionX = Math.Abs(obj.getPosition().PositionNow.X - movable.getPosition().PositionNow.X) < Double.Epsilon;
                        bool collisionY = Math.Abs(obj.getPosition().PositionNow.Y - movable.getPosition().PositionNow.Y) < Double.Epsilon;

                        if(collisionX && collisionY)
                        {
                            throw new CollisionCommandException($"Collision with object [{movable.getPosition().PositionNow.X}:{movable.getPosition().PositionNow.Y}]");
                        }
                    }
                }
            }
        }
    }
}
