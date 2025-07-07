using NAudio.Codecs;
using NAudio.Dmo;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoSomething
{
    class Program
    {
        static Quest killEnemies = new Quest("Monster Slayer", "Kill 5 enemies.", 5);
        static Quest openChests = new Quest("Treasure Hunter", "Open 3 chests.", 3);
        static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string gameFolder = Path.Combine(appData, "DoSomethingGame");
        static string saveFile = Path.Combine(gameFolder, "savegame.json");
        static char[,] villageMap = VillageMap();
        static void Main(string[] args)
        {
            MainMusicPlayer.Play(); // Play main music
            StartUpMenu();
        }
        //music
        static MusicPlayer MainMusicPlayer = new MusicPlayer("medieval_sound.mp3");
        static MusicPlayer BattleMusicPlayer = new MusicPlayer("bttle_sound.mp3");
        static MusicPlayer CoinMusicPlayer = new MusicPlayer("coin_sound.mp3");
        static MusicPlayer ItemEquipMusicPlayer = new MusicPlayer("item_equip.mp3");

        //Shop
        static int shopVisitCount = 0;
        static List<Weapon>? cachedShopWeapons = null;



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
                // Serialize both player and weapon
                var saveData = new
                {
                    Player = player,
                };
                string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
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
                // Deserialize the save data object
                var saveData = JsonSerializer.Deserialize<
                    SaveData>(json);
                if (saveData != null)
                {
                    player = saveData.Player;
                    // If weapon is present in save, set it
                    if (saveData.Weapon != null)
                    {
                        player.SetWeapon(saveData.Weapon);
                    }
                }
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

        // Helper class for load
        private class SaveData
        {
            public Player Player { get; set; }
            public Weapon Weapon { get; set; }
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
                if (key == ConsoleKey.Enter)
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
                else if (key == ConsoleKey.Enter && selected == 5)
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

            string[] classOptions = { "Knight", "Assassin", "Mage", "Archer" };
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

            OpeningStory();

            Player Player = new Player(classOptions[selectedClass]);
            Game(Player); // Start the game with the selected class
        }

        static void OpeningStory()
        {
            Console.Clear();
            MainMusicPlayer.Pause(); // Pause main music
            string[] storyLines = new[]
            {
                "Long ago, the world of Eldrath was united under the immortal king Valdaran and powered by the ancient Heartstones.",
                "",
                "But betrayal shattered the empire.",
                "The Heartstones were lost.",
                "The world broke.",
                "",
                "Now, centuries later, chaos reigns.",
                "Monsters roam, kingdoms crumble, and the gods remain silent.",
                "",
                "You awaken with a strange mark — a sign of forgotten power.",
                "The Heartstones call once more.",
                "And they call for you...", 
                "to rule the world.",
                "",
                "Your journey begins now."
            };

            foreach (string line in storyLines)
            {
                Console.WriteLine(line);
                Thread.Sleep(2000);
            }
            Console.ReadKey(true);
            MainMusicPlayer.Play(); // Resume main music
        }

        static void Game(Player player)
        {
            Console.Clear();
            Console.WriteLine(player.GETLVL());
            Random rand = new Random();
            while (true)
            {
                string[] options = {
            "Cave",           // Swapped
            "Forest (LVL 3)", // Swapped + level display
            "Castle (LVL 5)",
            "Boss Fight (LVL 10)",
            "Village",
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
                            case "Forest (LVL 3)":
                                if (player.GETLVL() >= 3)
                                    Forest(player, rand);
                                else
                                {
                                    Console.WriteLine("You need to be at least LVL 3 to enter the Forest!");
                                    Thread.Sleep(1500);
                                }
                                break;
                            case "Cave":
                                Cave(player, rand);
                                break;
                            case "Castle (LVL 5)":
                                if (player.GETLVL() >= 5)
                                    Castle(player, rand);
                                else
                                {
                                    Console.WriteLine("You need to be at least LVL 5 to enter the Castle!");
                                    Thread.Sleep(1500);
                                }
                                break;
                            case "Village":
                                MoveOnMap(villageMap, player, rand);
                                break;
                            case "Boss Fight (LVL 10)":
                                if (player.GETLVL() >= 10)
                                    BossFight(player, rand);
                                else
                                {
                                    Console.WriteLine("You need to be at least LVL 10 for Boss Fight!");
                                    Thread.Sleep(1500);
                                }
                                break;
                        }
                    }
                } while (true);
            }
        }


        static void PAUSEMenu(Player player)
        {
            string[] options = { "Resume", "Stats", "Achievements", "Save Game", "Exit to Main Menu" };
          
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
                        Console.WriteLine($"║ ▶ {options[i],-17}   ║");
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
                    Console.Clear();
                    return;
                case 1:
                    player.ShowStats();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey(true);
                    PAUSEMenu(player);
                    break;
                case 2:
                    Console.WriteLine(player.ShowAchievements());
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey(true);
                    PAUSEMenu(player);
                    break;
                case 3:
                    Console.WriteLine("Saving game...");
                    SaveGame(player, saveFile);
                    Console.WriteLine("Game saved!");
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey(true);
                    PAUSEMenu(player);
                    break;
                case 4:
                    // Exit to main menu
                    Console.WriteLine("Press Enter to continue...");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter)
                    {
                        PAUSEMenu(player);
                    }
                    SaveGame(player, saveFile); // Save the game before exiting
                    StartUpMenu();
                    break;
            }
        }



        static void Forest(Player Player, Random rand)
        {
            char[,] map = MapGenerator.GenerateMazeWithChestsAndEnemies("forest", 21, 21);
            Player.ForestVisits += 1;
            if(Player.ForestVisits >= 1 && Player.CaveVisits >= 1 && Player.CastleVisits >= 1 && Player.BossFightVisits >= 1)
            {
                Player.UnlockAchievement("Explorer");
            }
            map[0, 0] = '3';
            MoveOnMap(map, Player, rand);
        }

        static void Cave(Player Player, Random rand)
        {
            char[,] map = MapGenerator.GenerateMazeWithChestsAndEnemies("cave", 21, 21);
            Player.CaveVisits += 1; 
            if (Player.ForestVisits >= 1 && Player.CaveVisits >= 1 && Player.CastleVisits >= 1 && Player.BossFightVisits >= 1)
            {
                Player.UnlockAchievement("Explorer");
            }
            map[0, 0] = '2';
            MoveOnMap(map, Player, rand);
        }

        static void Castle(Player Player, Random rand)
        {
            char[,] map = MapGenerator.GenerateMazeWithChestsAndEnemies("castle", 21, 21);
            Player.CastleVisits += 1; 
            if (Player.ForestVisits >= 1 && Player.CaveVisits >= 1 && Player.CastleVisits >= 1 && Player.BossFightVisits >= 1)
            {
                Player.UnlockAchievement("Explorer");
            }
            map[0, 0] = '1';
            MoveOnMap(map, Player, rand);
        }

        static void BossFight(Player Player, Random rand) 
        {
            Player.BossFightVisits += 1; 
            if (Player.ForestVisits >= 1 && Player.CaveVisits >= 1 && Player.CastleVisits >= 1 && Player.BossFightVisits >= 1)
            {
                Player.UnlockAchievement("Explorer");
            }
            char[,] map = BossMap();
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
            if (!playedSound)
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
                    EnemyKilled(ref killEnemies);

                    Player.Kills += 1; // Increment kills
                    if (Player.Kills == 1)
                    {
                        Console.WriteLine("Achievement unlocked: First Blood!");
                        Player.UnlockAchievement("First Blood");
                        Thread.Sleep(2000);
                    }
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

        static void Shop(Player Player, Random random)
        {
            Player.ShopVisits += 1;

            shopVisitCount++;

            if (shopVisitCount % 5 == 0 || cachedShopWeapons == null)
            {
                // Generate new shop weapons
                int random1, random2, random3;
                do
                {
                    random1 = random.Next(0, 7);
                    random2 = random.Next(0, 7);
                    random3 = random.Next(0, 7);
                } while (random1 == random2 || random1 == random3 || random2 == random3);

                Weapon weapon1 = new Weapon();
                Weapon weapon2 = new Weapon();
                Weapon weapon3 = new Weapon();

                string[] types = { "Sword", "Big Sword", "Axe", "Bow", "Spear", "Hammer", "Dagger" };
                weapon1.SetType(types[random1]);
                weapon2.SetType(types[random2]);
                weapon3.SetType(types[random3]);

                cachedShopWeapons = new List<Weapon> { weapon1, weapon2, weapon3 };
                shopVisitCount = 0;
            }

            List<Weapon> shopWeapons = new List<Weapon>(cachedShopWeapons);

            string potionName = "Potion (Restores 20 HP)";
            int potionPrice = 20;
            int selected = 0;
            ConsoleKey key;

            Console.CursorVisible = false;


            do
            {
                Console.Clear();
                // Replace the shop display section with this for perfect ║ alignment
                Console.WriteLine($"╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine($"║                        RPG SHOP                              ║");
                Console.WriteLine($"╠══════════════════════════════════════════════════════════════╣");
                Console.WriteLine($"║   Gold: {Player.GetGold(),-22}                               ║");
                if(Player.GetPositions() >= 10)
                {
                    Console.WriteLine($"║   Potions: {Player.GetPositions()} left                                           ║");
                }
                else
                {
                    Console.WriteLine($"║   Potions: {Player.GetPositions()} left                                            ║");
                }
                Console.WriteLine($"║   Your weapon attack: {Player.GetWeapon().GetATTACK(),-21}                  ║");
                Console.WriteLine($"╠══════════════════════════════════════════════════════════════╣");

                // Display weapons
                for (int i = 0; i < shopWeapons.Count; i++)
                {
                    string weaponName = shopWeapons[i].GetName();
                    int weaponAttack = shopWeapons[i].GetATTACK();
                    int weaponPrice = shopWeapons[i].GetPrice();
                    bool isEquipped = Player.GetWeapon() != null && Player.GetWeapon().GetName() == weaponName;
                    string equippedText = isEquipped ? " (Equipped)" : "";
                    string line = $"{(i == selected ? "▶" : " ")} {weaponName} (ATTACK {weaponAttack}) | Price: {weaponPrice} gold{equippedText}";
                    Console.WriteLine($"║ {line,-61}║");
                }

                int potionIndex = shopWeapons.Count;
                int upgradeIndex = potionIndex + 1;
                int exitIndex = potionIndex + 2;

                // Potion
                string potionLine = $"{(selected == potionIndex ? "▶" : " ")} {potionName}";
                Console.WriteLine($"║ {potionLine,-61}║");

                // Upgrade Weapon
                string upgradeLine = $"{(selected == upgradeIndex ? "▶" : " ")} Upgrade Weapon (+10 ATK) | 100 gold";
                Console.WriteLine($"║ {upgradeLine,-61}║");

                // Exit
                string exitLine = $"{(selected == exitIndex ? "▶" : " ")} Exit";
                Console.WriteLine($"║ {exitLine,-61}║");

                Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");
                Console.WriteLine("Use ↑ ↓ to navigate. Enter to buy. Esc for pause.");

                key = Console.ReadKey(true).Key;
                int optionsCount = shopWeapons.Count + 3;

                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + optionsCount) % optionsCount;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % optionsCount;
                else if (key == ConsoleKey.Escape)
                {
                    PAUSEMenu(Player);
                    return;
                }
                else if (key == ConsoleKey.Enter)
                {
                    if (selected == exitIndex)
                        return;

                    if (selected == potionIndex)
                    {
                        if (Player.GetGold() >= potionPrice)
                        {
                            if (Player.GetPositions() < 10)
                            {
                                Player.SubtractGold(potionPrice);
                                Player.AddPositions(1);
                                Console.Clear();
                                Console.WriteLine("You bought a potion!");
                                Console.WriteLine($"You now have {Player.GetPositions()} potions.");
                                ItemEquipMusicPlayer.Play();
                                Thread.Sleep(400);
                                ItemEquipMusicPlayer.Pause();
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
                        continue;
                    }

                    else if (selected == upgradeIndex)
                    {
                        int upgradeCost = 100;
                        if (Player.GetWeapon() == null)
                        {
                            Console.Clear();
                            Console.WriteLine("You don't have a weapon to upgrade!");
                            Thread.Sleep(1000);
                        }
                        else if (Player.GetGold() < upgradeCost)
                        {
                            Console.Clear();
                            Console.WriteLine("Not enough gold to upgrade!");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Player.SubtractGold(upgradeCost);
                            Player.AddATTACK(10);
                            Console.Clear();
                            Console.WriteLine("Weapon upgraded! +10 Attack.");
                            ItemEquipMusicPlayer.Play();
                            Thread.Sleep(400);
                            ItemEquipMusicPlayer.Pause();
                            Thread.Sleep(1000);
                        }
                        continue;
                    }

                    else if (selected < shopWeapons.Count)
                    {
                        Weapon weaponToBuy = shopWeapons[selected];
                        int price = weaponToBuy.GetPrice();

                        if (Player.GetWeapon() != null && Player.GetWeapon().GetATTACK() == weaponToBuy.GetATTACK())
                        {
                            Console.Clear();
                            Console.WriteLine("You already have this weapon equipped!");
                            Thread.Sleep(1000);
                            continue;
                        }

                        if (Player.GetGold() >= price)
                        {
                            Player.SubtractGold(price);
                            Player.RemoveWeapon();
                            Player.SetWeapon(weaponToBuy);
                            Console.Clear();
                            Console.WriteLine($"You bought and equipped {weaponToBuy.GetName()}!");
                            ItemEquipMusicPlayer.Play();
                            Thread.Sleep(400);
                            ItemEquipMusicPlayer.Pause();
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Not enough gold!");
                            Thread.Sleep(1000);
                        }
                    }
                }
            } while (true);
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
                if(map[0, 0] != '4')
                    Console.WriteLine("'D' Dragon, 'G' Goblin, 'S' Skeleton, 'O' Orc, 'T' Troll, 'V' Vampire, 'L' Slime, 'B' Bandit, 'C' Chest, 'X' Exit, 'P' you, 'b' Boss.");
                else
                    Console.WriteLine("'c' Casino, 's' Shop, 'v' to talk to Villager, 'P' you.");

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
                    case 'O': Enemy EnemyO = new Enemy("Orc", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyO.GetClass()}"); Battle(Player, EnemyO, rand); break;
                    case 'T': Enemy EnemyT = new Enemy("Troll", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyT.GetClass()}"); Battle(Player, EnemyT, rand); break;
                    case 'V': Enemy EnemyV = new Enemy("Vampire", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyV.GetClass()}"); Battle(Player, EnemyV, rand); break;
                    case 'L': Enemy EnemyL = new Enemy("Slime", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyL.GetClass()}"); Battle(Player, EnemyL, rand); break;
                    case 'B': Enemy EnemyB = new Enemy("Bandit", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyB.GetClass()}"); Battle(Player, EnemyB, rand); break;
                    case 'C': ChestOpened(ref openChests); Chest(Player, map); break;
                    case 'c': Casino(Player); break;
                    case 's': Shop(Player, rand); break;
                    case 'v': TalkToVillager(ref killEnemies, ref openChests, Player); break;
                    case 'b': Enemy Enemyb = new Enemy("Boss", Player.GETLVL()); Console.WriteLine($"you attacked by {Enemyb.GetClass()}"); Battle(Player, Enemyb, rand); break;
                    case 'G': Enemy EnemyG = new Enemy("Goblin", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyG.GetClass()}"); Battle(Player, EnemyG, rand); break;
                    case 'S': Enemy EnemyS = new Enemy("Skeleton", Player.GETLVL()); Console.WriteLine($"you attacked by {EnemyS.GetClass()}"); Battle(Player, EnemyS, rand); break;
                    case 'X': villageMap = VillageMap();
                        Console.WriteLine("You reached the exit! Game Over.");
                        return;
                }

                // Move player
                if (map[0, 0] != '4' && (destination != 'c' || destination != 's' || destination != 'v'))
                {
                    map[playerY, playerX] = '.';
                }
                else
                {
                    if (destination == 'c' || destination == 's' || destination == 'v')
                    {
                        map[playerY, playerX] = destination; // Keep the current tile if it's a special location
                       
                        int oldX = newX, oldY = newY; // Store old position

                        switch (key)
                        {
                            case ConsoleKey.UpArrow: oldY++; break;
                            case ConsoleKey.DownArrow: oldY--; break;
                            case ConsoleKey.LeftArrow: oldX++; break;
                            case ConsoleKey.RightArrow: oldX--; break;
                        }
                        newX = oldX; // Reset newX to oldX
                        newY = oldY; // Reset newY to oldY
                    }
                    else
                    {
                        map[playerY, playerX] = ' '; // Clear the tile if it's not a special location
                    }
                }
                playerX = newX;
                playerY = newY;
                map[playerY, playerX] = 'P'; // Place player on the new tile
                CheckExplorer(map, Player);
            }
        }

        static void PrintMap(char[,] map) // this function print map (used in MoveOnMap)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Console.Write($"{map[y, x]} ");
                }
                Console.WriteLine();
            }
        }

        static char[,] VillageMap()
        {
            char[,] map = new char[10, 10];

            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    map[y, x] = ' ';

            for (int i = 0; i < 10; i++)
            {
                map[0, i] = '#';
                map[9, i] = '#';
                map[i, 0] = '#';
                map[i, 9] = '#';
            }

            for (int y = 2; y <= 4; y++)
                for (int x = 2; x <= 4; x++)
                    map[y, x] = '#';

            map[3, 3] = 'v';
            map[4, 3] = ' ';

            map[1, 7] = 'c';
            map[0, 7] = '#'; map[2, 7] = '#';
            map[1, 6] = '#'; map[1, 8] = '#';
            map[2, 7] = ' ';

            for (int y = 6; y <= 8; y++)
                for (int x = 1; x <= 3; x++)
                    map[y, x] = '#';

            map[7, 2] = 's';
            map[6, 2] = ' ';

            map[8, 5] = 'P';
            map[0, 0] = '4';
            map[8, 8] = 'X';
            return map;
        }

        static void Casino(Player player)
        {
            string[] lines =
            {
                "╔════════════════════════════════════╗",
                "║            🎰 CASINO 🎰            ║",
                "╠════════════════════════════════════╣",
                "║     Blackjack                      ║",
                "║     Dice Duel                      ║",
                "║     Roulette                       ║",
                "║     Exit                           ║",
                "╚════════════════════════════════════╝"
            };

            int selectedIndex = 0;
            int menuStartRow = 3;
            int totalOptions = 4;

            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (i >= menuStartRow && i < menuStartRow + totalOptions)
                    {
                        int optionIndex = i - menuStartRow;
                        if (optionIndex == selectedIndex)
                            line = line.Substring(0, 4) + "▶" + line.Substring(5);
                        else
                            line = line.Substring(0, 4) + " " + line.Substring(5);
                    }

                    Console.WriteLine(line);
                }

                Console.WriteLine("Use ↑ ↓ to navigate. Press Enter to select.");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % totalOptions;
                        break;
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + totalOptions) % totalOptions;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        switch (selectedIndex)
                        {
                            case 0:
                                Blackjack(player);
                                break;
                            case 1:
                                DiceDuel(player);
                                break;
                            case 2:
                                Roulette(player);
                                break;
                            case 3:
                                return; // Exit casino
                        }

                        Console.WriteLine("\nPress any key to return to the casino menu...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        static void Blackjack(Player player)
        {
            Console.WriteLine("♠️ Welcome to Blackjack!");
            Console.Write("Enter your stake: ");
            if (!int.TryParse(Console.ReadLine(), out int stake) || stake <= 0)
            {
                Console.WriteLine("Invalid stake.");
                return;
            }

            if (stake > player.GetGold())
            {
                Console.WriteLine("Not enough gold.");
                return;
            }

            Random rand = new Random();
            int playerTotal = rand.Next(2, 12) + rand.Next(2, 12);
            int dealerTotal = rand.Next(14, 22);

            Console.WriteLine($"You drew cards totaling: {playerTotal}");
            Console.WriteLine($"Dealer's total: {dealerTotal}");

            if (playerTotal > 21)
            {
                Console.WriteLine("Bust! You lost.");
                player.SubtractGold(stake);
            }
            else if (dealerTotal > 21 || playerTotal > dealerTotal)
            {
                Console.WriteLine("You win!");
                player.AddGold(stake);
            }
            else if (playerTotal == dealerTotal)
            {
                Console.WriteLine("Push. It's a draw.");
            }
            else
            {
                Console.WriteLine("Dealer wins. You lost.");
                player.SubtractGold(stake);
            }


        }

        static void DiceDuel(Player player)
        {
            Console.WriteLine("🎲 Welcome to Dice Duel!");
            Console.Write("Enter your stake: ");
            if (!int.TryParse(Console.ReadLine(), out int stake) || stake <= 0)
            {
                Console.WriteLine("Invalid stake.");
                return;
            }

            if (stake > player.GetGold())
            {
                Console.WriteLine("Not enough gold.");
                return;
            }

            Random rand = new Random();
            int dealerRoll = rand.Next(1, 7);
            int playerRoll = rand.Next(1, 7);

            Console.WriteLine($"Dealer rolled: {dealerRoll}");
            Console.WriteLine($"You rolled: {playerRoll}");

            if (playerRoll > dealerRoll)
            {
                Console.WriteLine("🎉 You win!");
                Console.WriteLine($"You won {stake} gold!");
                player.AddGold(stake);
            }
            else if (playerRoll == dealerRoll)
            {
                Console.WriteLine("It's a draw! Your gold is returned.");
            }
            else
            {
                Console.WriteLine("💸 You lost!");
                Console.WriteLine($"You lost {stake} gold!");
                player.SubtractGold(stake);
            }

        }

        static void Roulette(Player player)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            string[] betOptions =
            {
                "╔════════════════════════════════════╗",
                "║            🎡 ROULETTE 🎡          ║",
                "╠════════════════════════════════════╣",
                "║     Bet on Red                     ║",
                "║     Bet on Black                   ║",
                "║     Bet on a Number (0–36)         ║",
                "║     Exit                           ║",
                "╚════════════════════════════════════╝"
            };

            int selectedIndex = 0;
            int menuStartRow = 3;

            while (true)
            {
                Console.Clear();
                for (int i = 0; i < betOptions.Length; i++)
                {
                    string line = betOptions[i];

                    if (i >= menuStartRow && i < menuStartRow + 4)
                    {
                        int optionIndex = i - menuStartRow;
                        if (optionIndex == selectedIndex)
                            line = line.Substring(0, 3) + "▶" + line.Substring(4);
                        else
                            line = line.Substring(0, 3) + " " + line.Substring(4);
                    }

                    Console.WriteLine(line);
                }

                Console.WriteLine("Use ↑ ↓ to choose your bet. Press Enter to select.");
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 4) % 4;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 4;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        if (selectedIndex == 3)
                            return; // Exit

                        Console.Write("Enter your stake: ");
                        if (!int.TryParse(Console.ReadLine(), out int stake) || stake <= 0)
                        {
                            Console.WriteLine("Invalid stake. Press any key...");
                            Console.ReadKey(true);
                            continue;
                        }

                        if (stake > player.GetGold())
                        {
                            Console.WriteLine("Not enough gold. Press any key...");
                            Console.ReadKey(true);
                            continue;
                        }

                        string betType = selectedIndex == 0 ? "Red" : selectedIndex == 1 ? "Black" : "Number";
                        int betNumber = -1;

                        if (betType == "Number")
                        {
                            Console.Write("Enter a number to bet on (0-36): ");
                            if (!int.TryParse(Console.ReadLine(), out betNumber) || betNumber < 0 || betNumber > 36)
                            {
                                Console.WriteLine("Invalid number. Press any key...");
                                Console.ReadKey(true);
                                continue;
                            }
                        }

                        // Simulate spin
                        Console.WriteLine("Spinning...");
                        Thread.Sleep(1500);

                        Random rand = new Random();
                        int resultNumber = rand.Next(0, 37);
                        string resultColor = GetRouletteColor(resultNumber);
                        Console.WriteLine($"🎲 The ball lands on: {resultNumber} ({resultColor})");

                        bool win = false;
                        int winnings = 0;

                        if (betType == "Red" || betType == "Black")
                        {
                            if (resultColor == betType)
                            {
                                win = true;
                                winnings = stake;
                                player.AddGold(winnings);
                            }
                            else
                            {
                                player.SubtractGold(stake);
                            }
                        }
                        else if (betType == "Number")
                        {
                            if (resultNumber == betNumber)
                            {
                                win = true;
                                winnings = stake * 36;
                                player.AddGold(winnings);
                            }
                            else
                            {
                                player.SubtractGold(stake);
                            }
                        }

                        if (win)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"🎉 You WON {winnings} gold!");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("💸 You lost your stake.");
                        }

                        Console.ResetColor();
                        Console.WriteLine("\nPress any key to return to the roulette table...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        static string GetRouletteColor(int number)
        {
            if (number == 0)
                return "Green";

            int[] redNumbers = {
                1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36
            };

            return redNumbers.Contains(number) ? "Red" : "Black";
        }


        static void AcceptQuest(ref Quest quest)
        {
            if (!quest.IsAccepted)
            {
                quest.IsAccepted = true;
                Console.WriteLine($"✅ You accepted the quest: {quest.Name}");
            }
            else
            {
                Console.WriteLine($"You already accepted the quest: {quest.Name}");
            }
        }

        static void RewardQuest(ref Quest quest, Player Player)
        {
            if (quest.IsCompleted && !quest.IsRewarded)
            {
                quest.IsRewarded = true;
                Player.AddGold(100);
                Player.SetXP(Player.GETXP() + 50);
                Console.WriteLine($"🏆 You received 100 gold and 50 XP for completing '{quest.Name}'!");
            }
        }

        static void TalkToVillager(ref Quest killQuest, ref Quest chestQuest, Player Player)
        {
            Console.WriteLine("Villager: Hello! I have some tasks for you.");

            // Kill enemies quest
            if (!killQuest.IsAccepted)
            {
                Console.WriteLine("1) Accept 'Kill 5 enemies' quest");
            }
            else
            {
                killQuest.ShowStatus();
                if (killQuest.IsCompleted && !killQuest.IsRewarded)
                    Console.WriteLine("You can turn in this quest.");
            }

            // Open chests quest
            if (!chestQuest.IsAccepted)
            {
                Console.WriteLine("2) Accept 'Open 3 chests' quest");
            }
            else
            {
                chestQuest.ShowStatus();
                if (chestQuest.IsCompleted && !chestQuest.IsRewarded)
                    Console.WriteLine("You can turn in this quest.");
            }

            Console.WriteLine("Choose an option or press any other key to exit:");

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    AcceptQuest(ref killQuest);
                    break;
                case ConsoleKey.D2:
                    AcceptQuest(ref chestQuest);
                    break;
                case ConsoleKey.T: // Turn in quests (example key)
                    if (killQuest.IsCompleted && !killQuest.IsRewarded)
                        RewardQuest(ref killQuest, Player);
                    if (chestQuest.IsCompleted && !chestQuest.IsRewarded)
                        RewardQuest(ref chestQuest, Player);
                    break;
                default:
                    Console.WriteLine("Leaving villager.");
                    break;
            }
        }
        static void EnemyKilled(ref Quest killQuest)
        {
            killQuest.AddProgress();
        }

        static void ChestOpened(ref Quest chestQuest)
        {
            chestQuest.AddProgress();
        }

        static void CheckExplorer(char[,] map, Player player)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == ' ')
                    {
                        return;
                    }
                }
            }

        }

        static char[,] BossMap()
        {
            char[,] map =
            { {'#','#','#','#','#','#', },
              {'#','P',' ','b','X','#', },
              {'#','#','#','#','#','#', },
            };

            return map;
        }
    }
}