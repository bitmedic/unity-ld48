using System;
using UnityEngine;

namespace LD48
{
    [Serializable]
    public class PortDefinition
    {
        public Vector2Int position = Vector2Int.zero; // zero = no position restriction

        public PortDefinition()
        {
        }

        public override string ToString()
        {
            return $"Port Definition ({position})";
        }
    }
}