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
                case "Goblin":
                    this.HP = 18 + playerLevel * 3;
                    this.ATTACK = 6 + playerLevel;
                    this.Xp = 5 + playerLevel;
                    break;
                case "Skeleton":
                    this.HP = 16 + playerLevel * 3;
                    this.ATTACK = 8 + playerLevel;
                    this.Xp = 7 + playerLevel;
                    break;
                case "Dragon":
                    this.HP = 55 + playerLevel * 8;
                    this.ATTACK = 18 + playerLevel * 2;
                    this.Xp = 20 + playerLevel * 2;
                    break;
                case "Orc":
                    this.HP = 28 + playerLevel * 4;
                    this.ATTACK = 11 + playerLevel * 2;
                    this.Xp = 10 + playerLevel * 2;
                    break;
                case "Troll":
                    this.HP = 42 + playerLevel * 6;
                    this.ATTACK = 13 + playerLevel * 2;
                    this.Xp = 15 + playerLevel * 2;
                    break;
                case "Vampire":
                    this.HP = 36 + playerLevel * 5;
                    this.ATTACK = 15 + playerLevel * 2;
                    this.Xp = 18 + playerLevel * 2;
                    break;
                case "Slime":
                    this.HP = 12 + playerLevel * 2;
                    this.ATTACK = 4 + playerLevel;
                    this.Xp = 3 + playerLevel;
                    break;
                case "Bandit":
                    this.HP = 22 + playerLevel * 3;
                    this.ATTACK = 9 + playerLevel * 2;
                    this.Xp = 8 + playerLevel * 2;
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
