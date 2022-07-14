using HM2.GameSolve.Interfaces;
using HM2.IoCs;
using Ships.Interpret.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Interpret.Parse
{
    public class UserRulesParse : IInterpret
    {
        const string term = "user";
        string order;
        public UserRulesParse(string order)
        {
            this.order = order;
        }
        public void Interpret()
        {
            if (!order.Contains(term))
            {
                IoC<string>.Resolve("Registration", "orderUser", null);
            }
            else
            {
                var start = order.IndexOf(term) + term.Length;
                order = order.Replace(Environment.NewLine, "").Substring(start);
                var lengh = order.IndexOf(',');
                string user = order.Remove(lengh).Trim().Trim(':').Trim(',').Trim();

                IoC<string>.Resolve("Registration", "orderUser", user);
            }
        }
    }
}
