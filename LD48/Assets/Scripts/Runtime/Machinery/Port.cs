using System;
using UnityEngine;

namespace LD48
{
    [Serializable]
    public class Port
    {
        public Vector2Int position = Vector2Int.zero; // zero = no position restriction
        public Machine connectedMachine;

        public Port()
        {
        }

        public Port(Machine connectedMachine)
        {
            this.connectedMachine = connectedMachine;
        }

        public override string ToString()
        {
            return $"Port ({connectedMachine})";
        }
    }
}