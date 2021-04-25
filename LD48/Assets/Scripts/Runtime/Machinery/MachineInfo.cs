using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    [CreateAssetMenu(fileName = "Machine", menuName = "ScriptableObjects/Machine", order = 1)]
    public class MachineInfo : ScriptableObject
    {
        public string name;
        public string key;
        public int maxInputPorts; // unused right now
        public int maxOutputPorts; // unused right now
        public List<Production> production;
        public StringIntDictionary inputCapacity;
        public StringIntDictionary outputCapacity;

        public MachineInfo()
        {
            production = new List<Production>();
            inputCapacity = new StringIntDictionary();
            outputCapacity = new StringIntDictionary();
        }

        public MachineInfo(string name) : this()
        {
            this.name = name;
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