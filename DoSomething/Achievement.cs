using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class achievement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Unlocked { get; set; }

        public achievement()
        {

        }
        public achievement(string title, string description)
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
                Console.WriteLine($"🏆 Achievement Unlocked: {Title} - {Description}");
                Thread.Sleep(2000);
                Console.ResetColor();
            }
        }
    }
}
