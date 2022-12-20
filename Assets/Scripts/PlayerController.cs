using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 3.0f;
    public bool isOnGround = true;
    public bool jumpedOnce = false;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;

    private float score = 0;
    private float scoreTimeModifier = 10.0f;
    private float scoreDashModifier = 3.0f;
    private SpawnManager spawnManagerScript;
    private MoveLeft playerDash;
    private Rigidbody playerRb;
    private Animator playerAnim;
    private AudioSource playerAudio;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        spawnManagerScript = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        // do sprawdzenia
        //playerDash = GameObject.Find("Background").GetComponent<MoveLeft>();
    }

    // Update is called once per frame
    void Update()
    {
        Jumping();
        Dashing();
        Score();
    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !spawnManagerScript.gameOver && spawnManagerScript.gameStarted)
        {
            dirtParticle.startDelay = 0;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            playerAnim.SetTrigger("Jump_trig");
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            jumpedOnce = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && jumpedOnce)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            jumpedOnce = false;
        }
    }

    private void Dashing()
    {
        if (Input.GetKey(KeyCode.Tab) && isOnGround && !spawnManagerScript.gameOver && spawnManagerScript.gameStarted)
        {
            playerAnim.speed = 2;
            spawnManagerScript.playerDashing = true;
        }
        else
        {
            playerAnim.speed = 1;
            spawnManagerScript.playerDashing = false;
        }
    }

    private void Score()
    {
        if (!spawnManagerScript.gameOver && spawnManagerScript.gameStarted)
        {
            if (spawnManagerScript.playerDashing)
            {
                score += Time.deltaTime * scoreDashModifier * scoreTimeModifier;
            }
            else
            {
                score += Time.deltaTime * scoreTimeModifier;
            }
            string scoreOutput = "Score = " + Mathf.Round(score);
            Debug.Log(scoreOutput);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && spawnManagerScript.gameStarted)
        {
            //playerAnim.SetBool("Jump_b", false);
            isOnGround = true;
            jumpedOnce = false;
            if (!spawnManagerScript.gameOver)
            {
                dirtParticle.Play();
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!spawnManagerScript.gameOver)
            {
                explosionParticle.Play();
                playerAudio.PlayOneShot(crashSound, 1.0f);
            }
            spawnManagerScript.gameOver = true;
            string gameOverLog = "Game Over! You scored " + Mathf.Round(score) + " points!";
            Debug.Log(gameOverLog);
            playerAnim.SetBool("Death_b", true);
            dirtParticle.Stop();
        }
    }
}
