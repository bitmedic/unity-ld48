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
        public MachineInfo machineInfo;

        private int rotation = 0;

        public void ResetRotate()
        {
            rotation = 0;
        }

        public void DoRotate()
        {
            rotation++;

            if (rotations == null || rotation > rotations.Count)
            {
                rotation = 0;
            }
        }

        public TileBase GetTileRotation()
        {
            if (rotation == 0)
            {
                return this.tile;
            }
            else
            {
                if (rotation - 1 <= this.rotations.Count)
                {
                    return this.rotations[rotation - 1];
                }
            }

            return this.tile;
        }
    }
}
