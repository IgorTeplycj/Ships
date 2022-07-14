using HM2.GameSolve.Interfaces;
using HM2.GameSolve.Structures;
using HM2.IoCs;
using HM2.MovableObject;
using Ships.Interpret.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Interpret.Parse
{
    public class CommandParse : IInterpret
    {
        const string term = "action";
        string order;
        public CommandParse(string order)
        {
            this.order = order;
        }
        public void Interpret()
        {
            if (!order.Contains(term))
                return;

            var start = order.IndexOf(term) + term.Length;
            order = order.Replace(Environment.NewLine, "").Substring(start);
            var lengh = order.IndexOf(',');
            string action = order.Remove(lengh).Trim().Trim(':').Trim(',').Trim();

            var orderCommand = IoC<Func<IMovable, string, ICommand>>.Resolve(action);

            IoC<Func<IMovable, string, ICommand>>.Resolve("Registration", "orderCommand", orderCommand);
        }
    }
}
