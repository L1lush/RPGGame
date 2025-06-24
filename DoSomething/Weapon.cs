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
        private int price;

        public Weapon()
        {
        }

        public Weapon(string name)
        {
            this.name = name;
            switch (name)
            {
                case "Sword":
                    this.name = "Sword";
                    this.ATTACK = 10;
                    break;
                case "Big Sword":
                    this.name = "Big Sword";
                    this.ATTACK = 30;
                    break;
                case "Axe":
                    this.name = "Axe";
                    this.ATTACK = 25;
                    break;
                case "Bow":
                    this.name = "Bow";
                    this.ATTACK = 15;
                    break;
                case "Spear":
                    this.name = "Spear";
                    this.ATTACK = 18;
                    break;
                case "Hammer":
                    this.name = "Hammer";
                    this.ATTACK = 28;
                    break;
                case "Dagger":
                    this.name = "Dagger";
                    this.ATTACK = 5;
                    break;
            }
        }

        public void SetType(string name)
        {
            this.name = name;
            switch (name)
            {
                case "Sword":
                    this.name = "Sword";
                    this.ATTACK = 10;
                    this.price = 30;
                    break;
                case "Big Sword":
                    this.name = "Big Sword";
                    this.ATTACK = 30;
                    this.price = 90;
                    break;
                case "Axe":
                    this.name = "Axe";
                    this.ATTACK = 25;
                    this.price = 75;
                    break;
                case "Bow":
                    this.name = "Bow";
                    this.ATTACK = 15;
                    this.price = 45;
                    break;
                case "Spear":
                    this.name = "Spear";
                    this.ATTACK = 18;
                    this.price = 55;
                    break;
                case "Hammer":
                    this.name = "Hammer";
                    this.ATTACK = 28;
                    this.price = 85;
                    break;
                case "Dagger":
                    this.name = "Dagger";
                    this.ATTACK = 8;
                    this.price = 24;
                    break;
            }
            
        }

        public int GetATTACK()
        {
            return this.ATTACK;
        }

        public int GetPrice()
        {
            return this.price;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}
