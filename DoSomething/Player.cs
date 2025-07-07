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
        public List<achievement> Achievements { get; set; } = new List<achievement>();

        //stats
        public int Kills { get; set; } = 0;
        public int GoldCollected { get; set; } = 0;
        public int WeaponEquiped { get; set; } = 0;
        public int ShopVisits { get; set; } = 0;
        public int ForestVisits { get; set; } = 0;
        public int CaveVisits { get; set; } = 0;
        public int CastleVisits { get; set; } = 0;
        public int BossFightVisits { get; set; } = 0;

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

            Achievements = new List<achievement>
            {
                new achievement("First Blood", "Defeat your first enemy."),
                new achievement("Treasure Hunter", "Collect 100 gold."),
                new achievement("Master of Arms", "Equip a weapon."),
                new achievement("Level Up", "Reach level 5."),
                new achievement("Explorer", "Visit all locations.")
            };
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
            GoldCollected += amount; // Track total gold collected

            if(GoldCollected >= 100)
            {
                UnlockAchievement("Treasure Hunter"); // Unlock achievement for collecting 100 gold
            }
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
            WeaponEquiped += 1;
            if (WeaponEquiped == 1)
            {
                UnlockAchievement("Master of Arms"); // Unlock achievement for equipping a weapon
            }
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

            if(currentLVL == 5)
            {
                UnlockAchievement("Level Up"); // Unlock achievement for reaching level 5
            }
        }
        public void UnlockAchievement(string title)
        {
            var ach = Achievements.FirstOrDefault(a => a.Title == title);
            if (ach != null && !ach.Unlocked)
            {
                ach.Unlock();
                Console.WriteLine($"Achievement unlocked: {ach.Title} - {ach.Description}");
            }
        }

        public string ShowAchievements()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Achievements:");
            foreach (var ach in Achievements)
            {
                sb.AppendLine($"{ach.Title} - {ach.Description} (Unlocked: {ach.Unlocked})");
            }
            return sb.ToString();
        }
    }
}
