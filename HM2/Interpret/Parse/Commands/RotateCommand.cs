using HM2.GameSolve.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Interpret.Parse.Commands
{
    public class RotateCommand : ICommand
    {
        IMovable movable;
        public RotateCommand(IMovable movable)
        {
            this.movable = movable;
        }
        public void Execute()
        {
            movable.rotate();
        }
    }
}
