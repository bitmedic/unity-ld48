using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UndirectionalConveyors : MonoBehaviour
{
    [Header("Factory Tilemap")]
    public Tilemap tilemap;

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

    public void BuildNewConveyor(Vector3Int position, TileBase tileToPlace)
    {
        BuildNewConveyor(position, tileToPlace, position); 
    }

    public void RemoveConveyor(Vector3Int position)
    {
        TileBase removedConveyor = tilemap.GetTile(position);
        tilemap.SetTile(position, null);

        Direction outputDir = GetOutputDirectionFromTile(removedConveyor);
        TileBase nextNeighbour = GetNeighbour(position, outputDir);
        Vector3Int nextNeighbourPosition = GetNeighbourPosition(position, outputDir);

        if (nextNeighbour != null && IsTileConveyor(nextNeighbour))
        {
            BuildNewConveyor(nextNeighbourPosition, nextNeighbour);
        }
    }

    private void BuildNewConveyor(Vector3Int position, TileBase tileToPlace, Vector3Int firstPosition)
    {
        Debug.Log("Building " + tileToPlace.name + " at " + position);

        Direction outputDir = GetOutputDirectionFromTile(tileToPlace);
        Direction inputDir = GetInputDirectionFromTile(tileToPlace);

        List<Direction> incomingConveyors = GetIncomingConveyors(position, outputDir);

        if (incomingConveyors.Count > 0)
        {
            if (incomingConveyors.Count == 1)
            {
                PlaceConveyor(position, incomingConveyors[0], outputDir);
            }
            else
            {
                PlaceConveyor(position, DirectionGetOpposite(outputDir), outputDir); // for now, straight always wins; this should be extended for merging
            }
        }
        else
        {
            PlaceConveyor(position, inputDir, outputDir);
        }

        Vector3Int outputNeighborPosition = GetNeighbourPosition(position, outputDir);
        TileBase outputNeighborTile = GetNeighbour(position, outputDir);
        if (IsTileConveyor(outputNeighborTile))
        {   
            if (position == firstPosition) // cycle breaker
            {
                BuildNewConveyor(outputNeighborPosition, outputNeighborTile, firstPosition);
            }
            else
            {
                // Debug.Log("No updates necessary for downstream conveyors");
            }
        }
        return;
    }

    private List<Direction> GetIncomingConveyors(Vector3Int position, Direction outputDir)
    {
        List<Direction> incomingDirections = new List<Direction>();

        AddDirectionIfIncomingFrom(Direction.NE);
        AddDirectionIfIncomingFrom(Direction.SE);
        AddDirectionIfIncomingFrom(Direction.SW);
        AddDirectionIfIncomingFrom(Direction.NW);

        return incomingDirections;

        void AddDirectionIfIncomingFrom(Direction checkDirection)
        {
            if (outputDir != checkDirection)
            {
                TileBase neighborTileNE = GetNeighbour(position, checkDirection);
                if (neighborTileNE != null)
                {
                    Direction opposite = DirectionGetOpposite(checkDirection);
                    if (GetOutputDirectionFromTile(neighborTileNE) == opposite)
                    {
                        Debug.Log("There is a conveyor in " + checkDirection + " that goes to " + opposite);
                        incomingDirections.Add(checkDirection);
                    }
                }
            }
        }
    }

    private TileBase PlaceConveyor(Vector3Int position, Direction neighbourPreviousDirection, Direction neighbourNextDirection)
    {
        TileBase tileToPlace = GetConveyerTileFromDirection(neighbourPreviousDirection, neighbourNextDirection);
        tilemap.SetTile(position, tileToPlace);
        return tileToPlace;
    }

    public bool IsTileConveyor(TileBase conveyor)
    {
        if (conveyor == null) return false;

        string key = conveyor.name.ToLowerInvariant();
        // check all next neighbours straight
        if (
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

    private Direction GetInputDirectionFromTile(TileBase tile)
    {
        return GetDirectionFromTile(tile, true);
    }

    private Direction GetOutputDirectionFromTile(TileBase tile)
    {
        return GetDirectionFromTile(tile, false);
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



    private TileBase GetNeighbour(Vector3Int position, Direction direction)
    {
        Vector3Int neighbourPosition = GetNeighbourPosition(position, direction);
        return GetNeighbourAt(neighbourPosition.x, neighbourPosition.y);
    }

    private static Vector3Int GetNeighbourPosition(Vector3Int position, Direction direction)
    {
        Vector3Int neighbourPosition = new Vector3Int(position.x, position.y, 0);

        if (direction.Equals(Direction.NW))
        {
            neighbourPosition = new Vector3Int(position.x, position.y + 1, 0);
        }
        if (direction.Equals(Direction.NE))
        {
            neighbourPosition = new Vector3Int(position.x + 1, position.y, 0);
        }
        if (direction.Equals(Direction.SE))
        {
            neighbourPosition = new Vector3Int(position.x, position.y - 1, 0);
        }
        if (direction.Equals(Direction.SW))
        {
            neighbourPosition = new Vector3Int(position.x - 1, position.y, 0);
        }

        return neighbourPosition;
    }

    private TileBase GetNeighbourAt(int x, int y)
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

        throw new InvalidOperationException();  // should never happen ;-)
    }
}
