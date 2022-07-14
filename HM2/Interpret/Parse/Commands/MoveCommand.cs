using HM2.GameSolve.Interfaces;
using HM2.GameSolve.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Interpret.Parse.Commands
{
    public class MoveCommand : ICommand
    {
        IMovable movable;
        Vector vector;
        public MoveCommand(IMovable movable, Vector vector)
        {
            this.movable = movable;
            this.vector = vector;
        }
        public void Execute()
        {
            movable.setPosition(vector);
        }
    }
}
