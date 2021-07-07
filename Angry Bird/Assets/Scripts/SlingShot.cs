using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    private Vector3 slingshotMiddleVector;
    private Vector3 dis;

    public SlingshotState slingshotState;
    public Transform leftSlingshotOrigin, rightSlingshotOrigin;

    public LineRenderer slingshotLineRenderer1, slingshotLineRenderer2, trajectoryLineRenderer;

    public GameObject birdToThrow;
    public Transform birdWaitPosition;

    public Vector3 slingOffset;

    public float throwSpeed;

    public bool isPressed;

    void Awake()
    {
        slingshotState = SlingshotState.Idle;

        slingshotLineRenderer1.SetPosition(0, leftSlingshotOrigin.position);
        slingshotLineRenderer2.SetPosition(0, rightSlingshotOrigin.position);

        slingshotMiddleVector = new Vector3((leftSlingshotOrigin.position.x + rightSlingshotOrigin.position.x) / 2,
            (leftSlingshotOrigin.position.y + rightSlingshotOrigin.position.y) / 2, 0);

        SetSlingshotLineRendererActive(false);
        SetTrajectoryLineRendererActive(false);

        GetComponent<CircleCollider2D>().offset = birdWaitPosition.localPosition;
    }

    private void Update()
    {
        if(isPressed)
        {
            InitializeBird();
        }
    }

    private void OnMouseDrag()
    {
        slingshotState = SlingshotState.UserPulling;
        Camera.main.GetComponent<CameraFollow>().isFollowing = true;
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dis = pos - slingshotMiddleVector;
        dis.z = 0;

        if (dis.magnitude > 1.5f)
        {
            dis = dis.normalized * 1.5f;
        }

        birdToThrow.transform.position = dis + slingshotMiddleVector;
        DisplaySlingshotLineRenderers();
        DisplayTrajectoryRenderer(dis.magnitude);
    }

    private void OnMouseUp()
    {
        slingshotState = SlingshotState.BirdFlying;
        ThrowBird(dis.magnitude);
        SetSlingshotLineRendererActive(false);
        SetTrajectoryLineRendererActive(false);
    }

    public void InitializeBird()
    {
        birdToThrow.transform.position = Vector2.MoveTowards(birdToThrow.transform.position, birdWaitPosition.position,Time.deltaTime*2);

        slingshotState = SlingshotState.Idle;

        if(Vector2.Distance(birdToThrow.transform.position,birdWaitPosition.position)<=0f)
        {
            DisplaySlingshotLineRenderers();
            isPressed = false;
        }
        
    }

    private void SetSlingshotLineRendererActive(bool active)
    {
        slingshotLineRenderer1.enabled = active;
        slingshotLineRenderer2.enabled = active;
    }

    void DisplaySlingshotLineRenderers()
    {
        SetSlingshotLineRendererActive(true);
        slingshotLineRenderer1.SetPosition(1, birdToThrow.transform.position+slingOffset);
        slingshotLineRenderer2.SetPosition(1, birdToThrow.transform.position+slingOffset);
    }

    void SetTrajectoryLineRendererActive(bool active)
    {
        trajectoryLineRenderer.enabled = active;
    }

    private void DisplayTrajectoryRenderer(float distance)
    {
        SetTrajectoryLineRendererActive(true);
        Vector3 v2 = slingshotMiddleVector - birdToThrow.transform.position;
        int segmentCount = 15;

        Vector2[] segments = new Vector2[segmentCount];
        segments[0] = birdToThrow.transform.position;

        Vector2 segVelocity = new Vector2(v2.x, v2.y) * throwSpeed * distance;

        for (int i = 0; i < segmentCount; i++)
        {
            float time = i * Time.fixedDeltaTime * 5f;
            segments[i] = segments[0] + segVelocity * time + 0.5f * Physics2D.gravity * Mathf.Pow(time, 2);
        }

        trajectoryLineRenderer.positionCount = segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            trajectoryLineRenderer.SetPosition(i, segments[i]);
        }
    }

    void ThrowBird(float distance)
    {
        Vector3 velocity = slingshotMiddleVector - birdToThrow.transform.position;
        birdToThrow.GetComponent<Bird>().onThrow();
        birdToThrow.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y) * throwSpeed * distance;
    }
}