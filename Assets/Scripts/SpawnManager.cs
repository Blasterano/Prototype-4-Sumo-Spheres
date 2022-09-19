using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject bossPrefab;
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefabs;
    public GameObject[] miniEnemyPrefab;
    public int bossRound;
    public int enemyCount;
    public AudioClip clip;

    private AudioSource audioSource;
    private int waveNumber = 1;
    private float spawnRange=9.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
        SpawnEnemyWave(waveNumber);
        Instantiate(powerupPrefabs[GeneratePowerupId()], GenerateSpawnPosition(), powerupPrefabs[GeneratePowerupId()].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if (enemyCount == 0)
        {
            waveNumber++;

            if (waveNumber % bossRound == 0)
                SpawnBossWave(waveNumber);
            else
                SpawnEnemyWave(waveNumber);
            
            Instantiate(powerupPrefabs[GeneratePowerupId()], GenerateSpawnPosition(), powerupPrefabs[GeneratePowerupId()].transform.rotation);
            audioSource.PlayOneShot(clip);
        }
    }

    public int GetWaveCount()
    {
        return waveNumber;
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        int spawnId;
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            spawnId = Random.Range(0, enemyPrefab.Length);
            Instantiate(enemyPrefab[spawnId], GenerateSpawnPosition(), enemyPrefab[spawnId].transform.rotation);

        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return randomPos;
    }

    private void SpawnBossWave(int currentRound)
    {
        int miniEnemysToSpawn;

        // We don't want to divide by zero
        if (bossRound != 0)
        {
            miniEnemysToSpawn = currentRound / bossRound;
        }
        else
            miniEnemysToSpawn = 1;

        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for(int i=0;i<amount;i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefab.Length);
            Instantiate(miniEnemyPrefab[randomMini], GenerateSpawnPosition(), miniEnemyPrefab[randomMini].transform.rotation);
        }
    }

    private int GeneratePowerupId()
    {
        int randomPowerup = Random.Range(0, powerupPrefabs.Length);
        return randomPowerup;    
    }
}
