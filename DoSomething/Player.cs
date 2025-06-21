using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class Player
    {
        private int HP;
        private int ATTACK;
        private string CLASS;

        public Player(string CLASS)
        {
            this.CLASS = CLASS;

            if(CLASS == "Knight")
            {
                this.HP = 20;
                this.ATTACK = 5;
            }

            if(CLASS == "Assassin")
            {
                this.HP = 15;
                this.ATTACK = 7;
            }

        }

        public void SetHP(int HP) { this.HP = HP; }
        public int GetHP() { return this.HP; }

        public void SetATTACK(int ATTACK) { this.ATTACK = ATTACK; }
        public int GetATTACK() {  return this.ATTACK; }

        public void SetCLASS(string CLASS) { this.CLASS = CLASS; }
        public string GetCLASS() { return this.CLASS; }

        public void ShowStats()
        {
            Console.WriteLine($"HP: {this.HP} ATTACK: {this.ATTACK} CLASS: {this.CLASS}");
        }
    }
}
