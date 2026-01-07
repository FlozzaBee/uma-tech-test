using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolycubeMovementManager : MonoBehaviour
{
    [SerializeField] private GameGrid _gameGrid;
    
    private Transform _cam;

    private bool _isMovingPolycube = false;
    private Polycube _currentlyHeldPolycube;
    private Cube _selectedCubeOfPolycube;

    private Vector3Int _lastCellAlongRay;
    
    //Debug
    private List<Vector3> _voxelIntersections = new List<Vector3>();
    private List<Vector3Int> _voxelsTravered = new List<Vector3Int>();
    
    private void Start()
    {
        _cam = Camera.main.transform;
    }

    private void OnEnable()
    {
        Systems.InputSystem.ActionDefinitions.Interact.Subscribe(OnInteract);
    }

    private void OnDisable()
    {
        Systems.InputSystem.ActionDefinitions.Interact.Unsubscribe(OnInteract);
    }

    private void Update()
    {
        if (!_isMovingPolycube)
        {
            return;
        }
        
        //Cast ray through grid, finding all unoccupied cells along the ray
        _lastCellAlongRay = GetFurthestCellAlongRay();

        _currentlyHeldPolycube.transform.position = _lastCellAlongRay - _selectedCubeOfPolycube.transform.localPosition;
    }
    
    private void OnInteract()
    {
        if (_isMovingPolycube)
        {
            //Try drop polycube
            //if successful
            
            _isMovingPolycube = false;
            _currentlyHeldPolycube = null;
        }
        else
        {
            Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out Cube cube))
                {
                    _currentlyHeldPolycube = cube.GetPolycube();
                    _selectedCubeOfPolycube = cube;
                    _isMovingPolycube = true;
                    Debug.Log($"Hit cube {cube.GetPolycube().name}");
                }
            }
        }
    }

    // Fast Voxel Traversal Algorithm 
    private Vector3Int GetFurthestCellAlongRay()
    {
        _voxelIntersections.Clear();
        _voxelsTravered.Clear();
        Vector3 start = _cam.position;
        Vector3 dir = _cam.forward;
        Vector3Int cell = Vector3Int.RoundToInt(start);

        if (!IsInsideGrid(cell))
        {
            return default;
        }

        Vector3Int lastValid = cell;
        
        int maxSteps = 500; 
        for (int i = 0; i < maxSteps; i++)
        {
            // Calculate distances to the next voxel boundaries along each axis
            Vector3 tMax = new Vector3(
                dir.x != 0
                    ? ((dir.x > 0 ? cell.x + 0.5f : cell.x - 0.5f) - start.x) / dir.x
                    : float.PositiveInfinity, 
                // +- 0.5f as I want our coordinates to represent the center of reach cell, not the corner. 
                // This makes placing our cubes a little tidier i think.
                // if direction is axis aligned, this will never be the next axis to step along, so set to inf

                dir.y != 0
                    ? ((dir.y > 0 ? cell.y + 0.5f : cell.y - 0.5f) - start.y) / dir.y
                    : float.PositiveInfinity,

                dir.z != 0
                    ? ((dir.z > 0 ? cell.z + 0.5f : cell.z - 0.5f) - start.z) / dir.z
                    : float.PositiveInfinity
            );

            float t;

            // Find the axis with the shortest step, which is the next voxel the ray enters.
            if (tMax.x < tMax.y && tMax.x < tMax.z)
            {
                t = tMax.x;
                cell.x += dir.x > 0 ? 1 : -1;
            }
            else if (tMax.y < tMax.z)
            {
                t = tMax.y;
                cell.y += dir.y > 0 ? 1 : -1;
            }
            else
            {
                t = tMax.z;
                cell.z += dir.z > 0 ? 1 : -1;
            }

            Vector3 intersectionPoint = start + dir * t;
            _voxelIntersections.Add(intersectionPoint);;

            if (!IsInsideGrid(cell) || _gameGrid.IsCellOccupied(cell))
            {
                break;
            }
            
            _voxelsTravered.Add(cell);

            lastValid = cell;
        }
        
        Debug.Log(lastValid);

        return lastValid;
    }
    
    private bool IsInsideGrid(Vector3Int cell)
    {
        return cell.x >= 0 && cell.x < _gameGrid.GridSize.x &&
               cell.y >= 0 && cell.y < _gameGrid.GridSize.y &&
               cell.z >= 0 && cell.z < _gameGrid.GridSize.z;
    }

    private void OnDrawGizmos()
    {
        if (_isMovingPolycube)
        {
            var size = Vector3.one * 0.9f;
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_lastCellAlongRay, size);
            
            Gizmos.DrawRay(_cam.position, _cam.forward * 100);

            Gizmos.color = Color.yellow;
            foreach (var voxelIntersection in _voxelIntersections)
            {
                Gizmos.DrawSphere(voxelIntersection, 0.1f);
            }

            foreach (var voxel in _voxelsTravered)
            {
                Gizmos.DrawWireCube(voxel, Vector3.one);
            }
        }
        
    }
}
