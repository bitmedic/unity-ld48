using System.Collections.Generic;

namespace LD48
{
    public class Production
    {
        public enum Strategy
        {
            Time = 0,
            Formula = 1,
            Forward = 2
        }

        public string material;
        public int amount;
        public Strategy strategy = Strategy.Time;
        public int tickCost;

        private int tickCounter;

        public Production()
        {
        }

        public Production(Strategy strategy) : this()
        {
            this.strategy = strategy;
        }

        public Production(string material, int amount = 1, int tickCost = 1) : this()
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

        public override string ToString()
        {
            return $"Production of {material} ({amount}, {strategy})";
        }
    }
}