using System.Collections.Generic;
using UnityEngine;

public class GameGridManager : MonoBehaviour
{
    [SerializeField] private Vector3Int _gridSize = new Vector3Int(32, 16, 32);
    [SerializeField] private Polycube _polycubePrefab;
    [SerializeField] private Transform _polycubeRoot;

    public Vector3Int GridSize => _gridSize; 

    public Polycube[] LoadPolycubeStates(SaveResumeSystem.SaveData saveData)
    {
        // Remove any existing polycubes
        foreach (Transform child in _polycubeRoot)
        {
            Destroy(child.gameObject);
        }
        
        // Instantiate new polycubes from the save data, then set their state
        Polycube[] polycubes = new Polycube[saveData.Polycubes.Length];
        for (var i = 0; i < saveData.Polycubes.Length; i++)
        {
            var polycubeSaveData = saveData.Polycubes[i];
            Polycube polycube = Instantiate(_polycubePrefab, _polycubeRoot);
            polycube.SetStateFromSave(polycubeSaveData);
            polycubes[i] = polycube;
        }

        return polycubes;
    }

    public Polycube[] GetExistingPolycubes()
    {
        List<Polycube> polycubes = new List<Polycube>();
        foreach (Transform child in _polycubeRoot.transform)
        {
            if (child.TryGetComponent(out Polycube polycube))
            {
                polycubes.Add(polycube);
            }
        }

        return polycubes.ToArray();
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var gridSize = GridSize;
        
        // Draw valid play space outline
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((gridSize / 2) - (Vector3.one * 0.5f), gridSize);

        if (!Application.isPlaying)
        {
            return;
        }
        
        // Draw occupied cells
        Gizmos.color = Color.red;
        for (var x = 0; x < gridSize.x; x++)
        for (var y = 0; y < gridSize.y; y++)
        for (var z = 0; z < gridSize.z; z++)
        {
            if (Systems.GameGridSystem.IsCellOccupied(new Vector3Int(x, y, z)))
            {
                Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one);
            }
        }
    }
#endif
}
