using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameGridSystem
{
    public Vector3Int GridSize => GameGridManager.GridSize;
    public Polycube[] Polycubes;

    private GameGridManager GameGridManager
    {
        get
        {
            if (_gameGridManager == null)
            {
                _gameGridManager = Object.FindFirstObjectByType<GameGridManager>();
            }

            return _gameGridManager;
        }
    }

    private GameGridManager _gameGridManager;
    private bool _firstSceneOpen = true;
    
    private struct Cell
    {
        public bool IsOccupied;
    }
    
    // 3d array of cells
    private Cell[,,] _cells;

    public GameGridSystem()
    {
        Init();

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    // On scene reload, probably wouldn't need this in a more production ready game, but for this prototype it's
    // useful to reload the default scene
    private void OnSceneChanged(Scene arg0, Scene scene)
    {
        // Dont trigger on first game start
        if (_firstSceneOpen)
        {
            _firstSceneOpen = false;
            return;
        }
        _gameGridManager = null;
        Init();
    }

    private void Init()
    {
        _cells = new Cell[GridSize.x, GridSize.y, GridSize.z];
        Polycubes = GameGridManager.GetExistingPolycubes();
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
        
        Polycubes = GameGridManager.LoadPolycubeStates(saveData);
        
        InitPolycubes();
    }
}
