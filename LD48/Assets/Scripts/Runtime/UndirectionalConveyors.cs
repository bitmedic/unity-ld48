using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UndirectionalConveyors : MonoBehaviour
{
    [Header("Factory Tilemap")]
    public Tilemap tilemap;

    [Header("Conveyor Dot")]
    public TileBase conveyor_dot;

    [Header("Conveyor Tiles Straight")]
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

    [Header("Building Tiles")]
    public TileBase rocket;
    public TileBase drill;
    public TileBase refinery;
    public TileBase smelter;
    public TileBase armory;
    public TileBase empty_building_background;


    private enum Direction { NW, NE, SE, SW, None };
    private enum ConnectionType { Input, Output, None };

    public void BuildNewConveyor(Vector3Int position)
    {
        // get best matching neighbour
        Vector3Int neighbourPreviousPosition = new Vector3Int(0, 0, 0);
        Direction neighbourPreviousDirection = Direction.None;

        TileBase neighbourPreviousTile = GetBestMatchingOpenNeighbour(position, ref neighbourPreviousPosition, ref neighbourPreviousDirection, ConnectionType.Input, null);

        if (neighbourPreviousTile != null)
        {
            // change previous neighbour Tile to match new tile 
            TileBase changedTile = ChangePreviousTileToMatchNewTile(neighbourPreviousPosition, neighbourPreviousTile, DirectionGetOpposite(neighbourPreviousDirection));

            // check if Open Input Neighbour exists
            Vector3Int neighbourNextPosition = new Vector3Int(0, 0, 0);
            Direction neighbourNextDirection = Direction.None;
            TileBase neighbourNextTile = GetBestMatchingOpenNeighbour(position, ref neighbourNextPosition, ref neighbourNextDirection, ConnectionType.Output, changedTile);

            if (neighbourNextTile != null)
            {
                // previous and next tiles are given - Get Opposided as the directions are from the outside intio the tile and we need the direction inside the tile
                PlaceConveyor(position, neighbourPreviousDirection, neighbourNextDirection);

                // change next neighbour Tile to match new tile 
                ChangeNextTileToMatchNewTile(neighbourNextPosition, neighbourNextTile, DirectionGetOpposite(neighbourNextDirection));
            }
            else
            {
                // place new tile origination from previous neighbour and going straight (for now)
                PlaceConveyor(position, neighbourPreviousDirection, DirectionGetOpposite(neighbourPreviousDirection));
            }
        }
        else
        {
            // check if Open next Neighbour exists
            Vector3Int neighbourNextPosition = new Vector3Int(0, 0, 0);
            Direction neighbourNextDirection = Direction.None;
            TileBase neighbourNextTile = GetBestMatchingOpenNeighbour(position, ref neighbourNextPosition, ref neighbourNextDirection, ConnectionType.Output, neighbourPreviousTile);

            if (neighbourNextTile != null)
            {
                // change next neighbour Tile to match new tile 
                ChangeNextTileToMatchNewTile(neighbourNextPosition, neighbourNextTile, DirectionGetOpposite(neighbourNextDirection));

                // conveyor has a target conveyor/building for its own output, but no input. so place it straight
                PlaceConveyor(position, DirectionGetOpposite(neighbourNextDirection), neighbourNextDirection);
            }
            else
            {
                // start new conveyor chain as nothing is close to it
                //  = place conveyor dot
                PlaceConveyor(position, Direction.None, Direction.None);
            }
        }
    }

    private void ChangeNextTileToMatchNewTile(Vector3Int position, TileBase tile, Direction comingFrom)
    {
        // TODO check if target is rocket or building
        if (tile != null && !IsTileConveyor(tile))
        {
            // if next tile is anything but a conveyer, we don't need to modify it
            return;
        }

        Direction goingTo = GetDirectionFromTile(tile, false);

        if (comingFrom == goingTo)
        {
            // if they are the same, build straight from coming from as this is the next tile in the line
            goingTo = DirectionGetOpposite(comingFrom);
        }

        PlaceConveyor(position, comingFrom, goingTo);
    }

    private TileBase ChangePreviousTileToMatchNewTile(Vector3Int position, TileBase tile, Direction goingTo)
    {
        // TODO check if target is rocket or building
        if (tile != null && !IsTileConveyor(tile))
        {
            // if next tile is anything but a conveyer, we don't need to modify it
            return tile;
        }

        Direction comingFrom = GetDirectionFromTile(tile, true);

        if (comingFrom == goingTo)
        {
            // if they are the same, build straight to goinng to from as this is the previous  tile in the line
            comingFrom = DirectionGetOpposite(goingTo);
        }

        return PlaceConveyor(position, comingFrom, goingTo);
    }

    private TileBase PlaceConveyor(Vector3Int position, Direction neighbourPreviousDirection, Direction neighbourNextDirection)
    {
        TileBase tileToPlace = GetConveyerTileFromDirection(neighbourPreviousDirection, neighbourNextDirection);
        tilemap.SetTile(position, tileToPlace);
        return tileToPlace;
    }

    private TileBase GetBestMatchingOpenNeighbour(Vector3Int position, ref Vector3Int rocketPosition, ref Direction neighbourDirection, ConnectionType connectionType, TileBase neighbourPreviousTile)
    {
        // find neighbouring rocket as first priority - Rocket can only be connected at the conveyors output port (never on the input)
        if (connectionType.Equals(ConnectionType.Output))
        {
            if ((Math.Abs(position.x) <= 2 && Math.Abs(position.y) <= 2) && !(Math.Abs(position.x) == 2 && Math.Abs(position.y) == 2))
            {
                rocketPosition = new Vector3Int(0,0,0);
                TileBase tileRocket = tilemap.GetTile(rocketPosition);
                // thats the circle around the rocket taht connects to it

                if (position.x == 2)
                {
                    // top right row
                    neighbourDirection = Direction.SW;
                }
                else if (position.x == -2)
                {
                    // bottem left row
                    neighbourDirection = Direction.NE;
                }
                else if (position.y == -2)
                {
                    // bottom right row
                    neighbourDirection = Direction.NW;
                }
                else if (position.y == 2)
                {
                    // top left row
                    neighbourDirection = Direction.SE;
                }

                return tileRocket;
            }
        }
        
        // find neighbouring conveyors in all 4 direction
        Vector3Int positionNW = new Vector3Int(position.x + 0, position.y + 1, 0);
        Vector3Int positionNE = new Vector3Int(position.x + 1, position.y + 0, 0);
        Vector3Int positionSE = new Vector3Int(position.x + 0, position.y - 1, 0);
        Vector3Int positionSW = new Vector3Int(position.x - 1, position.y + 0, 0);

        TileBase neightbourTile = GetOpenNeighbourConveyor(positionNW, connectionType);
        if (neightbourTile != null && neightbourTile != neighbourPreviousTile)
        {
            rocketPosition = positionNW;
            neighbourDirection = Direction.NW;
            return neightbourTile;
        }

        neightbourTile = GetOpenNeighbourConveyor(positionNE, connectionType);
        if (neightbourTile != null && neightbourTile != neighbourPreviousTile)
        {
            rocketPosition = positionNE;
            neighbourDirection = Direction.NE;
            return neightbourTile;
        }

        neightbourTile = GetOpenNeighbourConveyor(positionSE, connectionType);
        if (neightbourTile != null && neightbourTile != neighbourPreviousTile)
        {
            rocketPosition = positionSE;
            neighbourDirection = Direction.SE;
            return neightbourTile;
        }

        neightbourTile = GetOpenNeighbourConveyor(positionSW, connectionType);
        if (neightbourTile != null && neightbourTile != neighbourPreviousTile)
        {
            rocketPosition = positionSW;
            neighbourDirection = Direction.SW;
            return neightbourTile;
        }

        // if not find neighbouring buildings in all 4 directions including empty buildings tiles
        // TODO

        return null;
    }


    private TileBase GetOpenNeighbourConveyor(Vector3Int position, ConnectionType connectionType)
    {
        if (tilemap.HasTile(position))
        {
            TileBase neighbourTile = tilemap.GetTile(position);

            if (IsTileConveyor(neighbourTile))
            {
                if (!IsConveyerAlreadyConnected(neighbourTile, position.x, position.y, connectionType))
                //if (IsConveyorOpenEndedOutput(neighbourTile))
                {
                    // this is a prevous open ended neighbour
                    return neighbourTile;
                }
            }
            else if (neighbourTile != null)
            {
                return neighbourTile;
            }
        }

        return null;
    }


    public bool IsTileConveyor(TileBase conveyor)
    {
        if (conveyor == null) return false;

        string key = conveyor.name.ToLowerInvariant();
        // check all next neighbours straight
        if (key.Equals(conveyor_dot.name.ToLowerInvariant()) ||
            key.Equals(conveyor_straight_nwse.name.ToLowerInvariant()) ||
            key.Equals(conveyor_straight_nesw.name.ToLowerInvariant()) ||
            key.Equals(conveyor_straight_senw.name.ToLowerInvariant()) ||
            key.Equals(conveyor_straight_swne.name.ToLowerInvariant()) ||        
            key.Equals(conveyor_curveleft_nwne.name.ToLowerInvariant()) || 
            key.Equals(conveyor_curveleft_nese.name.ToLowerInvariant()) ||
            key.Equals(conveyor_curveleft_sesw.name.ToLowerInvariant()) ||
            key.Equals(conveyor_curveleft_swnw.name.ToLowerInvariant()) ||
            key.Equals(conveyor_curveright_nwsw.name.ToLowerInvariant()) ||
            key.Equals(conveyor_curveright_nenw.name.ToLowerInvariant()) ||
            key.Equals(conveyor_curveright_sene.name.ToLowerInvariant()) ||
            key.Equals(conveyor_curveright_swse.name.ToLowerInvariant()))
        {
            return true;
        }
        
        // it's not a conveyor
        return false;
    }

    /// <summary>
    /// Checks if the Conveyers input at the Tile is connected to another connect 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="comingFrom">the direction the conveyer is coming from</param>
    /// <returns>true if connected, false if not. </returns>
    private bool IsConveyerAlreadyConnected(TileBase self, int x, int y, ConnectionType connectionType/*, ref Direction comingFrom*/)
    {
        string key = self.name.ToLowerInvariant();

        Direction comingFrom = GetDirectionFromTile(self, true);
        Direction goingTo = GetDirectionFromTile(self, false);

        if (connectionType.Equals(ConnectionType.Input))
        {
            TileBase neighbour = GetNeighbour(new Vector2Int(x, y), goingTo);
            if (HasTileAConnectionAt(neighbour, DirectionGetOpposite(goingTo), connectionType))
            {
                // check if the neighbour is coming from the same direction this tile is going to => they are connected
                return true;
            }
        }
        else if (connectionType.Equals(ConnectionType.Output))
        {
            TileBase neighbour = GetNeighbour(new Vector2Int(x, y), comingFrom);
            if (HasTileAConnectionAt(neighbour, DirectionGetOpposite(comingFrom), connectionType))
            {
                // check if the neighbour is coming from the same direction this tile is going to => they are connected
                return true;
            }
        }            

        return false;
    }

    private Direction DirectionGetOpposite(Direction direction)
    {
        if (direction.Equals(Direction.NW))
        {
            return Direction.SE;
        }
        if (direction.Equals(Direction.NE))
        {
            return Direction.SW;
        }
        if (direction.Equals(Direction.SE))
        {
            return Direction.NW;
        }
        if (direction.Equals(Direction.SW))
        {
            return Direction.NE;
        }
        return Direction.None;
    }

    /// <summary>
    /// Checks if any Conveyor Tile is coming from a given direction or going to a given direction
    /// </summary>
    /// <param name="tileBase">Any Tile</param>
    /// <param name="directionConnectionGoingTo">A Direction erelative to the given Tile</param>
    /// <returns></returns>
    private bool HasTileAConnectionAt(TileBase tileBase, Direction directionConnection, ConnectionType connectionType)
    {
        if (IsTileConveyor(tileBase))
        {
            // the Direction the tile should have the connection at
            string substringDirection = tileBase.name.Substring(tileBase.name.Length - 4, 2);

            if (connectionType.Equals(ConnectionType.Output))
            {
                substringDirection = tileBase.name.Substring(tileBase.name.Length - 2, 2);
            }

            // if the tiles name has the connection in the wanted directio, it's true
            if (tileBase != null && substringDirection.ToLowerInvariant().Equals(directionConnection.ToString().ToLowerInvariant()))
            {
                return true;
            }
        }
        else if (tileBase != null)
        {
            // if neighbour is factory or rocket
            return true;
        }

        return false;
    }

    private Direction GetDirectionFromTile(TileBase tile, bool comingFrom)
    {
        string substring = tile.name.ToLowerInvariant().Substring(tile.name.Length - 4, 2);

        if (comingFrom == false)
        {
            substring = tile.name.ToLowerInvariant().Substring(tile.name.Length - 2, 2);
        }

        if (substring.Equals("nw"))
        {
            return Direction.NW;
        }
        if (substring.Equals("ne"))
        {
            return Direction.NE;
        }
        if (substring.Equals("se"))
        {
            return Direction.SE;
        }
        if (substring.Equals("sw"))
        {
            return Direction.SW;
        }

        return Direction.None;
    }



    private TileBase GetNeighbour(Vector2Int position, Direction direction)
    {
        Vector3Int positon = new Vector3Int(position.x, position.y, 0);

        if (direction.Equals(Direction.NW))
        {
            positon = new Vector3Int(position.x, position.y + 1, 0);
        }
        if (direction.Equals(Direction.NE))
        {
            positon = new Vector3Int(position.x + 1, position.y, 0);
        }
        if (direction.Equals(Direction.SE))
        {
            positon = new Vector3Int(position.x, position.y - 1, 0);
        }
        if (direction.Equals(Direction.SW))
        {
            positon = new Vector3Int(position.x - 1, position.y, 0);
        }

        return GetNeighbour(positon.x, positon.y);
    }

    private TileBase GetNeighbour(int x, int y)
    {
        // radius around spawn for rocket
        if (Math.Abs(x) <= 1 && Math.Abs(y) <= 1)
        {
            return tilemap.GetTile(new Vector3Int(0, 0, 0));
        }

        if (tilemap.HasTile(new Vector3Int(x, y, 0)))
        {
            return tilemap.GetTile(new Vector3Int(x, y, 0));
        }
        else
        {
            return null;
        }
    }

    private TileBase GetConveyerTileFromDirection(Direction comingFrom, Direction goingTo)
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
            else if (goingTo.Equals(Direction.None))
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
            else if (goingTo.Equals(Direction.None))
            {
                return this.conveyor_straight_nwse;
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
            else if (goingTo.Equals(Direction.None))
            {
                return this.conveyor_straight_senw;
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
            else if (goingTo.Equals(Direction.None))
            {
                return this.conveyor_straight_swne;
            }
        }
        else if (comingFrom.Equals(Direction.None))
        {
            if (goingTo.Equals(Direction.NE))
            {
                return this.conveyor_straight_swne;
            }
            else if (goingTo.Equals(Direction.NW))
            {
                return this.conveyor_straight_senw;
            }
            else if (goingTo.Equals(Direction.SE))
            {
                return this.conveyor_straight_nwse;
            }
            else if (goingTo.Equals(Direction.SW))
            {
                return this.conveyor_straight_nesw;
            }
        }

        return conveyor_dot;
    }
}
