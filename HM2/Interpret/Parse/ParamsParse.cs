using HM2.IoCs;
using Ships.Interpret.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Interpret.Parse
{
    public class ParamsParse : IInterpret
    {
        const string term = "params";
        string order;
        public ParamsParse(string order)
        {
            this.order = order;
        }
        public void Interpret()
        {
            if (!order.Contains(term))
                return;

            order = order.Replace(Environment.NewLine, "");
            var start = order.IndexOf(term) + term.Length;
            order = order.Substring(start);

            var lengh = order.IndexOf(')');
            string _params = order.Remove(lengh).Trim().Trim(':').Trim(',').Trim().Trim('(').Trim();

            IoC<string>.Resolve("Registration", "orderParams", _params);
        }
    }
}
