using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LD48
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildingSizeSO", order = 1)]
    public class BuildingSizeSO : ScriptableObject
    {
        public TileBase tile;
        public int width;
        public int height;
        public List<TileBase> rotations;
        public List<MachineInfo> machineInfo;

        private int rotation = 0;

        public void ResetRotate()
        {
            rotation = 0;
        }

        public void DoRotate(int order = 1)
        {
            rotation = rotation + order;
            if (rotation < 0)
            {
                rotation += rotations.Count;
            }
            else if (rotation >= rotations.Count)
            {
                rotation -= rotations.Count;
            }

        }

        public TileBase GetRotatedTile()
        {
            if (rotations.Count == 0)
            {
                return this.tile;
            }
            else
            {
                return rotations[rotation];
            }
        }
    }
}
