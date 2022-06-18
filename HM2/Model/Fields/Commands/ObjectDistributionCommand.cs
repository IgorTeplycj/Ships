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
    public class ObjectDistributionCommand : ICommand
    {
        double epsilon = Math.Pow(10, -10);
        Square<Square<IMovable>> field;
        IMovable obj;
        public ObjectDistributionCommand(Square<Square<IMovable>> field, IMovable obj)
        {
            this.field = field;
            this.obj = obj;
        }

        public void Execute()
        {
            Vector vect = obj.getPosition();
            double x = vect.PositionNow.X;
            double y = vect.PositionNow.Y;

            foreach (var item in field.objContainer)
            {
                var check1 = (item.StartX - x) <= Double.Epsilon;
                var check2 = (item.EndX - x) >= -Double.Epsilon;

                var check3 = (item.StartY - y) <= Double.Epsilon;
                var check4 = (item.EndY - y) >= -Double.Epsilon;

                if (check1 && check2 && check3 && check4)
                {
                    if (!item.objContainer.Contains(obj))
                    {
                        item.objContainer.Add(obj);
                    }
                }
            }

        }
    }
}
