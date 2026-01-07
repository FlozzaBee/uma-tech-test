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
            foreach (var position in polycube.GetCubePositions())
            {
                _cells[position.x, position.y, position.z].IsOccupied = true;
            }
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

    public bool IsCellOccupied(Vector3Int cell)
    {
        return _cells[cell.x, cell.y, cell.z].IsOccupied;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawWireCube(_gridSize/2, _gridSize);
    }
#endif
}
