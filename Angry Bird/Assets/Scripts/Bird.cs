using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public BirdState birdState { get; set; }

    private TrailRenderer lineRenderer;
    private Rigidbody2D myBody;
    private CircleCollider2D myCollider;
    private AudioSource audioSource;

    private void Awake()
    {
        InitializeVaraible();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(birdState==BirdState.Thrown && myBody.velocity.magnitude<=GameVariables.minVelocity)
        {
            StartCoroutine(DestroyAfterDelay(2f));
        }
    }

    void InitializeVaraible()
    {
        lineRenderer = GetComponent<TrailRenderer>();
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();

        lineRenderer.enabled = false;
        lineRenderer.sortingLayerName = "Foreground";

        myBody.isKinematic = true;
        myCollider.radius = GameVariables.BirdColliderRadiusBig;

        birdState = BirdState.BeforeThrown;
    }

    public void onThrow()
    {
        audioSource.Play();
        lineRenderer.enabled = true;
        myBody.isKinematic = false;
        
        myCollider.radius = GameVariables.BirdColliderRadiusNormal;
        birdState = BirdState.Thrown;
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
