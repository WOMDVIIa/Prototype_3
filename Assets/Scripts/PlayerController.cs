using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 3.0f;
    public bool isOnGround = true;
    public bool jumpedOnce = false;
    
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

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
        scoreText = GameObject.Find("Score Text").GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("HighScore Text").GetComponent<TextMeshProUGUI>();
        scoreText.text = "Score: 0";
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
            scoreText.text = "Score: " + Mathf.Round(score);
            //string scoreOutput = "Score = " + Mathf.Round(score);
            //Debug.Log(scoreOutput);
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
                playerAnim.SetBool("Death_b", true);
                dirtParticle.Stop();
                spawnManagerScript.CallWaitingForRestart();
            }
            spawnManagerScript.gameOver = true;
            //playerAnim.SetBool("Death_b", true);
            //dirtParticle.Stop();
            //spawnManagerScript.CallWaitingForRestart();
            if (score > spawnManagerScript.highScore)
            {
                spawnManagerScript.highScore = Mathf.Round(score);
                highScoreText.text = "High Score: " + spawnManagerScript.highScore;
            }
        }
    }
}
