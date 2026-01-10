using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MathUtils = Utils.MathUtils;

public class PolycubeMovementManager : MonoBehaviour
{
    private Transform _cam;

    private bool _isMovingPolycube = false;
    private Polycube _currentlyHeldPolycube;
    private Polycube _currentlyHoveredPolycube;

    private Vector3Int _lastCellAlongRay;
    
    //Debug
    private List<Vector3> _voxelIntersections = new List<Vector3>();
    private List<Vector3Int> _voxelsAlongRay = new List<Vector3Int>();
    
    private void Start()
    {
        _cam = Camera.main.transform;
    }

    private void OnEnable()
    {
        Systems.InputSystem.ActionDefinitions.Interact.Subscribe(OnInteract);
        Systems.InputSystem.ActionDefinitions.RotateX.Subscribe(OnRotateX);
        Systems.InputSystem.ActionDefinitions.RotateY.Subscribe(OnRotateY);
        Systems.InputSystem.ActionDefinitions.RotateZ.Subscribe(OnRotateZ);
    }

    private void OnDisable()
    {
        Systems.InputSystem.ActionDefinitions.Interact.Unsubscribe(OnInteract);
        Systems.InputSystem.ActionDefinitions.RotateX.Unsubscribe(OnRotateX);
        Systems.InputSystem.ActionDefinitions.RotateY.Unsubscribe(OnRotateY);
        Systems.InputSystem.ActionDefinitions.RotateZ.Unsubscribe(OnRotateZ);
    }

    private void Update()
    {
        if (_isMovingPolycube)
        {
            // End interaction if polycube becomes null (i.e. if destroyed by loading a save)
            if (_currentlyHeldPolycube == null)
            {
                OnInteract();
            }
            // Cast ray through grid, finding all unoccupied cells along the ray
            _lastCellAlongRay = GetFurthestCellAlongRay();

            // Check if a few of the last intersections are valid placement
            int depthToCheck = Mathf.Min(_voxelsAlongRay.Count, 20);
            for (int i = 1; i < depthToCheck; i++)
            {
                var candidateCell = _voxelsAlongRay[^i];
                // Account for offset from pivot
                Vector3Int candidatePolycubePosition = Vector3Int.RoundToInt(candidateCell - _currentlyHeldPolycube.GetPivotOffset()); 
                if (Systems.GameGridSystem.IsPolycubePositionValid(_currentlyHeldPolycube, candidatePolycubePosition))
                {
                    _currentlyHeldPolycube.SetTargetPosition(candidateCell);
                    break;
                }
            }
            
            // Set rotation visualiser position
            RotationVisualiser.Instance.transform.position =
                _currentlyHeldPolycube.transform.position + _currentlyHeldPolycube.GetPivotOffset();
        }
        else
        {
            Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
            if (Physics.Raycast(ray, out var hit) && hit.transform.TryGetComponent(out Cube cube))
            {
                if (cube.GetPolycube() != _currentlyHoveredPolycube)
                {
                    // Using ?.IsHoveredOver breaks here if the hovered object is destroyed (i.e. by loading a save),
                    // so use explicit check
                    if (_currentlyHoveredPolycube != null)  
                    {
                        _currentlyHoveredPolycube.IsHoveredOver(false);
                    }
                    _currentlyHoveredPolycube = cube.GetPolycube();
                    _currentlyHoveredPolycube?.IsHoveredOver(true);
                }
            }
            else if(_currentlyHoveredPolycube != null)
            {
                _currentlyHoveredPolycube?.IsHoveredOver(false);
                _currentlyHoveredPolycube = null;
            }
        }
    }
    
    public void ForceEndInteract()
    {
        if (_isMovingPolycube)
        {
            OnInteract();
        }
    }
    
    private void OnInteract()
    {
        if (_isMovingPolycube)
        {
            _currentlyHeldPolycube.EndInteract();
            Systems.GameGridSystem.SetPolycubeState(_currentlyHeldPolycube, true);
            _isMovingPolycube = false;
            _currentlyHeldPolycube = null;
        }
        else
        {
            if (_currentlyHoveredPolycube != null)
            {
                _currentlyHoveredPolycube.IsHoveredOver(false);
            }
            _currentlyHoveredPolycube = null;
            Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out Cube cube))
                {
                    _currentlyHeldPolycube = cube.GetPolycube();
                    _currentlyHeldPolycube.StartInteract(cube);
                    _isMovingPolycube = true;
                    Systems.GameGridSystem.SetPolycubeState(_currentlyHeldPolycube, false);
                }
            }
        }
    }
    
    private void OnRotateX()
    {
        if (_isMovingPolycube)
        {
            _currentlyHeldPolycube.RotateAroundPivot(Vector3.right);
            RotationVisualiser.Instance.VisualiseRotation(RotationVisualiser.Axis.X);
        }
    }
    
    private void OnRotateY()
    {
        if (_isMovingPolycube)
        {
            _currentlyHeldPolycube.RotateAroundPivot(Vector3.up);
            RotationVisualiser.Instance.VisualiseRotation(RotationVisualiser.Axis.Y);
        }
    }
    
    private void OnRotateZ()
    {
        if (_isMovingPolycube)
        {
            _currentlyHeldPolycube.RotateAroundPivot(Vector3.forward);
            RotationVisualiser.Instance.VisualiseRotation(RotationVisualiser.Axis.Z);
        }
    }

    
    // Fast Voxel Traversal Algorithm to get the grid cell we are looking at.
    private Vector3Int GetFurthestCellAlongRay()
    {
        #if UNITY_EDITOR
        // Recording voxel intersections for debug only.
        _voxelIntersections.Clear();
        #endif
        
        _voxelsAlongRay.Clear();
        Vector3 start = _cam.position;
        Vector3 dir = _cam.forward;
        // Start from current cell
        Vector3Int cell = Vector3Int.RoundToInt(start);

        // Unfortunate side effect of this method is that the camera must be in the grid for it to work :(
        // Could be fixed but for now it's fine.
        if (!Systems.GameGridSystem.IsCellInsideGrid(cell))
        {
            return default;
        }

        Vector3Int lastValid = cell;
        
        //Until we hit a blocked cell/exit the grid
        int maxSteps = 500; 
        for (int i = 0; i < maxSteps; i++)
        {
            // Calculate distances to the next voxel boundaries for each axis
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


            // Find the axis with the shortest step, which is the next voxel the ray enters.
            float t;
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

            

            if (!Systems.GameGridSystem.IsCellInsideGrid(cell) || Systems.GameGridSystem.IsCellOccupied(cell))
            {
                break;
            }
            
            _voxelsAlongRay.Add(cell);
            lastValid = cell;
            
#if UNITY_EDITOR
            Vector3 intersectionPoint = start + dir * t;
            _voxelIntersections.Add(intersectionPoint);
#endif
        }

        return lastValid;
    }
    
    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
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

            foreach (var voxel in _voxelsAlongRay)
            {
                Gizmos.DrawWireCube(voxel, Vector3.one);
            }
        }
    }
#endif
}
