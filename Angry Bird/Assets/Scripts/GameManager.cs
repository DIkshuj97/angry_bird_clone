using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public SlingShot slingShot;

    public static GameState gameState;

    public float timesincethrown= Mathf.Infinity;

    int currentBirdIndex;

    private List<GameObject> bricks;
    private List<GameObject> birds;
    private List<GameObject> pigs;

    private void Awake()
    {
        gameState = GameState.Start;

        bricks = new List<GameObject>(GameObject.FindGameObjectsWithTag("Brick"));
        birds = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird"));
        pigs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Pig"));
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Start:
                if (Input.GetMouseButton(0))
                {
                    slingShot.isPressed = true;
                    MoveBirdToSlingshot();
                }
                break;

            case GameState.Playing:
                
                if (slingShot.slingshotState == SlingshotState.BirdFlying && BricksBirdsPigsStoppedMoving())
                {
                    gameState = GameState.BirdMovingToSlingshot;
                    CheckGameState();
                }
                
                break;
        }

        timesincethrown+=Time.deltaTime;

        if(timesincethrown<1.5f)
        {
            cameraFollow.transform.position = Vector3.MoveTowards(cameraFollow.transform.position, cameraFollow.startPos, 10f*Time.deltaTime);

            if(Vector2.Distance(cameraFollow.transform.position,cameraFollow.startPos)<=0f)
            {
                slingShot.isPressed = true;
            }
        }
    }

    private void CheckGameState()
    {
        cameraFollow.isFollowing = false;
        
        if (AllPigsAreDestroyed())
        {
            gameState = GameState.Won;
            Debug.Log("won");
        }
        else if (currentBirdIndex == birds.Count - 1)
        {
            gameState = GameState.Lost;
            Debug.Log("Lost");
        }
        else
        {
            slingShot.slingshotState = SlingshotState.Idle;
            currentBirdIndex++;
            MoveBirdToSlingshot();
            timesincethrown = 0;
        }
    }

    private void MoveBirdToSlingshot()
    {     
        slingShot.birdToThrow = birds[currentBirdIndex];
        cameraFollow.birdToFollow = birds[currentBirdIndex].transform;
        gameState = GameState.Playing;
    }

    bool BricksBirdsPigsStoppedMoving()
    {
        foreach (var item in bricks.Union(birds).Union(pigs))
        {
            if (item != null && item.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > GameVariables.minVelocity)
            {
                return false;
            }
        }
        return true;
    }

    private bool AllPigsAreDestroyed()
    {
        return pigs.All(x => x == null);
    }
}
