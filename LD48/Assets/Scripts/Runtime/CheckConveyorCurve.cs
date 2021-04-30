using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LD48
{
    public class CheckConveyorCurve : MonoBehaviour
    {
        public Tilemap tilemap;

        [Header("Cobnveyor Tiles Straight")]
        public TileBase conveyor_straight_nwse;
        public TileBase conveyor_straight_nesw;
        public TileBase conveyor_straight_senw;
        public TileBase conveyor_straight_swne;

        [Header("Conveyor Tiles Curve Left")]
        public TileBase conveyor_curveleft_nwne;
        public TileBase conveyor_curveleft_nese;
        public TileBase conveyor_curveleft_swnw;
        public TileBase conveyor_curveleft_sesw;

        [Header("Conveyor Tiles Curve Right")]
        public TileBase conveyor_curveright_nwsw;
        public TileBase conveyor_curveright_swse;
        public TileBase conveyor_curveright_sene;
        public TileBase conveyor_curveright_nenw;

        public void RealignConveyorsCurve()
        {
            HashSet<string> machineTypes = new HashSet<string>();
            HashSet<string> unmappedMachineTypes = new HashSet<string>();

            for (int x = -50; x < 50; x++)
            {
                for (int y = -50; y < 50; y++)
                {
                    if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        string key = tile.name.ToLowerInvariant();

                        // check all next neighbours straight
                        if (key.Equals(conveyor_straight_nwse.name.ToLowerInvariant()))
                        {
                            TileBase nextNeighbourStraight = GetNeighbour(x, y - 1);

                            if (nextNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase nextNeighbourRight = GetNeighbour(x - 1, y);
                                TileBase nextNeighbourLeft = GetNeighbour(x + 1, y);

                                if (nextNeighbourRight != null)
                                {
                                    if ((nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_nesw.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_nenw.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_nese.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_nwsw);
                                    }
                                }

                                if (nextNeighbourLeft)
                                {
                                    if ((nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_swne.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_swse.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_swnw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_nwne);
                                    }
                                }
                            }
                        }
                        else if (key.Equals(conveyor_straight_nesw.name.ToLowerInvariant()))
                        {
                            TileBase nextNeighbourStraight = GetNeighbour(x - 1, y);

                            if (nextNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase nextNeighbourRight = GetNeighbour(x, y + 1);
                                TileBase nextNeighbourLeft = GetNeighbour(x, y - 1);

                                if (nextNeighbourRight != null)
                                {
                                    if ((nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_senw.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_sene.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_sesw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_nenw);
                                    }
                                }
                                
                                if (nextNeighbourLeft)
                                {
                                    if ((nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_nwse.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_nwsw.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_nwne.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_nese);
                                    }
                                }
                            }
                        }
                        else if (key.Equals(conveyor_straight_senw.name.ToLowerInvariant()))
                        {
                            TileBase nextNeighbourStraight = GetNeighbour(x, y + 1);

                            if (nextNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase nextNeighbourRight = GetNeighbour(x + 1, y);
                                TileBase nextNeighbourLeft = GetNeighbour(x - 1, y);

                                if (nextNeighbourRight != null)
                                {
                                    if ((nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_swne.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_swse.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_swnw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_sene);
                                    }
                                }
                                                                
                                if (nextNeighbourLeft)
                                {
                                    if ((nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_nesw.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_nenw.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_nese.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_sesw);
                                    }
                                }
                            }
                        }
                        else if (key.Equals(conveyor_straight_swne.name.ToLowerInvariant()))
                        {
                            TileBase nextNeighbourStraight = GetNeighbour(x + 1, y);

                            if (nextNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase nextNeighbourRight = GetNeighbour(x, y - 1);
                                TileBase nextNeighbourLeft = GetNeighbour(x, y + 1);

                                if (nextNeighbourRight != null)
                                {
                                    if ((nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_nwse.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_nwsw.name.ToLowerInvariant()) ||
                                        nextNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_nwne.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_swse);
                                    }
                                }
                                
                                if (nextNeighbourLeft)
                                {
                                    if ((nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_senw.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_sene.name.ToLowerInvariant()) ||
                                        nextNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_sesw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_swnw);
                                    }
                                }
                            }
                        }


                        // check all previous neighbours straight
                        if (key.Equals(conveyor_straight_nwse.name.ToLowerInvariant()))
                        {
                            TileBase prevNeighbourStraight = GetNeighbour(x, y + 1);

                            if (prevNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase prevNeighbourRight = GetNeighbour(x - 1, y);
                                TileBase prevNeighbourLeft = GetNeighbour(x + 1, y);

                                if (prevNeighbourRight != null)
                                {
                                    if ((prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_swne.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_sene.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_nwne.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_swse);
                                    }
                                }

                                if (prevNeighbourLeft)
                                {
                                    if ((prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_nesw.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_nwsw.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_sesw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_nese);
                                    }
                                }
                            }
                        }
                        else if (key.Equals(conveyor_straight_nesw.name.ToLowerInvariant()))
                        {
                            TileBase prevNeighbourStraight = GetNeighbour(x + 1, y);

                            if (prevNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase prevNeighbourRight = GetNeighbour(x, y + 1);
                                TileBase prevNeighbourLeft = GetNeighbour(x, y - 1);

                                if (prevNeighbourRight != null)
                                {
                                    if ((prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_nwse.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_swse.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_nese.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_nwsw);
                                    }
                                }

                                if (prevNeighbourLeft)
                                {
                                    if ((prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_senw.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_nenw.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_swnw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_sesw);
                                    }
                                }
                            }
                        }
                        else if (key.Equals(conveyor_straight_senw.name.ToLowerInvariant()))
                        {
                            TileBase prevNeighbourStraight = GetNeighbour(x, y + 1);

                            if (prevNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase prevNeighbourRight = GetNeighbour(x + 1, y);
                                TileBase prevNeighbourLeft = GetNeighbour(x - 1, y);

                                if (prevNeighbourRight != null)
                                {
                                    if ((prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_nesw.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_nwsw.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_sesw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_nenw);
                                    }
                                }

                                if (prevNeighbourLeft)
                                {
                                    if ((prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_swne.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_sene.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_nwne.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_swnw);
                                    }
                                }
                            }
                        }
                        else if (key.Equals(conveyor_straight_swne.name.ToLowerInvariant()))
                        {
                            TileBase prevNeighbourStraight = GetNeighbour(x - 1, y);

                            if (prevNeighbourStraight == null)
                            {
                                // check curve left and right
                                TileBase prevNeighbourRight = GetNeighbour(x, y - 1);
                                TileBase prevNeighbourLeft = GetNeighbour(x, y + 1);

                                if (prevNeighbourRight != null)
                                {
                                    if ((prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_straight_senw.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveright_nenw.name.ToLowerInvariant()) ||
                                        prevNeighbourRight.name.ToLowerInvariant().Equals(conveyor_curveleft_swnw.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveright_sene);
                                    }
                                }

                                if (prevNeighbourLeft)
                                {
                                    if ((prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_straight_nwse.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveright_swse.name.ToLowerInvariant()) ||
                                        prevNeighbourLeft.name.ToLowerInvariant().Equals(conveyor_curveleft_nese.name.ToLowerInvariant())))
                                    {
                                        tilemap.SetTile(new Vector3Int(x, y, 0), conveyor_curveleft_nwne);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private TileBase GetNeighbour(int x, int y)
        {
            if (tilemap.HasTile(new Vector3Int(x, y, 0)))
            {
                return tilemap.GetTile(new Vector3Int(x, y, 0));
            }
            else
            {
                return null;
            }
        }
    }
}