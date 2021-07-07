using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 startPos;
    public bool isFollowing;
    public Transform birdToFollow;

    private float minCameraX = 0f, maxCameraX = 14f;

    private void Awake()
    {
        startPos = transform.position;
    }


    void Update()
    {
        if(isFollowing)
        {
            if(birdToFollow!=null)
            {
                var birdPosition = birdToFollow.position;
                float x = Mathf.Clamp(birdPosition.x, minCameraX, maxCameraX);
                transform.position = new Vector3(x, startPos.y, startPos.z);
            }

            else
            {
                isFollowing = false;
            }
        }
    }
}
