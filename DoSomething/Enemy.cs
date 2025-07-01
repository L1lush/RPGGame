using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class Enemy
    {
        private int HP;
        private int ATTACK;
        private string CLASS;
        private int Xp;

        public Enemy(string CLASS, int playerLevel)
        {
            this.CLASS = CLASS;
            switch (CLASS)
            {
                case "Goblin": //forest
                    this.HP = 18 + playerLevel;
                    this.ATTACK = 6 + playerLevel;
                    this.Xp = 5 + playerLevel;
                    break;
                case "Skeleton": //cave
                    this.HP = 16 + playerLevel;
                    this.ATTACK = 8 + playerLevel;
                    this.Xp = 7 + playerLevel;
                    break;
                case "Dragon": //castle
                    this.HP = 40 + playerLevel;
                    this.ATTACK = 18 + playerLevel;
                    this.Xp = 20 + playerLevel;
                    break;
                case "Orc"://forest
                    this.HP = 28 + playerLevel;
                    this.ATTACK = 11 +  playerLevel;
                    this.Xp = 10 + playerLevel;
                    break;
                case "Troll": //forest
                    this.HP = 30 + playerLevel;
                    this.ATTACK = 13 + playerLevel;
                    this.Xp = 15 + playerLevel;
                    break;
                case "Vampire"://castle
                    this.HP = 36 + playerLevel;
                    this.ATTACK = 15 + playerLevel;
                    this.Xp = 18 + playerLevel;
                    break;
                case "Slime": // forest / cave
                    this.HP = 12 + playerLevel;
                    this.ATTACK = 4 + playerLevel;
                    this.Xp = 3 + playerLevel;
                    break;
                case "Bandit": //cave
                    this.HP = 22 + playerLevel;
                    this.ATTACK = 9 + playerLevel;
                    this.Xp = 8 + playerLevel;
                    break;
                case "Boss":
                    this.HP = 500;
                    this.ATTACK = 40;
                    this.Xp = 100;
                    break;
            }
        }
        
        public void SetHP(int HP) { this.HP = HP; }
        public int GetHP() { return this.HP; }
        public void AddHP(int amount)
        {
            this.HP += amount;
        }
        public void SubtractHP(int amount)
        {
            this.HP -= amount;
            if (this.HP < 0)
            {
                this.HP = 0; // Ensure HP doesn't go negative
            }
        }

        public void SetATTACK(int ATTACK) { this.ATTACK = ATTACK; }
        public int GetATTACK() { return this.ATTACK; }

        public void SetClass(string CLASS) { this.CLASS = CLASS; }
        public string GetClass() { return this.CLASS; }

        public void SetXp(int Xp) { this.Xp = Xp; }
        public int GetXp() { return this.Xp; }
    }
}
