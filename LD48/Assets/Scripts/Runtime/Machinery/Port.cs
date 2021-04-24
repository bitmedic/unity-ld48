using System;

namespace LD48
{
    [Serializable]
    public class Port
    {
        public PortDefinition definition;
        public Machine connectedMachine;

        public Port()
        {
        }

        public Port(PortDefinition definition) : this()
        {
            this.definition = definition;
        }

        public Port(Machine connectedMachine) : this()
        {
            this.connectedMachine = connectedMachine;
        }

        public override string ToString()
        {
            return $"Port ({connectedMachine})";
        }
    }
}