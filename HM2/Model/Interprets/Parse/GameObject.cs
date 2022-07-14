using HM2.IoCs;
using HM2.MovableObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Model.Interprets.Parse
{
    public class GameObject : IInterpret
    {
        const string term = "id";
        string order;
        public GameObject(string order)
        {
            this.order = order;
        }
        public void Interpret()
        {
            if (!order.Contains(term))
                return;

            string id = order.Substring(term, );

            UObject orderUbject = IoC<UObject>.Resolve(id);
            IoC<UObject>.Resolve("Registration", "orderUObject", orderUbject);
        }
    }
}
