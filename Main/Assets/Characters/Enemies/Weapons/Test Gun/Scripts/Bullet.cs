using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float timeAlive;
    public int damageAmt;
    public int hookLayer;
    public AudioSource impactSound;

    GameObject player;
    float timer;
    Rigidbody2D rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= timeAlive)
        {
            Destroy(gameObject);
        }

        rb.velocity = new Vector2(bulletSpeed * Mathf.Cos(Mathf.Deg2Rad * gameObject.transform.localEulerAngles.z), bulletSpeed * Mathf.Sin(Mathf.Deg2Rad * gameObject.transform.localEulerAngles.z));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player.GetComponent<PlayerHealth>().TakeDamage(damageAmt);
            player.GetComponent<PlayerHealth>().DamageSound();
            Destroy(gameObject);
        }

        if (!collision.collider.isTrigger && collision.gameObject.layer != hookLayer)
        {
            StartCoroutine(BulletHit());
        }
    }

    IEnumerator BulletHit()
    {
        impactSound.Play();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitWhile(() => impactSound.isPlaying);
        Destroy(gameObject);
    }
}
