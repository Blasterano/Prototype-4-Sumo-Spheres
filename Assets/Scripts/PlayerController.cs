using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public bool hasPowerup = false;
    public GameObject[] powerupIndicators;
    public PowerupType currentPowerup = PowerupType.None;
    public GameObject rocketPrefab;
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    public bool isdead = false;
    public AudioClip hit, powerup;

    private float powerupStrength=15.0f;
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;
    private AudioSource audioSource;
    private bool smashing = false;
    private float floorY;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        if (currentPowerup == PowerupType.Pushback)
            powerupIndicators[0].transform.position = transform.position + new Vector3(0, -0.5f, 0);
        else if(currentPowerup==PowerupType.Rocket)
            powerupIndicators[1].transform.position = transform.position + new Vector3(0, -0.5f, 0);
        else if (currentPowerup == PowerupType.Smash)
            powerupIndicators[2].transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (currentPowerup == PowerupType.Rocket && Input.GetKeyDown(KeyCode.F))
            LaunchRocket();

        if (currentPowerup == PowerupType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }

        if (transform.position.y < -2)
            isdead = true;
        else
            isdead = false;
}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            currentPowerup = other.gameObject.GetComponent<Powerup>().powerupType;

            if (currentPowerup == PowerupType.Pushback)
                powerupIndicators[0].gameObject.SetActive(true);
            else if (currentPowerup == PowerupType.Rocket)
                powerupIndicators[1].gameObject.SetActive(true);
            else if (currentPowerup == PowerupType.Smash)
                powerupIndicators[2].gameObject.SetActive(true);

            Destroy(other.gameObject);

            if (powerupCountdown != null)
                StopCoroutine(powerupCountdown);
            
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());

            audioSource.PlayOneShot(powerup);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if (currentPowerup == PowerupType.Pushback)
            {
                Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

                enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
                Debug.Log("Player collided with " + collision.gameObject.name + " with powerset to " + currentPowerup.ToString());
            }
            audioSource.PlayOneShot(hit);
        }
    }

    private void LaunchRocket()
    {
        foreach(var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;

        if (currentPowerup == PowerupType.Pushback)
            powerupIndicators[0].gameObject.SetActive(false);
        else if (currentPowerup == PowerupType.Rocket)
            powerupIndicators[1].gameObject.SetActive(false);
        else if (currentPowerup == PowerupType.Smash)
            powerupIndicators[2].gameObject.SetActive(false);

        currentPowerup = PowerupType.None;
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        // we will store y position of player before taking off
        floorY = transform.position.y;

        // calculate amount of time we will go up
        float jumpTime = Time.time + hangTime;

        while(Time.time<jumpTime)
        {
            // move player up while still keeping their x velocity
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        // now move the player down
        while(floorY<transform.position.y)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        // cycle through all the enemies
        for (int i=0;i<enemies.Length;i++)
        {
            if(enemies[i]!=null)
            {
                // apply explosion force that applies from our position
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }

        smashing = false;
        
    }
}
