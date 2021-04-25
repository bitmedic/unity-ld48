using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LD48
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourceNodeSO", order = 1)]
    public class ResourceNodeSO : ScriptableObject
    {
        public string resourceName;
        public List<TileBase> resourceNodeTiles;
    }
}
