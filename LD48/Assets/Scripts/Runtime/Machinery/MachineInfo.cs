using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    [CreateAssetMenu(fileName = "Machine", menuName = "ScriptableObjects/Machine", order = 1)]
    public class MachineInfo : ScriptableObject
    {
        public string name;
        public List<PortDefinition> inputPorts;
        public List<PortDefinition> outputPorts;
        public List<Production> production;
        public StringIntDictionary inputCapacity;
        public StringIntDictionary outputCapacity;

        public MachineInfo()
        {
            inputPorts = new List<PortDefinition>();
            outputPorts = new List<PortDefinition>();
            production = new List<Production>();
            inputCapacity = new StringIntDictionary();
            outputCapacity = new StringIntDictionary();
        }

        public MachineInfo(string name) : this()
        {
            this.name = name;
        }

        public MachineInfo WithInputPort(PortDefinition port)
        {
            inputPorts.Add(port);
            return this;
        }

        public MachineInfo WithOutputPort(PortDefinition port)
        {
            outputPorts.Add(port);
            return this;
        }

        public MachineInfo WithInputCapacity(string material, int capacity)
        {
            inputCapacity.Add(material, capacity);
            return this;
        }

        public MachineInfo WithOutputCapacity(string material, int capacity)
        {
            outputCapacity.Add(material, capacity);
            return this;
        }

        public MachineInfo WithProduction(Production production)
        {
            this.production.Add(production);
            return this;
        }

        public override string ToString()
        {
            return $"Machine Info {name}";
        }
    }
}