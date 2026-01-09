using UnityEngine;

public class SceneStart : MonoBehaviour
{
    [SerializeField] private GameGridManager _gameGridManager;
    void Awake()
    {
        Systems.Init(_gameGridManager);
    }
}
