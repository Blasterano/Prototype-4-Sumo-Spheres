using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f;
    public bool isBoss = false;
    public float spawnIntervel;
    public int miniEnemySpawnCount;

    private float nextSpawn;
    private Rigidbody enemyRb;
    private GameObject player;
    private SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");

        if (isBoss)
            spawnManager = FindObjectOfType<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector3 lookDirection = (player.transform.position - transform.position).normalized;
            enemyRb.AddForce(lookDirection * speed);
        }

        if(isBoss)
        {
            if(Time.time>nextSpawn)
            {
                nextSpawn = Time.time + spawnIntervel;
                spawnManager.SpawnMiniEnemy(miniEnemySpawnCount);
            }
        }

        if (transform.position.y < -10)
            Destroy(gameObject);
    }
}
