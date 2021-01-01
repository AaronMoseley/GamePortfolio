using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TestCell : MonoBehaviour
{
    GameObject[] vcams;
    GameObject gameManager;

    public List<GameObject> attachedCells = new List<GameObject>();
    public bool playerInside;

    bool levelFormed = false;
    bool touchingOther = false;

    private void Start()
    {
        vcams = GameObject.FindGameObjectsWithTag("VCam");
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
    }

    private void Update()
    {
        if(playerInside && !levelFormed)
        {
            FormLevel();
        } else if(playerInside && levelFormed)
        {
            VisibleCells();
        }

        if(playerInside && vcams[0].GetComponent<CinemachineConfiner>().m_BoundingShape2D != gameObject.GetComponent<PolygonCollider2D>())
        {
            ChangeCamera();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            playerInside = true;
        }

        /*bool temp = false;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (collision.gameObject == gameObject.transform.GetChild(i).gameObject)
            {
                temp = true;
            }
        }

        if (collision.gameObject.tag != "Player" && !temp)
        {
            touchingOther = true;
        }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            playerInside = false;

            for (int i = 0; i < attachedCells.Count; i++)
            {   
                if(!attachedCells[i].GetComponent<TestCell>().playerInside)
                {
                    attachedCells[i].SetActive(false);
                }
            }
        }
    }

    void ChangeCamera()
    {
        for (int i = 0; i < vcams.Length; i++)
        {
            if (vcams[i].GetComponent<CinemachineConfiner>())
            {
                vcams[i].GetComponent<CinemachineConfiner>().InvalidatePathCache();
                vcams[i].GetComponent<CinemachineConfiner>().m_BoundingShape2D = gameObject.GetComponent<PolygonCollider2D>();
            }
        }
    }

    void FormLevel()
    {
        levelFormed = true;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.GetComponent<TestExit>() && playerInside)
            {
                if (!gameObject.transform.GetChild(i).gameObject.GetComponent<TestExit>().attachedCell)
                {
                    gameObject.transform.GetChild(i).gameObject.GetComponent<TestExit>().CreateCell(gameManager.GetComponent<TestGen>().cells);
                }
            }
        }
    }

    void VisibleCells()
    {
        for (int i = 0; i < attachedCells.Count; i++)
        {
            attachedCells[i].SetActive(true);
        }
    }

    public bool CheckTouching()
    {
        return touchingOther;
    }
}
