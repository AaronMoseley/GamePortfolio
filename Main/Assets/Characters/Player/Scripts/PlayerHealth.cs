using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currHealth;

    public GameObject heartsParent;
    public GameObject heart;

    public Sprite heartImage;
    public Sprite heartFlash;
    public Sprite emptyHeart;

    public float flashTime;

    public float deathWaitTime;

    public AudioSource damagedSound;

    List<float> timers = new List<float>();
    List<int> indexes = new List<int>();

    GameObject[] hearts;
    GameObject gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        currHealth = maxHealth;
        hearts = new GameObject[maxHealth];

        for(int i = 0; i < maxHealth; i++)
        {
            hearts[i] = Instantiate(heart, heartsParent.transform);
        }
    }

    void Update()
    {
        for(int i = 0; i < timers.Count; i++)
        {
            timers[i] += Time.deltaTime;

            if(timers[i] >= flashTime)
            {
                hearts[indexes[i]].GetComponent<Image>().sprite = emptyHeart;

                timers.RemoveAt(i);
                indexes.RemoveAt(i);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (currHealth > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                currHealth--;
                FlashHeart(currHealth);
            }
        }

        if(currHealth <= 0)
        {
            gameManager.GetComponent<InGameMenuManager>().PlayerDie();
            gameObject.GetComponent<Movement>().enabled = false;

            if(gameObject.GetComponentInChildren<Grappler>())
            {
                gameObject.GetComponentInChildren<Grappler>().hook.SetActive(false);
            }

            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                if(!gameObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>())
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            StartCoroutine(StopTime());
        }
    }

    public void DamageSound()
    {
        damagedSound.Play();
    }

    public void AddHealth(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            if(i + currHealth < maxHealth)
            {
                hearts[i + currHealth].GetComponent<Image>().sprite = heartImage;
                currHealth++;
            }
        }
    }

    void FlashHeart(int index)
    {
        timers.Add(0);
        indexes.Add(index);

        hearts[index].GetComponent<Image>().sprite = heartFlash;
    }

    IEnumerator StopTime()
    {
        yield return new WaitForSeconds(deathWaitTime);
        Time.timeScale = 0;
    }
}
