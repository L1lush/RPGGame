using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    class Program
    {
        static int Potions = 3;
        static int GoblinXP = 5;
        static int SkeletonXP = 7;
        static int DragonXP = 20;
        static int ShopCount = 0;
        static void Main(string[] args)
        {
            Random rand = new Random();

            Console.WriteLine("write your Class (Knight, Assassin)");
            string Class = Console.ReadLine();
            Player Player;

            if (Class == "Knight" || Class == "Assassin")
            {
                Player = new Player(Class);
            }
            else
            {
                Main(args);
                return;
            }

            Console.Clear();
            Console.WriteLine(Player.GETLVL());
            Enemy Enemy = GenerateClass();
            while (true)
            {
                Console.WriteLine("Where to go");
                Console.WriteLine("1. forest");
                Console.WriteLine("2. cave");
                Console.WriteLine("3. Castle");
                Console.WriteLine("4. Stats");
                if(ShopCount == 5)
                    Console.WriteLine("5. Shop");
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        Forest(Enemy, Player, rand);
                        break;
                    case ConsoleKey.D2:
                        Cave(Enemy, Player, rand);
                        break;
                    case ConsoleKey.D3:
                        Castle(Enemy, Player, rand);
                        break;
                    case ConsoleKey.D4:
                        Player.ShowStats();
                        Thread.Sleep(3000);
                        break;
                    case ConsoleKey.D5:
                        Shop();
                        break;
                    default:
                        continue;
                        break;
                }

                Console.Clear();
            }

        }

        static void Forest(Enemy Enemy, Player Player, Random rand)
        {
            Chest(Enemy, Player, rand);

            Console.WriteLine($"You attacked by {Enemy.GetClass()}");
            Battle( Player, rand);

        }

        static void Cave(Enemy Enemy, Player Player, Random rand)
        {
            Chest(Enemy, Player, rand);

            Console.WriteLine($"You attacked by {Enemy.GetClass()}");
            Battle( Player, rand);
        }

        static void Castle(Enemy Enemy, Player Player, Random rand)
        {
            Chest(Enemy, Player, rand);

            Console.WriteLine($"You attacked by {Enemy.GetClass()}");
            Battle(Player, rand);
        }

        static void Chest(Enemy Enemy, Player Player, Random rand)
        {
            int Chest = rand.Next(1, 5);
            if (Chest == 2)
            {
                Player.SetGold(Player.GetGold() + 20);
            }
        }

        static void Battle( Player Player, Random rand)
        {
            Enemy Enemy = GenerateClass();
            bool enemyUsedPotion = false;

            while (Player.GetHP() > 0 && Enemy.GetHP() > 0)
            {
                Console.WriteLine("Choose Your Action");
                Console.WriteLine("1. Attack");
                Console.WriteLine("2. Run");
                Console.Write("3. Use Potion");
                Console.WriteLine($" - {Potions} Potions left");
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        Enemy.SetHP(Math.Max(0, Enemy.GetHP() - Player.GetATTACK()));
                        break;
                    case ConsoleKey.D2:
                        int num = rand.Next(1, 3);
                        if (num == 1) { Console.WriteLine("you ran away"); break; }
                        if (num == 2) { Player.SetHP(Math.Max(0, Player.GetHP() - Enemy.GetATTACK())); }
                        break;
                    case ConsoleKey.D3:
                        if (Potions > 0)
                        {
                            Console.WriteLine("You Used Potions {+20HP}");
                            Player.SetHP(Player.GetHP() + 20);
                            Potions -= 1;
                        }
                        else
                        {
                            Console.WriteLine("No more Potions");
                            continue;
                        }
                        break;
                }

                if (Enemy.GetHP() <= 0) 
                {
                    CheckLVL(Player, GoblinXP);
                    Console.WriteLine("You won");
                    break; 
                }
                Console.Clear();
                Console.WriteLine("Enemy Move");
                Console.WriteLine($"Enemy HP: {Enemy.GetHP()}");

                if (Enemy.GetHP() <= 5 && !enemyUsedPotion)
                {
                    Console.WriteLine("Enemy used a potion {+5 HP}!");
                    Enemy.SetHP(Enemy.GetHP() + 5);
                    enemyUsedPotion = true;
                }
                else
                {
                    Player.SetHP(Math.Max(0, Player.GetHP() - Enemy.GetATTACK()));
                    Console.WriteLine($"Enemy Attacked -{Enemy.GetATTACK()}HP");
                    Console.WriteLine($"your hp is {Player.GetHP()}");
                }

                if (Player.GetHP() <= 0) { Console.WriteLine("You Lost"); break; }
            }
        }

        static void CheckLVL(Player player, int XPGOT)
        {
            int CurrentLVL = player.GETLVL();
            int CurrentXP = player.GETXP();
            double RXP = player.GETXPR();

            if (CurrentXP + XPGOT < RXP)
            {
                player.SetXP(CurrentXP + XPGOT);
            }
            else if (CurrentXP + XPGOT == RXP)
            {
                player.SetLVL(CurrentLVL + 1);
                player.SetXP(0);
                player.SetXPR(player.GETXPR() * 1.25);
            }

            else
            {
                player.SetLVL(CurrentLVL + 1);
                player.SetXP((CurrentXP + XPGOT) - (int)player.GETXPR());
                player.SetXPR(player.GETXPR() * 1.25);
            }
        }

        static void Shop()
        {

        }

        static Enemy GenerateClass()
        {
            Enemy Enemy;
            Random rand = new Random();
            int num = rand.Next(0, 50);
            if (num <= 24) { return new Enemy("Goblin"); }
            else if (num <= 49) { return new Enemy("Skeleton"); }
            else { return new Enemy("Dragon"); }
        }
    }
}