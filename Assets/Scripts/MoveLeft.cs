using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private SpawnManager spawnManagerScript;
    private float leftBound = -15;

    // Start is called before the first frame update
    void Start()
    {
        
        spawnManagerScript = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnManagerScript.gameOver == false && spawnManagerScript.gameStarted)
        {
            transform.Translate(Vector3.left * Time.deltaTime * spawnManagerScript.speedCurrent);
        }

        if (transform.position.x < leftBound && gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }

    }
}