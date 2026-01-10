using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MathUtils = Utils.MathUtils;

public class Polycube : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private Transform _pivot;
    [SerializeField] private float _movementSmoothing = 16f;
    [SerializeField] private float _rotationSmoothing = 16f;
    [SerializeField] private Cube _cubePrefab;
    
    [SerializeField] private Cube[] _cubes;

    private Vector3Int[] _cubeOffsets;
    private Vector3 _targetPosition;
    private Quaternion _targetPivotRotation;
    private bool _isRotating;
    
    // Edit polycube shapes in the editor, then convert to a serialized array
#if UNITY_EDITOR
    public EditorButton getCubes = new EditorButton(nameof(GetCubes));

    public void GetCubes()
    {
        Undo.RecordObject(this, "Get Cubes");
        
        List<Cube> cubes = new();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Cube cube))
            {
                cubes.Add(cube);
            }
        }

        _cubes = cubes.ToArray();
        
        SnapCubesToGrid();
        
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Snap local positions to nearest integer 
    /// </summary>
    private void SnapCubesToGrid()
    {
        foreach (Cube cube in _cubes)
        {
            Vector3 position = cube.transform.localPosition;
            cube.transform.localPosition = Vector3Int.RoundToInt(position);
        }
    }
#endif

    public void Init()
    {
        foreach (Cube cube in _cubes)
        {
            cube.Init(this, _color);
        }
        
        UpdateCubeOffsets();

        _targetPosition = transform.position;
    }

    private void Update()
    {
        // Position smoothing
        transform.position = 
            MathUtils.ExpDecay(transform.position, _targetPosition, _movementSmoothing, Time.deltaTime);
        
        // Rotation smoothing
        if (_isRotating)
        {
            _pivot.rotation = MathUtils.ExpDecay(_pivot.rotation, _targetPivotRotation, _rotationSmoothing, Time.deltaTime);
            // Finish rotation when the current rotation is approximately equal to the target rotation
            if (Quaternion.Angle(_pivot.rotation, _targetPivotRotation) < 0.1f)
            {
                EndCurrentRotation();
            }
        }
    }
    
    public void SetTargetPosition(Vector3 position)
    {
        _targetPosition = position - _pivot.localPosition;
    }

    public void StartInteract(Cube selectedCube)
    {
        //Unparent cubes
        foreach (Cube cube in _cubes)
        {
            cube.transform.SetParent(this.transform, true);
        }
        _pivot.position = selectedCube.transform.position;
        //Reparent cubes to the pivot 
        foreach (Cube cube in _cubes)
        {
            cube.transform.SetParent(_pivot, true);
        }
        
        selectedCube.SetHighlight(Cube.HighlightType.Selected);
    }

    public void EndInteract()
    {
        // In case the polycube is being rotated when dropped
        if (_isRotating)
        {
            EndCurrentRotation();
        }

        foreach (Cube cube in _cubes)
        {
            cube.SetHighlight(Cube.HighlightType.None);
        }
    }

    public void IsHoveredOver(bool isHoveredOver)
    {
        foreach (Cube cube in _cubes)
        {
            cube.SetHighlight(isHoveredOver ? Cube.HighlightType.Hovered : Cube.HighlightType.None);
        }
    }

    public Vector3 GetPivotOffset()
    {
        return _pivot.localPosition;
    }
    
    /// <returns>The world positions of the cubes</returns>
    public Vector3Int[] GetCubePositions()
    {
        Vector3Int[] positions = new Vector3Int[_cubes.Length];
        for (var i = 0; i < _cubes.Length; i++)
        {
            positions[i] = Vector3Int.RoundToInt(_targetPosition + _cubeOffsets[i]);
        }

        return positions;
    }
    
    private void UpdateCubeOffsets()
    {
        _cubeOffsets = new Vector3Int[_cubes.Length];
        Vector3Int[] offsets = new Vector3Int[_cubes.Length];
        for (var i = 0; i < _cubes.Length; i++)
        {
            Vector3 offset = this.transform.InverseTransformPoint(_cubes[i].transform.position);
            offsets[i] = Vector3Int.RoundToInt(offset);
        }

        _cubeOffsets = offsets;
    }
    
    /// <returns>The offsets of the cubes relative to the polycube transform</returns>
    public Vector3Int[] GetCubeOffsets()
    {
        return _cubeOffsets;
    }
    
    /// <param name="rotationAxis">vector to rotate around. always rotates 90 degrees clockwise</param>
    public void RotateAroundPivot(Vector3 rotationAxis)
    {
        // If already mid rotation, apply the previous rotation immediately then move on to the new one.
        if (_isRotating)
        {
            EndCurrentRotation();
        }

        _isRotating = true;
        
        // Set new target rotation, which will be rotated to in update
        Quaternion delta = Quaternion.AngleAxis(90, rotationAxis);
        _targetPivotRotation = delta * _pivot.rotation;
    }

    private void EndCurrentRotation()
    {
        _isRotating = false;
        _pivot.rotation = _targetPivotRotation;
        
        UpdateCubeOffsets();
    }
    
    // Save Resume
    public SaveResumeSystem.PolycubeSaveData GetPolycubeSaveData()
    {
        SaveResumeSystem.PolycubeSaveData polycubeData = new SaveResumeSystem.PolycubeSaveData()
        {
            PolycubeName = gameObject.name,
            PolycubeColor = _color,
            PolycubePosition = Vector3Int.RoundToInt(transform.position),
            CubeOffsets = _cubeOffsets,
        };
        return polycubeData;
    }

    public void SetStateFromSave(SaveResumeSystem.PolycubeSaveData saveData)
    {
        gameObject.name = saveData.PolycubeName;
        transform.position = saveData.PolycubePosition;
        _color = saveData.PolycubeColor;
        _cubes = new Cube[saveData.CubeOffsets.Length];
        
        for (int i = 0; i < saveData.CubeOffsets.Length; i++)
        {
            Cube cube = Instantiate(_cubePrefab, transform);
            _cubes[i] = cube;
            cube.transform.localPosition = saveData.CubeOffsets[i];
            cube.Init(this, _color);
        }
    }
}
