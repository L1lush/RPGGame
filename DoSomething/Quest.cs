using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    struct Quest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RequiredAmount { get; set; }
        public int Progress { get; set; }
        public int Reward { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsRewarded { get; set; }

        public Quest(string name, string description, int requiredAmount, int reward)
        {
            Name = name;
            Description = description;
            RequiredAmount = requiredAmount;
            Reward = reward;
            Progress = 0;
            IsAccepted = false;
            IsCompleted = false;
            IsRewarded = false;
        }

        public void AddProgress()
        {
            int amount = 1;


            if (!IsAccepted || IsCompleted) return;

            Progress += amount;
            if (Progress >= RequiredAmount)
            {
                Progress = RequiredAmount;
                IsCompleted = true;
                Console.WriteLine($"🎉 Quest Completed: {Name}!");
            }
        }

        public void ResetQuest()
        {
            Progress = 0;
            IsAccepted = false;
            IsCompleted = false;
            IsRewarded = false;
        }
    }

}
