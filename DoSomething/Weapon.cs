using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class Weapon
    {
        private string name;
        private int ATTACK;

        public Weapon(string name)
        {
            this.name = name;
            switch (name)
            {
                case "Knife":
                    this.ATTACK = 10;
                    break;
                case "Sword":
                    this.ATTACK = 20;
                    break;
                case "Big Sword":
                    this.ATTACK = 30;
                    break;
            }
        }

        public int GetATTACK()
        {
            return this.ATTACK;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
        public int NumOfWeapons()
        {
            return 3;
        }
    }
}
