using System.Collections.Generic;

namespace LD48
{
    public class Machine
    {
        public string name;
        public List<Port> inputPorts;
        public List<Port> outputPorts;
        public Dictionary<string, Storage> inputStorage;
        public Dictionary<string, Storage> outputStorage;
        public List<Production> production;

        public Machine()
        {
            inputPorts = new List<Port>();
            outputPorts = new List<Port>();
            inputStorage = new Dictionary<string, Storage>();
            outputStorage = new Dictionary<string, Storage>();
            production = new List<Production>();
        }

        public Machine(string name) : this()
        {
            this.name = name;
        }

        public void Tick()
        {
            foreach (Production p in production)
            {
                List<string> materials = p.Produce();

                materials?.ForEach(m =>
                {
                    if (!outputStorage.ContainsKey(m)) outputStorage.Add(m, new Storage(m));
                    if (outputStorage[m].capacity > 0 && outputStorage[m].amount >= outputStorage[m].capacity) return;

                    outputStorage[m].amount++;
                });
            }
        }

        public Machine WithInputPort(Port port)
        {
            (inputPorts ??= new List<Port>()).Add(port);
            return this;
        }

        public Machine WithOutputPort(Port port)
        {
            (outputPorts ??= new List<Port>()).Add(port);
            return this;
        }

        public Machine WithInputStorage(Storage storage)
        {
            (inputStorage ??= new Dictionary<string, Storage>()).Add(storage.material, storage);
            return this;
        }

        public Machine WithOutputStorage(Storage storage)
        {
            (outputStorage ??= new Dictionary<string, Storage>()).Add(storage.material, storage);
            return this;
        }

        public Machine WithProduction(Production production)
        {
            (this.production ??= new List<Production>()).Add(production);
            return this;
        }
    }
}