using System;
using System.Collections.Generic;
using UnityEngine;

public class Polycube : MonoBehaviour
{
    [SerializeField] private Cube[] _cubes;
    
    //Edit polycube shapes in the editor, then convert to an array
#if UNITY_EDITOR
    public EditorButton getCubes = new EditorButton(nameof(GetCubes));

    public void GetCubes()
    {
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

    private void Start()
    {
        foreach (Cube cube in _cubes)
        {
            cube.Init(this);
        }
    }
    
    public Vector3Int[] GetCubePositions()
    {
        Vector3Int[] positions = new Vector3Int[_cubes.Length];
        for (var i = 0; i < _cubes.Length; i++)
        {
            positions[i] = Vector3Int.RoundToInt(transform.position + _cubes[i].transform.localPosition);
        }

        return positions;
    }
}
