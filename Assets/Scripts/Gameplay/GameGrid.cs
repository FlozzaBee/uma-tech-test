using System;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private Vector3Int _gridSize = new Vector3Int(32, 16, 32);
    public Vector3Int GridSize => _gridSize;

    [SerializeField] private Polycube[] _polycubes;

    public struct Cell
    {
        public bool IsOccupied;
    }
    
    //3d array of cells
    private Cell[,,] _cells;

    private void Start()
    {
        _cells = new Cell[_gridSize.x, _gridSize.y, _gridSize.z];
        
        //Populate grid from existing polycubes
        foreach (Polycube polycube in _polycubes)
        {
            polycube.Init();
            SetPolycubeState(polycube, true);
        }


        for (var x = 0; x < _cells.GetLength(0); x++)
        for (var y = 0; y < _cells.GetLength(1); y++)
        for (var z = 0; z < _cells.GetLength(2); z++)
        {
            if (_cells[x, y, z].IsOccupied)
            {
                Debug.Log($"Occupied {x} {y} {z}");
            }
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
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawWireCube((_gridSize / 2) - (Vector3.one * 0.5f), _gridSize);

        if (!Application.isPlaying)
        {
            return;
        }
        
        Gizmos.color = Color.red;
        for (var x = 0; x < _cells.GetLength(0); x++)
        for (var y = 0; y < _cells.GetLength(1); y++)
        for (var z = 0; z < _cells.GetLength(2); z++)
        {
            if (_cells[x, y, z].IsOccupied)
            {
                Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one);
            }
        }
    }
#endif
}
