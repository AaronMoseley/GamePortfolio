using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TestExit : MonoBehaviour
{
    //1 is right, -1 is left, 2 is up, -2 is down
    public int type;

    //size of the exit
    public int length;

    int nextCellExit = 0;

    public GameObject attachedCell;

    void Start()
    {

    }

    
    void Update()
    {
        
    }

    public void CreateCell(GameObject[] cells)
    {
        if (!attachedCell)
        {
            int num = -1;

            bool reset = false;

            while (!reset)
            {
                reset = true;

                int rand = Random.Range(0, cells.Length);

                for (int i = 0; i < cells[rand].transform.childCount; i++)
                {
                    if (cells[rand].transform.GetChild(i).gameObject.GetComponent<TestExit>())
                    {
                        if (cells[rand].transform.GetChild(i).gameObject.GetComponent<TestExit>().type == type * -1 && cells[rand].transform.GetChild(i).gameObject.GetComponent<TestExit>().length >= length - 2 && cells[rand].transform.GetChild(i).gameObject.GetComponent<TestExit>().length <= length + 2)
                        {
                            num = rand;
                            nextCellExit = i;
                        }
                        else if (i == cells[rand].transform.childCount - 1 && num != rand)
                        {
                            reset = false;
                        }
                    }
                }
            }

            GameObject cellInst = Instantiate(cells[num], gameObject.transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
            GameObject exit = cellInst.transform.GetChild(nextCellExit).gameObject;

            exit.transform.SetParent(gameObject.transform);
            cellInst.transform.SetParent(exit.transform);
            exit.transform.localPosition = new Vector2(0, 0);
            cellInst.transform.SetParent(null);
            exit.transform.SetParent(cellInst.transform);

            gameObject.transform.parent.GetComponent<TestCell>().attachedCells.Add(cellInst);

            attachedCell = cellInst;
            cellInst.GetComponent<TestCell>().attachedCells.Add(gameObject.transform.parent.gameObject);
            exit.GetComponent<TestExit>().attachedCell = gameObject;

            if(cellInst.GetComponent<TestCell>().CheckTouching())
            {
                Destroy(cellInst);
                CreateCell(cells);
            } else
            {
                cellInst.GetComponentInChildren<TilemapRenderer>().enabled = true;
            }
        }
    }
}
