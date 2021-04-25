using System;
using System.Collections.Generic;
using System.Linq;

namespace LD48
{
    [Serializable]
    public class AssemblyLine
    {
        public const bool DEBUG_NO_SHUFFLE = true;

        public List<Machine> machines;

        public AssemblyLine()
        {
            machines = new List<Machine>();
        }

        public void Tick()
        {
            if (!DEBUG_NO_SHUFFLE) machines.Shuffle();
            machines.ForEach(m => m.PrepareTick());

            // brute force over all machines as there is not necessarily a directed graph
            int triesLeft = machines.Count;
            while (machines.Any(m => !m.TickDone()) && triesLeft > 0)
            {
                triesLeft--;
                machines.ForEach(m => m.Tick());
            }

            machines.ForEach(m => m.EndTick());
        }

        public AssemblyLine WithMachine(Machine machine)
        {
            machines.Add(machine);
            return this;
        }

        public AssemblyLine WithMachines(params Machine[] machines)
        {
            this.machines.AddRange(machines);
            return this;
        }

        public override string ToString()
        {
            return $"Assembly Line ({machines.Count})";
        }
    }
}