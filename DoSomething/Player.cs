using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class Player
    {
        private int HP;
        private int ATTACK;
        private string CLASS;
        private int LVL = 1;
        private int XP = 0;
        private double XPR = 10;
        private int Gold = 0;

        public Player(string CLASS)
        {
            this.CLASS = CLASS;

            switch (CLASS)
            {
                case "Knight":
                    this.HP = 20;
                    this.ATTACK = 5;
                    break;
                case "Assassin":
                     this.HP = 15;
                    this.ATTACK = 7;
                    break;
            }

        }

        public void SetHP(int HP) { this.HP = HP; }
        public int GetHP() { return this.HP; }

        public void SetATTACK(int ATTACK) { this.ATTACK = ATTACK; }
        public int GetATTACK() {  return this.ATTACK; }

        public void SetCLASS(string CLASS) { this.CLASS = CLASS; }
        public string GetCLASS() { return this.CLASS; }

        public void SetLVL(int LVL) { this.LVL = LVL; }
        public int GETLVL() { return this.LVL; }

        public void SetXP(int XP) { this.XP = XP; }
        public int GETXP() { return this.XP; }

        public void SetXPR(double XPR) { this.XPR = XPR; }
        public double GETXPR() { return this.XPR; }

        public void SetGold(int Gold) {  this.Gold = Gold; }
        public int GetGold() {  return this.Gold; }

        public void ShowStats()
        {
            Console.WriteLine($"HP: {this.HP} ATTACK: {this.ATTACK} CLASS: {this.CLASS} LVL: {this.LVL} XP: {this.XP} XPR: {this.XPR}  GOLD: {this.Gold}");
        }

        public void LevelUp(int XPGOT)
        {
            int currentLVL = this.GETLVL();
            int currentXP = this.GETXP();
            double requiredXP = this.GETXPR();

            int totalXP = currentXP + XPGOT;

            while (totalXP >= requiredXP)
            {
                totalXP -= (int)requiredXP;
                currentLVL += 1;
                requiredXP *= 1.25;
            }

            this.SetLVL(currentLVL);
            this.SetXP(totalXP);
            this.SetXPR(requiredXP);
        }
    }
}
