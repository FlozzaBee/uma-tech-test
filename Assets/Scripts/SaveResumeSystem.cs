using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveResumeSystem
{
    [System.Serializable]
    public class PolycubeSaveData
    {
        public string PolycubeName;
        public Color PolycubeColor;
        public Vector3Int PolycubePosition;
        public Vector3Int[] CubeOffsets;
    }

    public class SaveData
    {
        public PolycubeSaveData[] Polycubes;
    }
    
    public SaveResumeSystem()
    {
        // SaveState();
        // TryLoadState();
    }
    
    public void SaveState()
    {
        SaveData saveData = PopulateSaveData();

        string json = JsonUtility.ToJson(saveData, true);
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        
        File.WriteAllText(path, json);
    }

    public void TryLoadState()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");

        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        
        Systems.GameGridSystem.PopulateGridFromSave(saveData);
    }

    /// <summary>
    /// Resets the save and reloads the default scene
    /// </summary>
    public void ResetState()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        Systems.Reset();
        int index =SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
        
    }

    private SaveData PopulateSaveData()
    {
        SaveData saveData = new SaveData()
        {
            Polycubes = new PolycubeSaveData[Systems.GameGridSystem.Polycubes.Length],
        };
        for (var i = 0; i < saveData.Polycubes.Length; i++)
        {
            saveData.Polycubes[i] = Systems.GameGridSystem.Polycubes[i].GetPolycubeSaveData();
        }

        return saveData;
    }
}
