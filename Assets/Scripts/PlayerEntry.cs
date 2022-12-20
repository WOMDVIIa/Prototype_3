using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntry : MonoBehaviour
{
    private float walkingSpeed = 3.0f;
    private float positionToStop = 0.0f;
    private Animator playerAnim;
    private SpawnManager spawnManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        spawnManagerScript = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!spawnManagerScript.gameStarted)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * walkingSpeed);
            if (transform.position.x > positionToStop)
            {
                playerAnim.SetFloat("Speed_f", 0.7f);
                spawnManagerScript.gameStarted = true;
            }
        }
    }
}
