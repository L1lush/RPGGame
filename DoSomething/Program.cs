using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using NAudio.Wave;
using System.ComponentModel;

namespace DoSomething
{
    class Program
    {
        static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string gameFolder = Path.Combine(appData, "DoSomethingGame");
        static string saveFile = Path.Combine(gameFolder, "savegame.json");

        static void Main(string[] args)
        {
            //Random rand = new Random();
            //Player Player = new Player("Knight");
            //Enemy Enemy = new Enemy("Goblin");
            //Test(Player, rand);

            //OpeningStory();
            MainMusicPlayer.Play(); // Play main music
            StartUpMenu();
        }
        //music
        static MusicPlayer MainMusicPlayer = new MusicPlayer("medieval_sound.mp3");
        static MusicPlayer BattleMusicPlayer = new MusicPlayer("bttle_sound.mp3");
        static MusicPlayer CoinMusicPlayer = new MusicPlayer("coin_sound.mp3");
        static MusicPlayer ItemEquipMusicPlayer = new MusicPlayer("item_equip.mp3");



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
                                Directory.CreateDirectory(gameFolder); // Ensure the game folder exists
                                StartNewGame();
                                break;
                            case 1:
                                LoadGame(saveFile);
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
                        break;
                }
            }
        }

        static void SaveGame(Player player, string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(player, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine($"Saving to: {filePath}");
                File.WriteAllText(filePath, json);
                Console.WriteLine("Game saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving game: " + ex.Message);
            }
        }



        static void LoadGame(string filePath)
        {
            Player player = new Player(); // Initialize player to avoid null reference
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Save file not found.");
                }

                string json = File.ReadAllText(filePath);
                player = JsonSerializer.Deserialize<Player>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading game: " + ex.Message);
            }

            if (player == null)
            {
                Console.WriteLine("Failed to load player data. Starting a new game.");
                StartNewGame();
                return;
            }
            Game(player); // Start the game with the loaded player
        }

        static void OpenSettings()
        {
            string[] options = { "Volume", "Back" };
            int selected = 0;
            ConsoleKey key;
            Console.CursorVisible = false;

            bool running = true;

            do
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════╗");
                Console.WriteLine("║         SETTINGS             ║");
                Console.WriteLine("╠══════════════════════════════╣");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                        Console.WriteLine($"║ ▶ {options[i],-27}║");
                    else
                        Console.WriteLine($"║   {options[i],-27}║");
                }
                Console.WriteLine("╚══════════════════════════════╝");
                Console.WriteLine("Use ↑ ↓ to navigate. Enter to select.");

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + options.Length) % options.Length;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % options.Length;
                if(key == ConsoleKey.Enter)
                {
                    switch (selected)
                    {
                        case 0:
                            AdjustMainMusicVolume();
                            break;
                        default:
                            running = false; // Exit settings menu
                            break;
                    }
                }

            } while (running);
        }

        static void AdjustMainMusicVolume()
        {
            int masterVolume = MainMusicPlayer.GetMasterVolume();
            int mainVolume = MainMusicPlayer.Getvolume();
            int battleVolume = BattleMusicPlayer.Getvolume();
            int coinVolume = CoinMusicPlayer.Getvolume();
            int itemEquipVolume = ItemEquipMusicPlayer.Getvolume();
            int selected = 0;
            string[] volumeOptions = 
            {
                "Master volume",
                "Main Music   ",
                "Battle Music ",
                "Coin Sound   ",
                "Item Equip   ",
                "Back         " 
            };
            ConsoleKey key;
            Console.CursorVisible = false;

            do
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════╗");
                Console.WriteLine("║      MUSIC VOLUME         ║");
                Console.WriteLine("╠═══════════════════════════╣");
                for (int i = 0; i < volumeOptions.Length; i++)
                {
                    string volumeDisplay = "";
                    switch (i)
                    {
                        case 0: volumeDisplay = $": {masterVolume,3}"; break;
                        case 1: volumeDisplay = $": {mainVolume,3}"; break;
                        case 2: volumeDisplay = $": {battleVolume,3}"; break;
                        case 3: volumeDisplay = $": {coinVolume,3}"; break;
                        case 4: volumeDisplay = $": {itemEquipVolume,3}"; break;
                        case 5: volumeDisplay = ""; break;
                    }
                    if (i == selected)
                    {
                        Console.WriteLine($"║ ▶ {volumeOptions[i]} {volumeDisplay,-10}║");
                        switch (i)
                        {
                            case 0: break;
                            case 1:
                                MainMusicPlayer.Play();
                                BattleMusicPlayer.Pause();
                                CoinMusicPlayer.Pause();
                                ItemEquipMusicPlayer.Pause();
                                break;
                            case 2:
                                BattleMusicPlayer.Play();
                                MainMusicPlayer.Pause();
                                CoinMusicPlayer.Pause();
                                ItemEquipMusicPlayer.Pause();
                                break;
                            case 3:
                                CoinMusicPlayer.Play();
                                MainMusicPlayer.Pause();
                                BattleMusicPlayer.Pause();
                                ItemEquipMusicPlayer.Pause();
                                break;
                            case 4: 
                                ItemEquipMusicPlayer.Play();
                                MainMusicPlayer.Pause();
                                BattleMusicPlayer.Pause();
                                CoinMusicPlayer.Pause();
                                break;
                            case 5:
                                MainMusicPlayer.Play();
                                ItemEquipMusicPlayer.Pause();
                                BattleMusicPlayer.Pause();
                                CoinMusicPlayer.Pause();
                                break;
                        }
                    }
                    else
                        Console.WriteLine($"║   {volumeOptions[i]} {volumeDisplay,-10}║");
                }
                Console.WriteLine("╚═══════════════════════════╝");
                Console.WriteLine("Use ↑ ↓ to select, ← → to adjust.");

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + volumeOptions.Length) % volumeOptions.Length;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % volumeOptions.Length;
                else if (key == ConsoleKey.LeftArrow)
                {
                    switch (selected)
                    {
                        case 0: masterVolume = Math.Max(0, masterVolume - 5); MainMusicPlayer.SetMasterVolume(masterVolume); break;
                        case 1: mainVolume = Math.Max(0, mainVolume - 5); MainMusicPlayer.Setvolume(mainVolume); break;
                        case 2: battleVolume = Math.Max(0, battleVolume - 5); BattleMusicPlayer.Setvolume(battleVolume); break;
                        case 3: coinVolume = Math.Max(0, coinVolume - 5); CoinMusicPlayer.Setvolume(coinVolume); break;
                        case 4: itemEquipVolume = Math.Max(0, itemEquipVolume - 5); ItemEquipMusicPlayer.Setvolume(itemEquipVolume); break;
                    }
                }
                else if (key == ConsoleKey.RightArrow)
                {
                    switch (selected)
                    {
                        case 0: masterVolume = Math.Min(100, masterVolume + 5); MainMusicPlayer.SetMasterVolume(masterVolume); break;
                        case 1: mainVolume = Math.Min(100, mainVolume + 5); MainMusicPlayer.Setvolume(mainVolume); break;
                        case 2: battleVolume = Math.Min(100, battleVolume + 5); BattleMusicPlayer.Setvolume(battleVolume); break;
                        case 3: coinVolume = Math.Min(100, coinVolume + 5); CoinMusicPlayer.Setvolume(coinVolume); break;
                        case 4: itemEquipVolume = Math.Min(100, itemEquipVolume + 5); ItemEquipMusicPlayer.Setvolume(itemEquipVolume); break;
                    }
                }
                else if(key == ConsoleKey.Enter && selected == 5)
                {
                    // Exit the volume adjustment menu
                    MainMusicPlayer.Play();
                    BattleMusicPlayer.Pause(); // Pause battle music
                    CoinMusicPlayer.Pause(); // Pause coin sound
                    ItemEquipMusicPlayer.Pause(); // Pause item equip sound
                    break;
                }
            } while (true);
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
            Game(Player); // Start the game with the selected class
        }

        static void Game(Player player)
        {
            Console.Clear();
            Console.WriteLine(player.GETLVL());
            Random rand = new Random();
            while (true)
            {
                string[] options = {
                        "Forest",
                        "Cave",
                        "Castle",
                        "Shop",
                        "Stats",
                    };
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
                        PAUSEMenu(player);
                        continue;
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        switch (options[selected])
                        {
                            case "Forest":
                                Forest(player, rand); // ADD MAP
                                break;
                            case "Cave":
                                Cave(player, rand); // ADD MAP
                                break;
                            case "Castle":
                                Castle(player, rand);
                                break;
                            case "Stats":
                                player.ShowStats();
                                Thread.Sleep(3000);
                                break;
                            case "Shop":
                                Shop(player);
                                break;
                        }
                    }
                } while (true);

            }
        }

        static void PAUSEMenu(Player player)
        {
            string[] options = { "Resume", "Stats", "Save Game", "Exit to Main Menu" };
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
                    // Save Game
                    Console.WriteLine("Saving game...");
                    SaveGame(player, saveFile);
                    Console.WriteLine("Game saved!");
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey(true);
                    PAUSEMenu(player);
                    break;
                case 3:
                    // Exit to main menu
                    Console.WriteLine("Press Enter to continue...");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter)
                    {
                        PAUSEMenu(player);
                    }
                    StartUpMenu();
                    break;
            }
        }


        static void Forest(Player Player, Random rand) // change ADD MAP

        {
            char[,] map = MapForest();
            MoveOnMap(map, Player, rand);
        }

        static void Cave(Player Player, Random rand)
        {
            char[,] map = MapCave();
            MoveOnMap(map, Player, rand);
        }

        static void Castle(Player Player, Random rand)
        {
            char[,] map = MapCastle();
            MoveOnMap(map, Player, rand);

        }

        static void Chest(Player Player, char[,] map)
        {
            bool playedSound = false;
            switch (map[0, 0])
            {
                case '1':
                    ChestCastle(Player);
                    break;
                case '2':
                    ChestCave(Player);
                    break;
                case '3':
                    ChestForest(Player);
                    break;
            }
            if(!playedSound)
            {
                CoinMusicPlayer.Play(); // Play coin sound
                Thread.Sleep(400);
                CoinMusicPlayer.Pause(); // Stop coin sound
                playedSound = true;
            }
        }

        static void ChestForest(Player Player)
        {
            Console.WriteLine("You got 15 Gold");
            Player.AddGold(15);
        }

        static void ChestCave(Player Player)
        {
            Console.WriteLine("You got 30 Gold");
            Player.AddGold(30);
        }

        static void ChestCastle(Player Player)
        {
            Console.WriteLine("You got 45 Gold");
            Player.AddGold(45);
        }

        static void Battle(Player Player, Enemy Enemy, Random rand)
        {
            bool enemyUsedPotion = false;
            MainMusicPlayer.Pause(); // Pause battle music
            BattleMusicPlayer.Play(); // Play battle music
            while (Player.GetHP() > 0 && Enemy.GetHP() > 0)
            {
                // Action selection menu
                string[] actions = {
                    "Attack                     ",
                    "Run                        ",
                   $"Use Potion ({Player.GetPositions()} left)        "
                };
                int selectedAction = 0;
                ConsoleKey key;
                do
                {
                    Console.Clear();
                    Console.WriteLine("╔═══════════════════════════════╗");
                    Console.WriteLine("║   Choose Your Action          ║");
                    Console.WriteLine("╠═══════════════════════════════╣");
                    Console.WriteLine($"║   Player HP: {Player.GetHP(),-16} ║");
                    Console.WriteLine($"║   Player Max HP: {Player.GetMaxHP(),-12} ║");
                    Console.WriteLine($"║   Player ATTACK: {Player.GetATTACK(),-12} ║");
                    Console.WriteLine($"║   Enemy HP: {Enemy.GetHP(),-16}  ║");
                    Console.WriteLine($"║   Enemy ATTACK: {Enemy.GetATTACK(),-13} ║");
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
                        Enemy.SubtractHP(Player.GetATTACK());
                        Console.WriteLine($"You attacked! Enemy HP is now {Enemy.GetHP()}");
                        break;
                    case 1: // Run
                        int num = rand.Next(1, 3);
                        if (num == 1)
                        {
                            Console.WriteLine("You ran away!");
                            BattleMusicPlayer.Pause(); // Pause battle music
                            MainMusicPlayer.Play(); // Play main music again
                            Thread.Sleep(1000);
                            return;
                        }
                        else
                        {
                            Player.SubtractHP(Enemy.GetATTACK());
                            Console.WriteLine($"Failed to run! Enemy attacked for {Enemy.GetATTACK()} damage.");
                            Console.WriteLine($"Your HP: {Player.GetHP()}");
                        }
                        break;
                    case 2: // Use Potion
                        if (Player.GetPositions() > 0)
                        {
                            Console.WriteLine("You used a potion! (+20 HP)");
                            Player.AddHP(20);
                            Player.SubtractPositions(1);
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
                    Player.LevelUp(Enemy.GetXp());
                    Player.AddGold(3); // Add 3 gold for winning
                    Console.WriteLine("You won!");
                    BattleMusicPlayer.Pause(); // Pause battle music
                    MainMusicPlayer.Play(); // Play main music again
                    Thread.Sleep(1500);
                    break;
                }

                // Enemy's turn
                Console.WriteLine("Enemy's move...");
                Thread.Sleep(800);
                if (Enemy.GetHP() <= 5 && !enemyUsedPotion)
                {
                    Console.WriteLine("Enemy used a potion! (+5 HP)");
                    Enemy.AddHP(5);
                    enemyUsedPotion = true;
                }
                else
                {
                    Player.SubtractHP(Enemy.GetATTACK());
                    Console.WriteLine($"Enemy attacked for {Enemy.GetATTACK()} damage.");
                    Console.WriteLine($"Your HP: {Player.GetHP()}");
                }

                if (Player.GetHP() <= 0)
                {
                    Console.WriteLine("You lost!");
                    Console.WriteLine("Game Over! Returning to main menu...");
                    BattleMusicPlayer.Pause(); // Pause battle music
                    MainMusicPlayer.Play(); // Play main music again
                    Thread.Sleep(2500);
                    StartUpMenu();
                    break;
                }
                Thread.Sleep(1200);
            }
        }

        static void Shop(Player Player)
        {
            string[] shopOptions = {
                "Knife (ATTACK 10) | Price: 30 gold       ",
                "Sword (ATTACK 20) | Price: 70 gold       ",
                "Big Sword (ATTACK 30) | Price: 100 gold  ",
                "Potion (Restores 20 HP) | Price: 20 gold ",
                "Exit                                     "
            };
            int[] prices = { 30, 70, 100, 20, 0 };
            int[] attacks = { 10, 20, 30, 0, 0 };
            int selected = 0;
            ConsoleKey key;

            Console.CursorVisible = false;
            do
            {
                Console.Clear();
                Console.WriteLine($"╔═══════════════════════════════════════════════╗");
                Console.WriteLine($"║           RPG SHOP                            ║");
                Console.WriteLine($"╠═══════════════════════════════════════════════╣");
                Console.WriteLine($"║   Gold: {Player.GetGold(),-28}          ║");
                Console.WriteLine($"╠═══════════════════════════════════════════════╣");
                for (int i = 0; i < shopOptions.Length; i++)
                {
                    if (i == selected)
                        Console.WriteLine($"║ ▶ {shopOptions[i],-28}   ║");
                    else
                        Console.WriteLine($"║   {shopOptions[i],-28}   ║");
                }
                Console.WriteLine($"╚═══════════════════════════════════════════════╝");
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
                    if (selected == 4) // Exit
                        return;
                    if (selected == 3) // Buy potion
                    {
                        if (Player.GetGold() >= prices[selected])
                        {
                            if (Player.GetPositions() < 10)
                            {
                                Player.SubtractGold(prices[selected]);
                                Player.AddPositions(1);
                                Console.Clear();
                                Console.WriteLine("You bought a potion!");
                                Console.WriteLine($"You now have {Player.GetPositions()} potions.");
                                ItemEquipMusicPlayer.Play(); // Play item equip sound
                                Thread.Sleep(400);
                                ItemEquipMusicPlayer.Pause(); // Stop item equip sound
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("You can't carry more potions!");
                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Not enough gold!");
                            Thread.Sleep(1000);
                        }
                        continue; // Skip further processing for potions
                    }
                    if (Player.GetGold() >= prices[selected])
                    {
                        Player.SubtractGold(prices[selected]);
                        Player.AddATTACK(attacks[selected]);
                        Console.Clear();
                        Console.WriteLine($"You bought {shopOptions[selected].Split('|')[0].Trim()}!");
                        Console.WriteLine($"Your attack is now {attacks[selected]}.");
                        ItemEquipMusicPlayer.Play(); // Play item equip sound
                        Thread.Sleep(400);
                        ItemEquipMusicPlayer.Pause(); // Stop item equip sound
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Not enough gold!");
                        Thread.Sleep(1000);
                    }
                }
            } while (true);
        }

        static void OpeningStory()
        {
            Console.WriteLine("Kael, Son of a God");
            Console.WriteLine("Press Enter to skip the intro or any other key to watch the story...");

            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Enter)
            {
                Console.Clear();
                return;
            }

            Console.Clear();
            ShowStoryLine("Kael was raised as an ordinary villager — quiet, kind, and curious.");
            ShowStoryLine("The people of Nerith Hollow treated him like family, but Kael always felt different.");
            ShowStoryLine("He had no memory of his real parents, only a pendant he wore since birth — glowing faintly when he was near danger.");
            ShowStoryLine("What he didn't know was that he is the son of the god Aetherion...");
            ShowStoryLine("The ancient god of balance and light. Aetherion had fallen in love with a mortal woman, Kael’s mother.");
            ShowStoryLine("To protect them from divine enemies, he sealed his power and hid Kael in the mortal world.");

            Console.WriteLine();
            ShowStoryLine("One night, under a blood-red moon, shadow beasts from the Umbraverse descended upon the village...");
            ShowStoryLine("The skies cracked with lightning, and flames swallowed the homes Kael knew.");
            ShowStoryLine("He fought to save his friends, but he was too weak — forced to watch as the monsters slaughtered everyone he loved.");
            ShowStoryLine("In that moment of pain, rage, and sorrow, something awoke inside him...");
            ShowStoryLine("Time slowed, the pendant shattered, and divine energy surged through him.");
            ShowStoryLine("He destroyed the creatures with light that came from within his own body.");
            ShowStoryLine("When he awoke, the village was ash.");

            Console.WriteLine();
            Thread.Sleep(3000);
            Console.Clear();
        }
        static void ShowStoryLine(string text)
        {
            Console.WriteLine(text);
            Thread.Sleep(4000);
        }

        static char[,] MapCastle() // Create map for castle // later ADD for forest and cave
        {
            // C - Chest || D - Dragon || G - Goblin
            char[,] map = new char[10, 10]
            {
                { '1', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'P', ' ', ' ', '#', 'C', ' ', ' ', ' ', '#' },
                { '#', '#', '#', ' ', '#', ' ', ' ', '#', ' ', '#' },
                { '#', ' ', ' ', ' ', 'G', ' ', ' ', '#', 'G', '#' },
                { '#', ' ', '#', '#', '#', '#', ' ', '#', ' ', '#' },
                { '#', ' ', '#', 'C', 'D', ' ', ' ', '#', 'G', '#' },
                { '#', ' ', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', ' ', '#', ' ', ' ', 'D', ' ', '#', ' ', '#' },
                { '#', ' ', ' ', 'D', '#', ' ', 'C', '#', 'X', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
            };

            return map;
        }

        static char[,] MapCave()
        {
            char[,] map = new char[10, 10]
            {
                { '2', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'P', '#', '#', ' ', 'S', 'C', '#', '#', '#' },
                { '#', ' ', '#', '#', ' ', ' ', '#', '#', '#', '#' },
                { '#', ' ', ' ', 'G', ' ', ' ', '#', '#', '#', '#' },
                { '#', '#', '#', ' ', '#', '#', ' ', ' ', '#', '#' },
                { '#', '#', '#', ' ', '#', '#', ' ', ' ', ' ', '#' },
                { '#', 'G', ' ', ' ', ' ', ' ', 'G', ' ', ' ', '#' },
                { '#', 'S', '#', '#', ' ', ' ', '#', '#', ' ', '#' },
                { '#', 'C', '#', '#', ' ', ' ', '#', '#', 'X', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
            };
            return map;
        }

        static char[,] MapForest()
        {
            char[,] map = new char[10, 10]
            {
                { '3', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'P', ' ', ' ', ' ', '#', 'C', '#', 'X', '#' },
                { '#', ' ', '#', ' ', ' ', ' ', 'G', '#', 'G', '#' },
                { '#', '#', '#', '#', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', 'C', '#', ' ', ' ', '#', '#', '#', '#', '#' },
                { '#', 'G', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', '#', '#', '#', ' ', '#' },
                { '#', '#', '#', '#', ' ', '#', 'C', '#', ' ', '#' },
                { '#', 'C', 'G', 'G', ' ', '#', ' ', 'G', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
            };

            return map;
        }

        static void MoveOnMap(char[,] map, Player Player, Random rand) // this function Draw the map and player need to move by using arrows also if you touch letter it will call another function for battle or get Chest
        {
            int playerX = 0, playerY = 0;

            // Find player start position
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == 'P')
                    {
                        playerX = x;
                        playerY = y;
                        break;
                    }
                }
            }

            while (true)
            {
                Console.Clear();
                PrintMap(map);

                Console.WriteLine("Move: ↑ ↓ ← → ");
                Console.WriteLine("Press Escape to pause.");
                Console.WriteLine("'D' is Dragon, 'G' is Goblin, 'S' is Skeleton, 'C' is Chest, 'X' is Exit, 'P' is you.");

                ConsoleKey key = Console.ReadKey(true).Key;
                int newX = playerX, newY = playerY;

                // Movement with arrow keys
                switch (key)
                {
                    case ConsoleKey.UpArrow: newY--; break;
                    case ConsoleKey.DownArrow: newY++; break;
                    case ConsoleKey.LeftArrow: newX--; break;
                    case ConsoleKey.RightArrow: newX++; break;
                    case ConsoleKey.Escape: PAUSEMenu(Player); break;
                }

                // Check bounds and wall collision
                if (newX < 0 || newX >= map.GetLength(1) ||
                    newY < 0 || newY >= map.GetLength(0) ||
                    map[newY, newX] == '#') continue;

                char destination = map[newY, newX];

                // Handle interactions
                switch (destination)
                {
                    case 'D': Enemy EnemyD = new Enemy("Dragon", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyD.GetClass()}"); Battle(Player, EnemyD, rand); break;
                    case 'C': Chest(Player, map); break;
                    case 'G': Enemy EnemyG = new Enemy("Goblin", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyG.GetClass()}"); Battle(Player, EnemyG, rand); break;
                    case 'S': Enemy EnemyS = new Enemy("Skeleton", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyS.GetClass()}"); Battle(Player, EnemyS, rand); break;
                    case 'X':
                        Console.WriteLine("You reached the exit! Game Over.");
                        return;
                }

                // Move player
                map[playerY, playerX] = '.';
                playerX = newX;
                playerY = newY;
                map[playerY, playerX] = 'P';
            }
        }

        static void PrintMap(char[,] map) // this function print map (used in MoveOnMap)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Console.Write($"{map[y, x]} ");
                }
                Console.WriteLine();
            }
        }
    }
}