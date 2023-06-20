using System;
using UnityEditor;
using UnityEngine;

public class PlayerSpawner : ControllerBase
{
    // Public fields
    
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private float handleArcRadius = 1f;

    // Private fields
    
    private PlayerController spawnedPlayer;

    private bool isInitialized;
    
    // Events

    public event Action<PlayerController> OnPlayerSpawned;
    
    // PlayerSpawner
    
    public void Init()
    {
        isInitialized = true;
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var spawnHeight = playerPrefab.GetSpawnHeight();
        var spawnPosition = transform.position + new Vector3(0f, spawnHeight, 0f);
        
        spawnedPlayer = Instantiate(playerPrefab, spawnPosition, transform.rotation);
        spawnedPlayer.Init();
        
        OnPlayerSpawned?.Invoke(spawnedPlayer);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, handleArcRadius);
    }
    
#endif
}
