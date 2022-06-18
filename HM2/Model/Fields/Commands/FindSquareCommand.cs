using HM2.GameSolve.Interfaces;
using HM2.Model.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Model.Fields.Commands
{
    public class FindSquareCommand<T> : ICommand
    {
        List<Square<T>> squaresWithObject;
        T gameObj;
        Square<Square<T>> field;
        public FindSquareCommand(Square<Square<T>> field, T gameObj, List<Square<T>> squaresWithObject)
        {
            this.squaresWithObject = squaresWithObject;
            this.gameObj = gameObj;
            this.field = field;
        }
        public void Execute()
        {
            foreach (var item in field.objContainer)
            {
                if (item.Contains(gameObj))
                {
                    if (!squaresWithObject.Contains(item))
                        squaresWithObject.Add(item);
                }
            }
        }
    }
}
