using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Authentication;
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
                StartUpMenu();
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
                Console.WriteLine("You got 20 Gold");
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
                    case ConsoleKey.Escape:
                        PAUSEMenu(Player);
                        continue;
                        break;
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
                    CheckLVL(Player, GoblinXP); // Change
                    Player.SetGold(Player.GetGold() + 3);
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

        static void Shop(Player Player)
        {
            Console.Clear();
            Console.WriteLine($"What do you want to buy (you have {Player.GetGold()} Gold)");
            Console.WriteLine("1. Knife (ATTACK 10) | Price: 30 gold");
            Console.WriteLine("2. Sword (ATTACK 20) | Price: 60 gold");
            Console.WriteLine("3. Big Sword (ATTACK 30) | Price: 90 gold");
            Console.WriteLine("4. Exit");

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    PAUSEMenu(Player);
                    break;
                case ConsoleKey.D1:
                    Player.SetATTACK(10);
                    break;
                case ConsoleKey.D2:
                    Player.SetATTACK(20);
                    break;
                case ConsoleKey.D3:
                    Player.SetATTACK(30);
                    break;
                case ConsoleKey.D4:
                    break;
            }
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

        static void StartUpMenu()
        {
            string[] lines =
            {
            "╔════════════════════════════════════╗",
            "║          ⚔️RPG MAIN MENU ⚔️       ║",
            "╠════════════════════════════════════╣",
            "║     Start New Game                 ║",
            "║     Load Game                      ║",
            "║     Settings                       ║",
            "║     Credits                        ║",
            "║     Exit                           ║",
            "╚════════════════════════════════════╝"
        };

            int selectedIndex = 0;
            int menuStartRow = 3;
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (i >= menuStartRow && i < menuStartRow + 5)
                    {
                        int optionIndex = i - menuStartRow;
                        if (optionIndex == selectedIndex)
                            line = line.Substring(0, 3) + "▶" + line.Substring(4);
                        else
                            line = line.Substring(0, 3) + " " + line.Substring(4);
                    }

                    Console.WriteLine(line);
                }

                Console.WriteLine("\nUse ↑ ↓ to navigate. Press Enter to select.");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 5;
                        break;
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 5) % 5;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();

                        // 🔽 Function call logic based on selection
                        switch (selectedIndex)
                        {
                            case 0:
                                StartNewGame();
                                break;
                            case 1:
                                LoadGame();
                                break;
                            case 2:
                                OpenSettings();
                                break;
                            case 3:
                                ShowCredits();
                                break;
                            case 4:
                                ExitGame();
                                return;
                        }

                        Console.WriteLine("\nPress any key to return to menu...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        static void LoadGame()
        {

        }

        static void OpenSettings() 
        {
            
        }

        static void StartNewGame()
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
                StartNewGame();
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
                if (ShopCount % 5 == 0)
                    Console.WriteLine("5. Shop");

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        PAUSEMenu(Player);
                        continue;
                        break;
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
                        Shop(Player);
                        break;
                    default:
                        continue;
                        break;
                }
                ShopCount++;
                Console.Clear();
            }
        }

        static void ShowCredits()
        {
            Console.WriteLine("Game developed by Ilya and Or!");
        }

        static void ExitGame()
        {
            Console.Clear();
            Console.WriteLine("Thank you for playing! Goodbye!");
            Thread.Sleep(2000);
            Environment.Exit(0);// Exits the application
        }

        static void PAUSEMenu(Player player)
        {
            string[] options = { "Resume", "Stats", "Exit to Main Menu" };
            int selected = 0;
            ConsoleKey key;

            Console.CursorVisible = false;
            do
            {
                Console.Clear();
                         Console.WriteLine("╔═══════════════════════╗");
                         Console.WriteLine("║       PAUSE MENU      ║");
                         Console.WriteLine("╠═══════════════════════╣");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                        Console.WriteLine($"║ ▶ {options[i],-17}   ║");// -17 is for alignment
                    else
                        Console.WriteLine($"║   {options[i],-17}   ║");
                }
                         Console.WriteLine("╚═══════════════════════╝");
                Console.WriteLine("\nUse ↑ ↓ to navigate. Enter to select.");

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + options.Length) % options.Length;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % options.Length;

            } while (key != ConsoleKey.Enter);

            switch (selected)
            {
                case 0:
                    // Resume game
                    Console.Clear();
                    return;
                case 1:
                    // Show stats
                    player.ShowStats();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey(true);
                    PAUSEMenu(player);
                    break;
                case 2:
                    // Exit to main menu
                    StartUpMenu();
                    break;
            }
        }
    }
}