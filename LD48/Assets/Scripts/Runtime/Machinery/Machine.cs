using System.Collections.Generic;
using System.Linq;
using static LD48.Production;

namespace LD48
{
    public class Machine
    {
        public string name;
        public List<Port> inputPorts;
        public List<Port> outputPorts;
        public Dictionary<string, Storage> inputStorage;
        public Dictionary<string, Storage> tempStorage; // production during tick
        public Dictionary<string, Storage> outputStorage;
        public List<Production> production;

        private bool fetchingDone;
        private bool productionDone;

        public Machine()
        {
            inputPorts = new List<Port>();
            outputPorts = new List<Port>();
            inputStorage = new Dictionary<string, Storage>();
            tempStorage = new Dictionary<string, Storage>();
            outputStorage = new Dictionary<string, Storage>();
            production = new List<Production>();
        }

        public Machine(string name) : this()
        {
            this.name = name;
        }

        public Machine PrepareTick()
        {
            fetchingDone = false;
            productionDone = false;

            return this;
        }

        public bool TickDone()
        {
            return fetchingDone && productionDone;
        }

        public Machine EndTick()
        {
            // copy handled materials to output
            foreach (KeyValuePair<string, Storage> temp in tempStorage)
            {
                if (!outputStorage.ContainsKey(temp.Key)) outputStorage.Add(temp.Key, new Storage(temp.Key));
                outputStorage[temp.Key].amount += temp.Value.amount;
                temp.Value.amount = 0;
            }

            return this;
        }

        public void FullTick()
        {
            PrepareTick();
            Tick();
            EndTick();
        }

        public Machine Tick()
        {
            // Step 1: fetch inputs, fill input storage
            if (!fetchingDone)
            {
                foreach (Port port in inputPorts)
                {
                    if (port.connectedMachine.outputStorage.Count == 0) continue;
                    Storage input = port.connectedMachine.outputStorage.FirstOrDefault(s => s.Value.amount > 0).Value;
                    if (input == null) continue;

                    if (!inputStorage.ContainsKey(input.material)) inputStorage.Add(input.material, new Storage(input.material));
                    if (inputStorage[input.material].capacity == 0 || inputStorage[input.material].capacity > inputStorage[input.material].amount)
                    {
                        inputStorage[input.material].amount++;
                        input.amount--;

                        fetchingDone = true;
                    }
                }
            }

            // Step 2: produce, fill output storage
            if (!productionDone)
            {
                foreach (Production p in production)
                {
                    switch (p.strategy)
                    {
                        case Strategy.Forward:
                            // check if there is any input and output is empty
                            Storage input = inputStorage.FirstOrDefault(i => i.Value.amount > 0).Value;
                            if (input != null)
                            {
                                if (tempStorage.All(o => o.Value.amount == 0))
                                {
                                    EnsureTempStorage(input.material);
                                    input.amount--;
                                    tempStorage[input.material].amount++;
                                    productionDone = true;
                                }
                            }

                            break;

                        case Strategy.Time:
                        case Strategy.Formula:
                            p.Produce()?.ForEach(m =>
                            {
                                EnsureTempStorage(m);
                                if (outputStorage[m].capacity > 0 && outputStorage[m].amount >= outputStorage[m].capacity) return;

                                tempStorage[m].amount++;
                                productionDone = true;
                            });
                            break;
                    }
                }
            }

            return this;
        }

        private void EnsureTempStorage(string material)
        {
            if (!tempStorage.ContainsKey(material)) tempStorage.Add(material, new Storage(material));
            if (!outputStorage.ContainsKey(material)) outputStorage.Add(material, new Storage(material));
            tempStorage[material].capacity = outputStorage[material].capacity;
        }

        public Machine WithInputPort(Port port)
        {
            inputPorts.Add(port);
            port.connectedMachine.outputPorts.Add(new Port(this));
            return this;
        }

        public Machine WithOutputPort(Port port)
        {
            outputPorts.Add(port);
            port.connectedMachine.inputPorts.Add(new Port(this));
            return this;
        }

        public Machine WithInputStorage(Storage storage)
        {
            inputStorage.Add(storage.material, storage);
            return this;
        }

        public Machine WithOutputStorage(Storage storage)
        {
            outputStorage.Add(storage.material, storage);
            return this;
        }

        public Machine WithProduction(Production production)
        {
            this.production.Add(production);
            return this;
        }

        public override string ToString()
        {
            return $"Machine {name}";
        }
    }
}