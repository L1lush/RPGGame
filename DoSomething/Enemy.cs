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

        public Enemy(string CLASS)
        {
            this.CLASS = CLASS;

            if (CLASS == "Goblin")
            {
                this.HP = 20;
                this.ATTACK = 5;
            }

            if (CLASS == "Skeleton")
            {
                this.HP = 15;
                this.ATTACK = 7;
            }

            if (CLASS == "Dragon")
            {
                this.HP = 80;
                this.ATTACK = 10;
            }
        }

        public void SetHP(int HP) { this.HP = HP; }
        public int GetHP() { return this.HP; }

        public void SetATTACK(int ATTACK) { this.ATTACK = ATTACK; }
        public int GetATTACK() { return this.ATTACK; }

        public void SetClass(string CLASS) { this.CLASS = CLASS; }
        public string GetClass() { return this.CLASS; }
    }
}
