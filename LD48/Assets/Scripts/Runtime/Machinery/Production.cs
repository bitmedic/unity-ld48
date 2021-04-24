using System.Collections.Generic;

namespace LD48
{
    public class Production
    {
        public enum Strategy
        {
            Time = 0,
            Formula = 1
        }

        public string material;
        public int amount;
        public Strategy strategy = Strategy.Time;
        public int tickCost;

        private int tickCounter;

        public Production()
        {
        }

        public Production(string material, int amount, int tickCost) : this()
        {
            this.material = material;
            this.amount = amount;
            this.tickCost = tickCost;

            strategy = Strategy.Time;
        }

        public List<string> Produce()
        {
            tickCounter++;
            if (tickCounter < tickCost) return null;
            tickCounter = 0;

            List<string> result = new List<string>();
            for (int i = 0; i < amount; i++)
            {
                result.Add(material);
            }

            return result;
        }
    }
}