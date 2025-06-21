using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    class Program
    {
        static int Potions = 3;
        static void Main(string[] args)
        {
            Random rand = new Random();

            Console.WriteLine("write your Class (Knight, Assassin)");
            string Class = Console.ReadLine();
            Player Player = new Player(Class);
            Console.Clear();

            Enemy Enemy = GenerateClass();
            while (true)
            {
                Console.WriteLine("Where to go");
                Console.WriteLine("1. forest");
                Console.WriteLine("2. cave");
                Console.WriteLine("3. Castle");
                Console.WriteLine("4. Stats");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.D1)
                    Forest(Enemy, Player, rand);
                if (keyInfo.Key == ConsoleKey.D2)
                    Cave(Enemy, Player, rand);
                if (keyInfo.Key == ConsoleKey.D3)
                    Castle(Enemy, Player, rand);
                if (keyInfo.Key == ConsoleKey.D4)
                    Player.ShowStats();
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
            int Chest = rand.Next(1, 4);
            if (Chest == 2)
            {
                Console.WriteLine("you got Chest");
                Console.WriteLine($"+5HP and +5HP");
                Player.SetHP(Player.GetHP() + 5);
                Player.SetATTACK(Player.GetATTACK() + 5);
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
                Console.WriteLine("3. Use Potion");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.D1)
                {
                    Enemy.SetHP(Math.Max(0, Enemy.GetHP() - Player.GetATTACK()));
                }
                if (keyInfo.Key == ConsoleKey.D2)
                {
                    int num = rand.Next(1, 3);
                    if (num == 1) { Console.WriteLine("you ran away"); break; }
                    if (num == 2) { Player.SetHP(Math.Max(0, Player.GetHP() - Enemy.GetATTACK())); }
                }
                if (keyInfo.Key == ConsoleKey.D3 && Potions > 0)
                {
                    Console.WriteLine("You Used Potions {+20HP}");
                    Player.SetHP(Player.GetHP() + 20);
                    Potions -= 1;
                }

                if (Enemy.GetHP() <= 0) { Console.WriteLine("You won"); break; }
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

        static Enemy GenerateClass()
        {
            Enemy Enemy;
            Random rand = new Random();
            int num = rand.Next(0, 2);
            if (num == 0) { return new Enemy("Goblin"); }
            if (num == 1) { return new Enemy("Skeleton"); }
            else { return new Enemy("Dragon"); }
        }
    }
}