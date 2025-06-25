using DoSomething;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{ 
    internal static class MapGenerator
    {

        public static char[,] GenerateMazeWithChestsAndEnemies(string mapType, int width = 20, int height = 20)
        {

            char[,] map;
            Random rand = new Random();

            // Enemies per map type
            Dictionary<string, char[]> enemyTypes = new()
        {
            { "forest", new[] { 'G', 'O', 'T', 'L' } }, // Goblin, Orc, Troll, Slime
            { "cave",   new[] { 'S', 'B', 'L' } },      // Skeleton, Bandit, Slime
            { "castle", new[] { 'D', 'V' } }           // Dragon, Vampire
        };

            char[] enemies = enemyTypes.ContainsKey(mapType.ToLower()) ? enemyTypes[mapType.ToLower()] : new[] { 'G' };

            for (int attempt = 0; attempt < 50; attempt++) // Try up to 50 times
            {
                map = new char[height, width];
                bool[,] visited = new bool[height, width];

                // Fill with walls
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        map[y, x] = '#';

                // Recursive maze generation
                void Carve(int x, int y)
                {
                    visited[y, x] = true;
                    map[y, x] = ' ';

                    var directions = new (int dx, int dy)[] { (0, -2), (0, 2), (-2, 0), (2, 0) }
                        .OrderBy(_ => rand.Next()).ToArray();

                    foreach (var (dx, dy) in directions)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && !visited[ny, nx])
                        {
                            map[y + dy / 2, x + dx / 2] = ' ';
                            Carve(nx, ny);
                        }
                    }
                }

                Carve(1, 1);
                map[1, 1] = 'P';
                map[height - 2, width - 2] = 'X';

                // Check path from P to X
                if (!PathExists(map, 1, 1, width - 2, height - 2))
                    continue;

                // Collect walkable tiles
                List<(int x, int y)> pathTiles = new();
                for (int y = 1; y < height - 1; y++)
                    for (int x = 1; x < width - 1; x++)
                        if (map[y, x] == ' ' && !(x == 1 && y == 1) && !(x == width - 2 && y == height - 2))
                            pathTiles.Add((x, y));

                // Place chest + enemy combos
                int chestPairs = rand.Next(3, 6);
                int placed = 0;

                while (placed < chestPairs && pathTiles.Count > 5)
                {
                    var (cx, cy) = pathTiles[rand.Next(pathTiles.Count)];
                    var directions = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) }
                        .OrderBy(_ => rand.Next()).ToArray();

                    foreach (var (dx, dy) in directions)
                    {
                        int ex = cx + dx, ey = cy + dy;
                        if (IsInBounds(ex, ey, width, height) &&
                            map[cy, cx] == ' ' && map[ey, ex] == ' ')
                        {
                            map[ey, ex] = enemies[rand.Next(enemies.Length)];
                            map[cy, cx] = 'C';

                            if (MustFightEnemyFirst(map, 1, 1, ex, ey, cx, cy))
                            {
                                pathTiles.RemoveAll(p => (p.x == cx && p.y == cy) || (p.x == ex && p.y == ey));
                                placed++;
                            }
                            else
                            {
                                map[cy, cx] = ' ';
                                map[ey, ex] = ' ';
                            }

                            break;
                        }
                    }

                    pathTiles.Remove((cx, cy));
                }

                // ✅ Add extra enemies (for difficulty)
                int extraEnemies = rand.Next(5, 10);
                while (extraEnemies > 0 && pathTiles.Count > 0)
                {
                    var (x, y) = pathTiles[rand.Next(pathTiles.Count)];
                    map[y, x] = enemies[rand.Next(enemies.Length)];
                    pathTiles.Remove((x, y));
                    extraEnemies--;
                }

                return map;
            }

            throw new Exception("Failed to generate a valid maze after many attempts.");
        }

        static bool PathExists(char[,] map, int startX, int startY, int goalX, int goalY)
        {
            int width = map.GetLength(1), height = map.GetLength(0);
            bool[,] seen = new bool[height, width];
            Queue<(int x, int y)> queue = new();
            queue.Enqueue((startX, startY));
            seen[startY, startX] = true;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                if (x == goalX && y == goalY) return true;

                foreach (var (dx, dy) in new[] { (0, 1), (1, 0), (0, -1), (-1, 0) })
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height &&
                        !seen[ny, nx] && map[ny, nx] != '#')
                    {
                        seen[ny, nx] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }

            return false;
        }

        static bool MustFightEnemyFirst(char[,] map, int px, int py, int ex, int ey, int cx, int cy)
        {
            int w = map.GetLength(1), h = map.GetLength(0);

            // BFS to enemy
            bool[,] reachEnemy = new bool[h, w];
            Queue<(int x, int y)> q = new();
            q.Enqueue((px, py));
            reachEnemy[py, px] = true;

            bool canReachEnemy = false;
            while (q.Count > 0)
            {
                var (x, y) = q.Dequeue();
                if (x == ex && y == ey) { canReachEnemy = true; break; }

                foreach (var (dx, dy) in new[] { (0, 1), (1, 0), (0, -1), (-1, 0) })
                {
                    int nx = x + dx, ny = y + dy;
                    if (IsInBounds(nx, ny, w, h) && !reachEnemy[ny, nx] && map[ny, nx] != '#')
                    {
                        reachEnemy[ny, nx] = true;
                        q.Enqueue((nx, ny));
                    }
                }
            }

            // BFS from player to chest, but treat enemy as wall
            bool[,] blocked = new bool[h, w];
            Queue<(int x, int y)> qc = new();
            qc.Enqueue((px, py));
            blocked[py, px] = true;

            bool canSkipEnemy = false;
            while (qc.Count > 0)
            {
                var (x, y) = qc.Dequeue();
                if (x == cx && y == cy) { canSkipEnemy = true; break; }

                foreach (var (dx, dy) in new[] { (0, 1), (1, 0), (0, -1), (-1, 0) })
                {
                    int nx = x + dx, ny = y + dy;
                    if (IsInBounds(nx, ny, w, h) && !blocked[ny, nx] &&
                        map[ny, nx] != '#' && !(nx == ex && ny == ey))
                    {
                        blocked[ny, nx] = true;
                        qc.Enqueue((nx, ny));
                    }
                }
            }

            return canReachEnemy && !canSkipEnemy;
        }

        static bool IsInBounds(int x, int y, int width, int height) =>
            x >= 0 && x < width && y >= 0 && y < height;
    }
}