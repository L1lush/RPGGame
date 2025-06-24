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

        public Enemy(string CLASS)
        {
            this.CLASS = CLASS;

            switch (CLASS)
            {
                case "Goblin":
                    this.HP = 20;
                    this.ATTACK = 5;
                    this.Xp = 5;
                    break;
                case "Skeleton":
                    this.HP = 15;
                    this.ATTACK = 7;
                    this.Xp = 7;
                    break;
                case "Dragon":
                    this.HP = 80;
                    this.ATTACK = 15;
                    this.Xp = 20;
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
