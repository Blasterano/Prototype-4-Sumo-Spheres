using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text[] text;
    public GameObject gameOver;
    public AudioClip clip;
    public bool gameover = false;

    private PlayerController playerController;
    private SpawnManager spawnManager;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        spawnManager = FindObjectOfType<SpawnManager>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isdead && playerController != null)
        {
            gameover = true;
            Destroy(playerController.gameObject);
            gameOver.SetActive(true);
            text[0].text = "Your Score : " + spawnManager.GetWaveCount();
            audioSource.PlayOneShot(clip);
        }
        else
        {
            text[1].text = "Wave : " + spawnManager.GetWaveCount();
            gameover = false;   
        }

    }


}
