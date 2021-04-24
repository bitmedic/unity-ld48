using System;
using UnityEngine;

namespace LD48
{
    [Serializable]
    public class Package
    {
        public string material;
        public string guid;
        public GameObject gameObject;

        public Package()
        {
            guid = new Guid().ToString();
        }

        public Package(string material) : this()
        {
            this.material = material;
        }

        public override string ToString()
        {
            return $"Package of {material}";
        }
    }
}