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

        private enum Direction { NW, NE, SE, SW, NONE };

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

                        // check all previous neighbours straight
                        if (key.Equals(conveyor_straight_nwse.name.ToLowerInvariant()))
                        {
                            Vector2Int prevNeighbourCell = new Vector2Int(x, y + 1);

                            if (!TryToConnectToPreviousConveyer(prevNeighbourCell, Direction.SE))
                            {
                                // previous straight conveyer was not able to connect, so check the neighbours right and left
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
                            Vector2Int prevNeighbourCell = new Vector2Int(x + 1, y);

                            if (!TryToConnectToPreviousConveyer(prevNeighbourCell, Direction.SW))
                            {
                                // previous straight conveyer was not able to connect, so check the neighbours right and left
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
                            Vector2Int prevNeighbourCell = new Vector2Int(x, y - 1);

                            if (!TryToConnectToPreviousConveyer(prevNeighbourCell, Direction.NW))
                            {
                                // previous straight conveyer was not able to connect, so check the neighbours right and left
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
                            Vector2Int prevNeighbourCell = new Vector2Int(x - 1, y);

                            if (!TryToConnectToPreviousConveyer(prevNeighbourCell, Direction.NE))
                            {
                                // previous straight conveyer was not able to connect, so check the neighbours right and left
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

        private bool TryToConnectToPreviousConveyer(Vector2Int prevNeighbourCell, Direction goingTo)
        {
            TileBase prevNeighbourStraight = GetNeighbour(prevNeighbourCell.x, prevNeighbourCell.y);

            if (prevNeighbourStraight == null)
            {
                return false; // if there is no previous tile previous conveyer to connect
            }

            if (IsTileConveyor(prevNeighbourStraight))
            {
                Direction comingFrom = Direction.NONE; 

                if (!IsConveyerOutputAlreadyConnected(prevNeighbourStraight, prevNeighbourCell.x, prevNeighbourCell.y, ref comingFrom))
                {
                    // change previousneighbour to match self
                    TileBase targetTile = this.GetConveyerDirection(comingFrom, goingTo);

                    if (targetTile != null)
                    {
                        tilemap.SetTile(new Vector3Int(prevNeighbourCell.x, prevNeighbourCell.y, 0), targetTile);
                    }

                    return true; // if previous conveyer was connected or did not exist
                }
                else
                {
                    return false; // previous conveyer was already connected
                }    
            }

            //else
            return true; // if it was something else but a conveyer (building), then still keep the connection
        }
        

        private TileBase GetConveyerDirection(Direction comingFrom, Direction goingTo)
        {
            if (comingFrom.Equals(Direction.NE))
            {
                if (goingTo.Equals(Direction.NW))
                {
                    return this.conveyor_curveright_nenw;
                }
                else if (goingTo.Equals(Direction.SE))
                {
                    return this.conveyor_curveleft_nese;
                }
                else if (goingTo.Equals(Direction.SW))
                {
                    return this.conveyor_straight_nesw;
                }
            }
            else if (comingFrom.Equals(Direction.NW))
            {
                if (goingTo.Equals(Direction.NE))
                {
                    return this.conveyor_curveleft_nwne;
                }
                else if (goingTo.Equals(Direction.SE))
                {
                    return this.conveyor_straight_nwse;
                }
                else if (goingTo.Equals(Direction.SW))
                {
                    return this.conveyor_curveright_nwsw;
                }
            }
            else if (comingFrom.Equals(Direction.SE))
            {
                if (goingTo.Equals(Direction.NE))
                {
                    return this.conveyor_curveright_sene;
                }
                else if (goingTo.Equals(Direction.NW))
                {
                    return this.conveyor_straight_senw;
                }
                else if (goingTo.Equals(Direction.SW))
                {
                    return this.conveyor_curveleft_sesw;
                }
            }
            else if (comingFrom.Equals(Direction.SW))
            {
                if (goingTo.Equals(Direction.NE))
                {
                    return this.conveyor_straight_swne;
                }
                else if (goingTo.Equals(Direction.NW))
                {
                    return this.conveyor_curveleft_swnw;
                }
                else if (goingTo.Equals(Direction.SE))
                {
                    return this.conveyor_curveright_swse;
                }
            }

            return null;
        }

        public bool IsTileConveyor(TileBase conveyor)
        {
            //if (conveyor == null)
            //{
            //    return true; // 
            //}

            string key = conveyor.name.ToLowerInvariant();
            // check all next neighbours straight
            if (key.Equals(conveyor_straight_nwse.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_straight_nesw.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_straight_senw.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_straight_swne.name.ToLowerInvariant()))
            {
                return true;
            }

            // check left curves
            else if (key.Equals(conveyor_curveleft_nwne.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_curveleft_nese.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_curveleft_sesw.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_curveleft_swnw.name.ToLowerInvariant()))
            {
                return true;
            }

            else if (key.Equals(conveyor_curveright_nwsw.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_curveright_nenw.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_curveright_sene.name.ToLowerInvariant()))
            {
                return true;
            }
            else if (key.Equals(conveyor_curveright_swse.name.ToLowerInvariant()))
            {
                return true;
            }

            // it's not a conveyor
            return false;
        }

        /// <summary>
        /// Checks if the Conveyer at the Tile is connected to another connect at his own output
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="comingFrom">the direction the conveyer is coming from</param>
        /// <returns>true if connected, false if not. </returns>
        private bool IsConveyerOutputAlreadyConnected(TileBase self, int x, int y, ref Direction comingFrom)
        {
            string key = self.name.ToLowerInvariant();

            // check all next neighbours straight
            if (key.Equals(conveyor_straight_nwse.name.ToLowerInvariant()))
            {
                comingFrom = Direction.NW;
                TileBase neighbour = GetNeighbour(x, y- 1);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("nw"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_straight_nesw.name.ToLowerInvariant()))
            {
                comingFrom = Direction.NE;
                TileBase neighbour = GetNeighbour(x - 1, y);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("ne"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_straight_senw.name.ToLowerInvariant()))
            {
                comingFrom = Direction.SE;
                TileBase neighbour = GetNeighbour(x, y + 1);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("se"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_straight_swne.name.ToLowerInvariant()))
            {
                comingFrom = Direction.SW;
                TileBase neighbour = GetNeighbour(x + 1, y);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("sw"))
                {
                    return true;
                }
            }

            // check left curves
            else if (key.Equals(conveyor_curveleft_nwne.name.ToLowerInvariant()))
            {
                comingFrom = Direction.NW;
                TileBase neighbour = GetNeighbour(x + 1, y);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("sw"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_curveleft_nese.name.ToLowerInvariant()))
            {
                comingFrom = Direction.NE;
                TileBase neighbour = GetNeighbour(x, y - 1);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("nw"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_curveleft_sesw.name.ToLowerInvariant()))
            {
                comingFrom = Direction.SE;
                TileBase neighbour = GetNeighbour(x - 1, y);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("ne"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_curveleft_swnw.name.ToLowerInvariant()))
            {
                comingFrom = Direction.SW;
                TileBase neighbour = GetNeighbour(x, y + 1);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("se"))
                {
                    return true;
                }
            }

            else if (key.Equals(conveyor_curveright_nwsw.name.ToLowerInvariant()))
            {
                comingFrom = Direction.NW;
                TileBase neighbour = GetNeighbour(x - 1, y);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("ne"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_curveright_nenw.name.ToLowerInvariant()))
            {
                comingFrom = Direction.NE;
                TileBase neighbour = GetNeighbour(x, y + 1);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("se"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_curveright_sene.name.ToLowerInvariant()))
            {
                comingFrom = Direction.SE;
                TileBase neighbour = GetNeighbour(x + 1, y);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("sw"))
                {
                    return true;
                }
            }
            else if (key.Equals(conveyor_curveright_swse.name.ToLowerInvariant()))
            {
                comingFrom = Direction.SW;
                TileBase neighbour = GetNeighbour(x, y - 1);
                if (neighbour != null && neighbour.name.Substring(neighbour.name.Length - 4, 2).ToLowerInvariant().Equals("nw"))
                {
                    return true;
                }
            }

            return false;
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