using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class Achievement
    {
        public string Title { get; }
        public string Description { get; }
        public bool Unlocked { get; private set; }

        public Achievement(string title, string description)
        {
            Title = title;
            Description = description;
            Unlocked = false;
        }

        public void Unlock()
        {
            if (!Unlocked)
            {
                Unlocked = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n🏆 Achievement Unlocked: {Title} - {Description}");
                Console.ResetColor();
            }
        }
    }
}
