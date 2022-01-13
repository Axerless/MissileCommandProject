using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    public float speed = 5f;
    [SerializeField] private int score;
    
    private Animator animator;
    private TrailRenderer trailRenderer;
    private Rigidbody2D rb;
    private GameObject houseHitObj;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "House"){

            houseHitObj = other.gameObject;
            BulletManager.Instance.HouseDestroy(houseHitObj);
            BulletDestroyed();
        }
        if(other.gameObject.tag == "PlayerBullet"){

            BulletManager.Instance.SetScore(score);
            other.gameObject.GetComponent<PlayerBullet>().DestroyBullet();
            BulletDestroyed();
        }
        if(other.gameObject.tag == "Shield"){

            AudioManager.Instance.Play("Shield");
            BulletDestroyed();
        }
        if(other.gameObject.tag == "Ground") StartCoroutine(BulletExplosion());;
    }

    void OnEnable()
    {
        Invoke("EnableTrail",0.05f);
    }

    void EnableTrail()
    {
        trailRenderer.time = 3f;
    }

    public void SetTarget(GameObject houseTarget)
    {
        Vector3 target = houseTarget.transform.position;
        target.z = 0f;

        Vector3 objectPos = transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void FixedUpdate()
    {
        rb.velocity = transform.right * speed;
        if(transform.position.y < -16f) BulletDestroyed();
    }
    IEnumerator BulletExplosion()
    {   
        trailRenderer.time = 0f;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Destroy");
        AudioManager.Instance.Play("BulletDestroy");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
    void BulletDestroyed()
    {
        trailRenderer.time = 0f;
        gameObject.SetActive(false);
    }
}

