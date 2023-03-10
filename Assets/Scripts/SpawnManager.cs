using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Audio;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool playerDashing = false;
    public float gravityModifier;
    public float speedNominal = 12.5f;
    public float speedDash = 25.0f;
    public float speedCurrent;
    public float highScore = 0;

    public GameObject[] obstaclePrefab;
    public GameObject playerPrefab;
    public GameObject[] activePrefabs;
    public TextMeshProUGUI restartText;
    public Image soundOn;
    public Image soundOff;

    private Vector3 spawnPosPlayer = new Vector3(-9, 1.0f, 0);
    private Vector3 spawnPos = new Vector3(30, 0, 0);
    private float startDelay = 4;
    private float repeatDelay;
    private float repeatDelayMin = 1.2f;
    private float repeatDelayMax = 3.0f;
    private int activePrefabsIndex = 1;
    private int maxActivePrefabs = 5;
    private bool gameRestartReady = false;
    private bool isSoundOn = true;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity *= gravityModifier;
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDashing)
        {
            speedCurrent = speedDash;
        }
        else
        {
            speedCurrent = speedNominal;
        }

        Restart();
        ToggleSounds();
    }


    private void SpawnPlayer()
    {
        activePrefabs[0] = Instantiate(playerPrefab, spawnPosPlayer, playerPrefab.transform.rotation);
        gameOver = false;
        gameStarted = false;
        Invoke("SpawnObstacle", startDelay);
        speedCurrent = speedNominal;
    }

    private void SpawnObstacle()
    {
        if (!gameOver)
        {
            int obstacleIndex = Random.Range(0, obstaclePrefab.Length);
            if (activePrefabsIndex < maxActivePrefabs)
            {
                activePrefabs[activePrefabsIndex] = Instantiate(obstaclePrefab[obstacleIndex], spawnPos, obstaclePrefab[0].transform.rotation);
                activePrefabsIndex++;
            }
            else
            {
                activePrefabs[1] = Instantiate(obstaclePrefab[obstacleIndex], spawnPos, obstaclePrefab[0].transform.rotation);
                activePrefabsIndex = 2;
            }

            repeatDelay = Random.Range(repeatDelayMin, repeatDelayMax);
            Invoke("SpawnObstacle", repeatDelay);
        }
    }
    private void Restart()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R) && gameRestartReady)
        {
            gameRestartReady = false;

            for (int i = 0; i < maxActivePrefabs; i++)
            {
                if (activePrefabs[i] != null)
                {
                    Destroy(activePrefabs[i]);
                }                
            }
            restartText.gameObject.SetActive(false);
            SpawnPlayer();
        }
    }

    private void ToggleSounds()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSoundOn)
            {
                soundOn.gameObject.SetActive(false);
                soundOff.gameObject.SetActive(true);
                isSoundOn = false;
                AudioListener.volume = 0;
            }
            else
            {
                soundOn.gameObject.SetActive(true);
                soundOff.gameObject.SetActive(false);
                isSoundOn = true;
                AudioListener.volume = 1;
            }
        }
    }

    public void CallWaitingForRestart()
    {
        StartCoroutine(WaitUntillRestartReady());
    }

    IEnumerator WaitUntillRestartReady()
    {
        yield return new WaitForSeconds(repeatDelayMax);
        gameRestartReady = true;
        restartText.gameObject.SetActive(true);
    }

}
