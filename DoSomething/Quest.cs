using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    struct Quest
    {
        public string Name;
        public string Description;
        public int RequiredAmount;
        public int Progress;
        public bool IsAccepted;
        public bool IsCompleted;
        public bool IsRewarded;

        public Quest(string name, string description, int requiredAmount)
        {
            Name = name;
            Description = description;
            RequiredAmount = requiredAmount;
            Progress = 0;
            IsAccepted = false;
            IsCompleted = false;
            IsRewarded = false;
        }

        public void AddProgress(int amount = 1)
        {
            if (!IsAccepted || IsCompleted) return;

            Progress += amount;
            if (Progress >= RequiredAmount)
            {
                Progress = RequiredAmount;
                IsCompleted = true;
                Console.WriteLine($"🎉 Quest Completed: {Name}!");
            }
        }

        public void ShowStatus()
        {
            if (!IsAccepted)
            {
                Console.WriteLine($"Quest available: {Name}");
                Console.WriteLine(Description);
            }
            else if (!IsCompleted)
            {
                Console.WriteLine($"Quest in progress: {Name} ({Progress}/{RequiredAmount})");
            }
            else if (IsCompleted && !IsRewarded)
            {
                Console.WriteLine($"Quest ready to turn in: {Name}");
            }
            else
            {
                Console.WriteLine($"Quest completed: {Name}");
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
