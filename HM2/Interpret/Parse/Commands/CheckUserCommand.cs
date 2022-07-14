using HM2.GameSolve.Interfaces;
using HM2.IoCs;
using Ships.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships.Interpret.Parse.Commands
{
    public class CheckUserCommand : ICommand
    {
        string idGameObject;
        string idUser;
        public CheckUserCommand(string idGameObject, string idUser)
        {
            this.idUser = idUser;
            this.idGameObject = idGameObject;
        }
        public void Execute()
        {
            if (idUser != null)
            {
                var GameObjectlist = IoC<List<string>>.Resolve(idUser.Trim());

                var isExist = GameObjectlist.Find(p => p == idGameObject);

                if (isExist == null)
                    throw new NoRightsException();
            }
        }
    }
}
