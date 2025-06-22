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
        static int ShopCount = 1;
        static void Main(string[] args)
        {
                StartUpMenu();
        }

        static void StartUpMenu()
        {
            string[] lines =
            {
            "╔════════════════════════════════════╗",
            "║          ⚔️RPG MAIN MENU ⚔️        ║",
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

                Console.WriteLine("Use ↑ ↓ to navigate. Press Enter to select.");

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

                        Console.WriteLine("Press any key to return to menu...");
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

        static void StartNewGame()
        {
            Random rand = new Random();

            string[] classOptions = { "Knight", "Assassin" };
            int selectedClass = 0;
            ConsoleKey classKey;
            Console.CursorVisible = false;

            do
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════╗");
                Console.WriteLine("║      Choose Your Class       ║");
                Console.WriteLine("╠══════════════════════════════╣");
                for (int i = 0; i < classOptions.Length; i++)
                {
                    if (i == selectedClass)
                        Console.WriteLine($"║ ▶ {classOptions[i],-27}║");
                    else
                        Console.WriteLine($"║   {classOptions[i],-27}║");
                }
                Console.WriteLine("╚══════════════════════════════╝");
                Console.WriteLine("Use ↑ ↓ to navigate. Press Enter to select.");

                classKey = Console.ReadKey(true).Key;
                if (classKey == ConsoleKey.UpArrow)
                    selectedClass = (selectedClass - 1 + classOptions.Length) % classOptions.Length;
                else if (classKey == ConsoleKey.DownArrow)
                    selectedClass = (selectedClass + 1) % classOptions.Length;

            } while (classKey != ConsoleKey.Enter);

            Player Player = new Player(classOptions[selectedClass]);

            Console.Clear();
            Console.WriteLine(Player.GETLVL());
            Enemy Enemy = GenerateClass();
            while (true)
            {
                    string[] options = {
                        "Forest",
                        "Cave",
                        "Castle",
                        "Stats",
                        (ShopCount % 5 == 0) ? "Shop" : null
                    };
                    options = options.Where(o => o != null).ToArray();
                    int selected = 0;
                    ConsoleKey key;

                    Console.CursorVisible = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("╔══════════════════════════════╗");
                    Console.WriteLine("║        Where to go?          ║");
                    Console.WriteLine("╠══════════════════════════════╣");
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (i == selected)
                            Console.WriteLine($"║ ▶ {options[i],-27}║");
                        else
                            Console.WriteLine($"║   {options[i],-27}║");
                    }
                    Console.WriteLine("╚══════════════════════════════╝");
                    Console.WriteLine("Esc for pause.");

                    key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow)
                        selected = (selected - 1 + options.Length) % options.Length;
                    else if (key == ConsoleKey.DownArrow)
                        selected = (selected + 1) % options.Length;
                    else if (key == ConsoleKey.Escape)
                    {
                        PAUSEMenu(Player);
                        return;
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        switch (options[selected])
                        {
                            case "Forest":
                                Forest(Enemy, Player, rand);
                                ShopCount++;
                                return;
                            case "Cave":
                                Cave(Enemy, Player, rand);
                                ShopCount++;
                                return;
                            case "Castle":
                                Castle(Enemy, Player, rand);
                                ShopCount++;
                                return;
                            case "Stats":
                                Player.ShowStats();
                                Thread.Sleep(3000);
                                return;
                            case "Shop":
                                Shop(Player);
                                ShopCount++;
                                return;
                        }
                    }
                } while (true);
                
            }
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
                Console.WriteLine("Use ↑ ↓ to navigate. Enter to select.");

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

        static void Battle(Player Player, Random rand)
        {
            Enemy Enemy = GenerateClass();
            bool enemyUsedPotion = false;

            while (Player.GetHP() > 0 && Enemy.GetHP() > 0)
            {
                // Action selection menu
                string[] actions = {
                    "Attack                     ",
                    "Run                        ",
                   $"Use Potion ({Potions} left)        " 
                };
                int selectedAction = 0;
                ConsoleKey key;
                do
                {
                    Console.Clear();
                    Console.WriteLine("╔═══════════════════════════════╗");
                    Console.WriteLine("║   Choose Your Action          ║");
                    Console.WriteLine("╠═══════════════════════════════╣");
                    for (int i = 0; i < actions.Length; i++)
                    {
                        if (i == selectedAction)
                            Console.WriteLine($"║ ▶ {actions[i],-16} ║");
                        else
                            Console.WriteLine($"║   {actions[i],-16} ║");
                    }
                    Console.WriteLine("╚═══════════════════════════════╝");
                    Console.WriteLine("Use ↑ ↓ to navigate. Enter to select. Esc for pause.");

                    key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow)
                        selectedAction = (selectedAction - 1 + actions.Length) % actions.Length;
                    else if (key == ConsoleKey.DownArrow)
                        selectedAction = (selectedAction + 1) % actions.Length;
                    else if (key == ConsoleKey.Escape)
                    {
                        PAUSEMenu(Player);
                    }
                } while (key != ConsoleKey.Enter);

                Console.Clear();
                switch (selectedAction)
                {
                    case 0: // Attack
                        Enemy.SetHP(Math.Max(0, Enemy.GetHP() - Player.GetATTACK()));
                        Console.WriteLine($"You attacked! Enemy HP is now {Enemy.GetHP()}");
                        break;
                    case 1: // Run
                        int num = rand.Next(1, 3);
                        if (num == 1)
                        {
                            Console.WriteLine("You ran away!");
                            Thread.Sleep(1000);
                            return;
                        }
                        else
                        {
                            Player.SetHP(Math.Max(0, Player.GetHP() - Enemy.GetATTACK()));
                            Console.WriteLine($"Failed to run! Enemy attacked for {Enemy.GetATTACK()} damage.");
                            Console.WriteLine($"Your HP: {Player.GetHP()}");
                        }
                        break;
                    case 2: // Use Potion
                        if (Potions > 0)
                        {
                            Console.WriteLine("You used a potion! (+20 HP)");
                            Player.SetHP(Player.GetHP() + 20);
                            Potions -= 1;
                        }
                        else
                        {
                            Console.WriteLine("No more potions left!");
                            Thread.Sleep(1000);
                            continue;
                        }
                        break;
                }

                if (Enemy.GetHP() <= 0)
                {
                    Player.LevelUp();
                    Player.SetGold(Player.GetGold() + 3);
                    Console.WriteLine("You won!");
                    Thread.Sleep(1500);
                    break;
                }

                // Enemy's turn
                Console.WriteLine("Enemy's move...");
                Thread.Sleep(800);
                if (Enemy.GetHP() <= 5 && !enemyUsedPotion)
                {
                    Console.WriteLine("Enemy used a potion! (+5 HP)");
                    Enemy.SetHP(Enemy.GetHP() + 5);
                    enemyUsedPotion = true;
                }
                else
                {
                    Player.SetHP(Math.Max(0, Player.GetHP() - Enemy.GetATTACK()));
                    Console.WriteLine($"Enemy attacked for {Enemy.GetATTACK()} damage.");
                    Console.WriteLine($"Your HP: {Player.GetHP()}");
                }

                if (Player.GetHP() <= 0)
                {
                    Console.WriteLine("You lost!");
                    Thread.Sleep(1500);
                    break;
                }
                Thread.Sleep(1200);
            }
        }

        static void Shop(Player Player)
        {
            string[] shopOptions = {
                "Knife (ATTACK 10) | Price: 30 gold    ",
                "Sword (ATTACK 20) | Price: 60 gold    ",
                "Big Sword (ATTACK 30) | Price: 90 gold",
                "Exit                                  "
            };
            int[] prices = { 30, 60, 90, 0 };
            int[] attacks = { 10, 20, 30, 0 };
            int selected = 0;
            ConsoleKey key;

            Console.CursorVisible = false;
            do
            {
                Console.Clear();
                Console.WriteLine($"╔════════════════════════════════════════════╗");
                Console.WriteLine($"║          RPG SHOP                          ║");
                Console.WriteLine($"╠════════════════════════════════════════════╣");
                Console.WriteLine($"║   Gold: {Player.GetGold(),-28}       ║");
                Console.WriteLine($"╠════════════════════════════════════════════╣");
                for (int i = 0; i < shopOptions.Length; i++)
                {
                    if (i == selected)
                        Console.WriteLine($"║ ▶ {shopOptions[i],-28}   ║");
                    else
                        Console.WriteLine($"║   {shopOptions[i],-28}   ║");
                }
                Console.WriteLine($"╚════════════════════════════════════════════╝");
                Console.WriteLine("Use ↑ ↓ to navigate. Enter to buy. Esc for pause.");

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + shopOptions.Length) % shopOptions.Length;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % shopOptions.Length;
                else if (key == ConsoleKey.Escape)
                {
                    PAUSEMenu(Player);
                    return;
                }
                else if (key == ConsoleKey.Enter)
                {
                    if (selected == 3) // Exit
                        return;
                    if (Player.GetGold() >= prices[selected])
                    {
                        Player.SetGold(Player.GetGold() - prices[selected]);
                        Player.SetATTACK(attacks[selected]);
                        Console.Clear();
                        Console.WriteLine($"You bought {shopOptions[selected].Split('|')[0].Trim()}!");
                        Console.WriteLine($"Your attack is now {attacks[selected]}.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        return;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Not enough gold!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    }
                }
            } while (true);
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