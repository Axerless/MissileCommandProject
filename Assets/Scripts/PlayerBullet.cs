using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{   
    public float bulletVelocity = 5f;

    private Animator animator;
    private TrailRenderer trailRenderer;
    private Rigidbody2D rb;
    private Vector3 mouseTarget;
    private bool isDestroyed;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Upgrade"){

            Upgrades upgrades = other.GetComponent<Upgrades>();
            if(upgrades != null) upgrades.SetUpgrade();
        }
        else if(other.gameObject.tag == "Ground") DestroyBullet();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void OnEnable()
    {
        Invoke("EnableTrail",0.05f);
        isDestroyed = false;
    }

    void EnableTrail()
    {
        trailRenderer.time = 1f;
    }

    public void SetTarget(Vector3 bulletTarget)
    {
        mouseTarget = bulletTarget;

        Vector3 target = bulletTarget;
        target.z = 0f;

        Vector3 objectPos = transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void FixedUpdate()
    {
        if(!isDestroyed) rb.velocity = transform.right * bulletVelocity;

        if(Vector2.Distance(transform.position, mouseTarget) <= 0.1f) DestroyBullet();
    }

    public void DestroyBullet()
    {
        if(!isDestroyed) StartCoroutine(Destruction(1.45f));
    }

    IEnumerator Destruction(float timeToDestroy)
    {
        AudioManager.Instance.Play("Explosion");
        rb.velocity = Vector2.zero;
        isDestroyed = true;
        trailRenderer.time = 0f;
        animator.SetTrigger("Destroy");
        yield return new WaitForSeconds(timeToDestroy);
        gameObject.SetActive(false);
    }
}

