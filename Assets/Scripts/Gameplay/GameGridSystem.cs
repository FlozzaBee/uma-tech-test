using System.Collections.Generic;
using UnityEngine;

public class GameGridSystem
{
    public Vector3Int GridSize => _gameGridManager.GridSize;
    public Polycube[] Polycubes;

    private GameGridManager _gameGridManager;
    
    private struct Cell
    {
        public bool IsOccupied;
    }
    
    // 3d array of cells
    private Cell[,,] _cells;

    public GameGridSystem(GameGridManager gameGameGridManager)
    {
        _gameGridManager = gameGameGridManager;
        Polycubes = gameGameGridManager.GetExistingPolycubes();
        
        _cells = new Cell[GridSize.x, GridSize.y, GridSize.z];
        
        // Populate grid from existing polycubes
        InitPolycubes();
    }

    private void InitPolycubes()
    {
        foreach (Polycube polycube in Polycubes)
        {
            polycube.Init();
            SetPolycubeState(polycube, true);
        }
    }
    
    public bool IsCellInsideGrid(Vector3Int cell)
    {
        return cell.x >= 0 && cell.x < GridSize.x &&
               cell.y >= 0 && cell.y < GridSize.y &&
               cell.z >= 0 && cell.z < GridSize.z;
    }

    public bool IsCellOccupied(Vector3Int cell)
    {
        return _cells[cell.x, cell.y, cell.z].IsOccupied;
    }

    public void SetPolycubeState(Polycube polycube, bool occupied)
    {
        foreach (var c in polycube.GetCubePositions())
        {
            _cells[c.x, c.y, c.z].IsOccupied = occupied;
        }
    }

    public bool IsPolycubePositionValid(Polycube polycube, Vector3Int position)
    {
        foreach (var cubeOffset in polycube.GetCubeOffsets())
        {
            Vector3Int c = position + cubeOffset;
            if (!IsCellInsideGrid(c) || _cells[c.x, c.y, c.z].IsOccupied)
            {
                return false;
            }
        }

        return true;
    }

    public void PopulateGridFromSave(SaveResumeSystem.SaveData saveData)
    {
        // Reset the grid
        _cells = new Cell[GridSize.x, GridSize.y, GridSize.z];
        
        Polycubes = _gameGridManager.LoadPolycubeStates(saveData);
        
        InitPolycubes();
    }
}
