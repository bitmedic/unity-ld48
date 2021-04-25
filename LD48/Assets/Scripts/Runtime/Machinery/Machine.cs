using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LD48.Production;

namespace LD48
{
    public delegate void OutputProduced(Machine machine, List<Package> packages);

    [Serializable]
    public class Machine
    {
        public MachineInfo info;
        public Vector3Int position;

        public List<Port> inputPorts;
        public List<Port> outputPorts;
        public List<Package> inputStorage;
        public List<Package> tempStorage; // production during tick
        public List<Package> outputStorage;

        public event OutputProduced OnOutputProduced;

        private StringIntDictionary productionTicks;
        private bool fetchingDone;
        private bool productionDone;

        public Machine()
        {
            inputPorts = new List<Port>();
            outputPorts = new List<Port>();
            inputStorage = new List<Package>();
            tempStorage = new List<Package>();
            outputStorage = new List<Package>();
            productionTicks = new StringIntDictionary();
        }

        public Machine(MachineInfo info) : this()
        {
            this.info = info;

            inputPorts.Clear();
            outputPorts.Clear();
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
            outputStorage.AddRange(tempStorage);

            if (tempStorage.Count > 0) OnOutputProduced?.Invoke(this, new List<Package>(tempStorage));
            tempStorage.Clear();

            return this;
        }

        public void FullTick()
        {
            PrepareTick();
            Tick();
            EndTick();
        }

        public int GetMaterialQuantity(string key)
        {
            return outputStorage.Count(s => s.material == key);
        }

        public Machine Tick()
        {
            // Step 1: fetch inputs, fill input storage
            if (!fetchingDone)
            {
                foreach (Port port in inputPorts)
                {
                    if (port.connectedMachine.outputStorage.Count == 0) continue;
                    Package input = port.connectedMachine.outputStorage[0];

                    int inputCapacity = Mathf.Max(info.inputCapacity.ContainsKey(input.material) ? info.inputCapacity[input.material] : 0, info.totalInputCapacity);
                    int outputCapacity = Mathf.Max(info.outputCapacity.ContainsKey(input.material) ? info.outputCapacity[input.material] : 0, info.totalOutputCapacity);
                    if ((inputCapacity == 0 || inputCapacity > inputStorage.Count(s => s.material == input.material)) && (outputCapacity == 0 || outputCapacity > outputStorage.Count(s => s.material == input.material)))
                    {
                        inputStorage.Add(input);
                        port.connectedMachine.outputStorage.RemoveAt(0);

                        fetchingDone = true;
                    }
                }
            }

            // Step 2: produce, fill output storage
            if (!productionDone)
            {
                foreach (Production p in info.production)
                {
                    switch (p.strategy)
                    {
                        case Strategy.Forward:
                            // check if there is any input and output is empty
                            if (inputStorage.Count > 0 && outputStorage.Count == 0)
                            {
                                tempStorage.Add(inputStorage[0]);
                                inputStorage.RemoveAt(0);
                                productionDone = true;
                            }
                            break;

                        case Strategy.Time:
                        case Strategy.Formula:
                            if (!productionTicks.ContainsKey(p.GetKey())) productionTicks.Add(p.GetKey(), 0);
                            productionTicks[p.GetKey()]++;
                            if (productionTicks[p.GetKey()] < p.tickCost)
                            {
                                productionDone = true;
                                continue;
                            }
                            productionTicks[p.GetKey()] = 0;

                            p.Produce(inputStorage).ForEach(m =>
                            {
                                int capacity = Mathf.Max(info.outputCapacity.ContainsKey(m) ? info.outputCapacity[m] : 0, info.totalOutputCapacity);
                                if (capacity > 0 && outputStorage.Count(o => o.material == m) >= capacity) return;

                                tempStorage.Add(new Package(m));
                                productionDone = true;
                            });
                            break;
                    }
                }
            }

            return this;
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

        public override string ToString()
        {
            return $"Machine {info.name}";
        }
    }
}