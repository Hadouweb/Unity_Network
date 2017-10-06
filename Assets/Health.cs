using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public const int maxHealth = 100;
    
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public bool destroyOnDeath;
    private NetworkStartPosition[] spawnPoints;
    
    public RectTransform healthBar;

    private void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }
    
    public void takeDamage(int amount)
    {
        if (!isServer)
            return;
        currentHealth -= amount;
        if (currentHealth > 0) 
            return;
        if (destroyOnDeath)
        {
            Destroy(gameObject);
            return;
        }
        currentHealth = maxHealth;
        RpcRespawn();
    }

    private void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    private void RpcRespawn()
    {
        if (!isLocalPlayer) 
            return;
        var spawnPoint = Vector3.zero;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }
        transform.position = spawnPoint;
    }
}
