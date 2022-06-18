using HM2.GameSolve.Interfaces;
using HM2.Model.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Model.Fields.Commands.MacroCommands
{
    public class UpdateFieldCommand : ICommand
    {
        Square<Square<IMovable>> mainField;
        IMovable gameObj;
        public UpdateFieldCommand(Square<Square<IMovable>> mainField, IMovable gameObj)
        {
            this.mainField = mainField;
            this.gameObj = gameObj;
        }
        public void Execute()
        {
            List<Square<IMovable>> squaresWithObject = new List<Square<IMovable>>(); //В этом списке будут хранится квадраты (окрестности) в которых найден игровой объект
            new FindSquareCommand<IMovable>(mainField, gameObj, squaresWithObject).Execute(); //вызываем команду, которая определяет список окрестностей с игровым объектом
            new ClearSquareCommand(squaresWithObject, gameObj).Execute(); //Вызываем команду очистки квадратов, если объект вышел из старого квадрата
            new ObjectDistributionCommand(mainField, gameObj).Execute(); //Вызываем команду обновления поля

            squaresWithObject = new List<Square<IMovable>>();
            new FindSquareCommand<IMovable>(mainField, gameObj, squaresWithObject).Execute(); //вызываем команду, которая определяет новый список окрестностей с игровым объектом
            new CheckCollisionCommand(squaresWithObject, gameObj).Execute(); //вызываем команду, которая определяет столкнулся ли игровой объект в новой окрестности с существующими там объектами
        }
    }
}
