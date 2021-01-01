using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunManager : MonoBehaviour
{
    public float timeBetweenShots;
    public GameObject bullet;
    public GameObject barrel;
    public int groundLayer;
    float timer;
    public bool colliding = false;
    public float correctAngle;
    public AudioSource gunshot;
    
    EnemyMovement movement;
    GameObject enemy;
    GameObject player;

    void Start()
    {
        movement = gameObject.GetComponentInParent<EnemyMovement>();
        enemy = movement.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");

        timer = timeBetweenShots;
    }

    void Update()
    {
        if(gameObject.transform.localRotation.eulerAngles.z > 180)
        {
            correctAngle = gameObject.transform.localRotation.eulerAngles.z - 360;
        } else
        {
            correctAngle = gameObject.transform.localRotation.eulerAngles.z;
        }

        if (Mathf.Abs(correctAngle) > 90)
        {
            gameObject.transform.localScale = new Vector3(1, -1, 1);
        } else if(Mathf.Abs(correctAngle) < 90)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        if(movement.detectionState == "shooting")
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.y - enemy.transform.position.y, player.transform.position.x - enemy.transform.position.x)));
            timer += Time.deltaTime;
        } else
        {
            if(movement.direction == -1 && gameObject.transform.localRotation.eulerAngles.z != 180)
            {
                gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
            } else if(movement.direction == 1 && gameObject.transform.localRotation.eulerAngles.z != 0)
            {
                gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        if (timer >= timeBetweenShots && movement.detectionState == "shooting" && !colliding)
        {
            Shoot();
            timer = 0;
        }
    }

    public void Shoot()
    {
        gunshot.Play();
        Instantiate(bullet, barrel.transform.position, gameObject.transform.localRotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            colliding = false;
        }
    }
}
