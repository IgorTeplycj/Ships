﻿using HM2.GameSolve.Interfaces;
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
            order = order.Replace(Environment.NewLine, "");
            var start = order.IndexOf(term) + term.Length;
            var lengh = order.IndexOf(',');

            string id = order.Substring(start, lengh).Trim().Trim(':').Trim(',').Trim();

            IMovable orderUbject = IoC<IMovable>.Resolve(id);
            IoC<IMovable>.Resolve("Registration", "orderUObject", orderUbject); 
        }
    }
}

