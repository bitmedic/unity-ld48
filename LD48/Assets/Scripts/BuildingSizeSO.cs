using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LD48
{
    [CreateAssetMenu(fileName ="Data", menuName ="ScriptableObjects/BuildingSizeSO", order =1)]
    public class BuildingSizeSO : ScriptableObject
    {
        public TileBase tile;
        public int width;
        public int height;
    }
}
