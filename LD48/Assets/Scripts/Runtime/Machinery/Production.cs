using System;
using System.Collections.Generic;
using System.Linq;

namespace LD48
{
    [Serializable]
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
        public StringIntDictionary formula;

        private string key;

        public Production()
        {
            formula = new StringIntDictionary();
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

        public string GetKey()
        {
            if (string.IsNullOrEmpty(key)) key = Guid.NewGuid().ToString();
            return key;
        }

        public List<string> Produce(List<Package> inputs)
        {
            List<string> result = new List<string>();

            if (strategy == Strategy.Formula)
            {
                // iteration 1: check formula constraints
                foreach (KeyValuePair<string, int> req in formula)
                {
                    if (inputs.Count(i => i.material == req.Key) < req.Value) return result;
                }

                // iteration 2: remove inputs
                foreach (KeyValuePair<string, int> req in formula)
                {
                    for (int i = 0; i < req.Value; i++)
                    {
                        inputs.RemoveAt(inputs.FindIndex(input => input.material == req.Key));
                    }
                }
            }
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