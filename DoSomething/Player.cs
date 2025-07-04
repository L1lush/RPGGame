﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class Player
    {
        public int HP { get; set; }
        public int MaxHP { get; set; } = 30;
        public int ATTACK { get; set; }
        public string CLASS { get; set; }
        public int LVL { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int XPR { get; set; } = 30;
        public int Gold { get; set; } = 0;
        public int Positions { get; set; } = 5;
        public Weapon Weapon { get; set; }
        private List<Achievement> achievements = new List<Achievement>();
        public bool FirstKill = true;

        public Player() 
        {

        }
        public Player(string CLASS)
        {
            this.CLASS = CLASS;
            this.Weapon = new Weapon(); 

            switch (CLASS)
            {
                case "Knight":
                    this.HP = 15;
                    this.CLASS = "Knight";
                    Weapon.SetType("Sword");//10
                    break;
                case "Assassin":
                    this.HP = 17;
                    this.CLASS = "Assassin";
                    Weapon.SetType("Dagger");//5
                    break;
                case "Mage":
                    this.HP = 12;
                    this.CLASS = "Mage";
                    Weapon.SetType("Dagger");//5
                    break;
                case "Archer":
                    this.HP = 14;
                    this.CLASS = "Archer";
                    Weapon.SetType("Bow");//15
                    break;
            }

            this.ATTACK = Weapon.GetATTACK();
        }

        public void SetHP(int HP) { this.HP = HP; }
        public int GetHP() { return this.HP; }
        public void AddHP(int amount)
        {
            this.HP += amount;
            if (this.HP > this.MaxHP)
            {
                this.HP = this.MaxHP; // Ensure HP doesn't exceed MaxHP
            }
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
        public void AddATTACK(int amount)
        {
            this.ATTACK += amount;
        }

        public void SetCLASS(string CLASS) { this.CLASS = CLASS; }
        public string GetCLASS() { return this.CLASS; }

        public void SetLVL(int LVL) { this.LVL = LVL; }
        public int GETLVL() { return this.LVL; }

        public void SetXP(int XP) { this.XP = XP; }
        public int GETXP() { return this.XP; }

        public void SetXPR(int XPR) { this.XPR = XPR; }
        public int GETXPR() { return this.XPR; }

        public void SetGold(int Gold) { this.Gold = Gold; }
        public int GetGold() { return this.Gold; }
        public void AddGold(int amount)
        {
            this.Gold += amount;
        }
        public void SubtractGold(int amount)
        {
            this.Gold -= amount;
            if (this.Gold < 0)
            {
                this.Gold = 0; // Ensure gold doesn't go negative
            }
        }

        public void SetPositions(int Positions) { this.Positions = Positions; }
        public int GetPositions() { return this.Positions; }
        public void AddPositions(int amount)
        {
            this.Positions += amount;
        }
        public void SubtractPositions(int amount)
        {
            this.Positions -= amount;
            if (this.Positions < 0)
            {
                this.Positions = 0; // Ensure positions don't go negative
            }
        }

        public void SetMaxHP(int MaxHP) { this.MaxHP = MaxHP; }
        public int GetMaxHP() { return this.MaxHP; }
        public void AddMaxHP(int amount)
        {
            this.MaxHP += amount;
        }

        public void SetWeapon(Weapon weapon) 
        {
            this.Weapon = weapon;
            this.ATTACK += weapon.GetATTACK(); // Update ATTACK based on the weapon
        }

        public void RemoveWeapon() 
        {
            this.ATTACK -= this.Weapon.GetATTACK(); // Remove weapon's attack bonus
            this.Weapon = new Weapon(); // Reset to default weapon
        }

        public Weapon GetWeapon() 
        {
            return this.Weapon;
        }

        public void ShowStats()
        {
            Console.WriteLine($"HP: {this.HP}  Max Hp: {this.MaxHP} ATTACK: {this.ATTACK} CLASS: {this.CLASS} LVL: {this.LVL} XP: {this.XP} XPR: {this.XPR}  GOLD: {this.Gold}  Positions: {this.Positions} Weapon: {this.Weapon.GetName()}");
        }

        public void LevelUp(int XPGOT)
        {
            int currentLVL = this.GETLVL();
            int currentXP = this.GETXP();
            int requiredXP = this.GETXPR();

            int totalXP = currentXP + XPGOT;

            while (totalXP >= requiredXP)
            {
                totalXP -= (int)requiredXP;
                currentLVL += 1;
                requiredXP = (int)(requiredXP * 1.25);
                this.AddMaxHP(5); // Increase MaxHP on level up
                this.AddHP(5); // Heal on level up
                this.AddATTACK(2); // Increase ATTACK on level up
            }

            this.SetLVL(currentLVL);
            this.SetXP(totalXP);
            this.SetXPR(requiredXP);
        }

        public void AddAchievement(Achievement achievement)
        {
            achievements.Add(achievement);
        }

        public void UnlockAchievement(string title)
        {
            var achievement = achievements.FirstOrDefault(a => a.Title == title);
            if (achievement != null && !achievement.Unlocked)
                achievement.Unlock();
        }

        public bool CheckIfUnlocked(string title)
        {
            var achievement = achievements.FirstOrDefault(a => a.Title == title);
            return achievement != null && achievement.Unlocked;
        }

        public bool IsAllAchievementsCompleted(Player player)
        {
            return player.GetAchievements().All(a => a.Unlocked);
        }

        public List<Achievement> GetAchievements()
        {
            return achievements;
        }

        public void ShowAchievements()
        {
            Console.Clear();
            Console.WriteLine("🏅 Your Achievements:");
            foreach (var a in achievements)
            {
                Console.ForegroundColor = a.Unlocked ? ConsoleColor.Green : ConsoleColor.DarkGray;
                Console.WriteLine($"- {a.Title}: {a.Description} {(a.Unlocked ? "✅" : "❌")}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

        public void CheckFirstKill()
        {
            if (FirstKill == true)
                UnlockAchievement("First Blood");
            FirstKill = false;
        }
    }
}
