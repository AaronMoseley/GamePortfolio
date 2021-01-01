using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CellGen : MonoBehaviour
{   
    //What we instantiate in the cells
    public GameObject[] otherCells;
    public GameObject[] mainCells;
    public GameObject[] startingCells;
    public GameObject[] endCells;
    public GameObject wallCell;

    //Info about the size of the level
    public int rows;
    public int columns;
    public int cellSize;

    //Allows it to switch rows as the cells are generated
    int amoInRow = 0;

    //Positions of the cells
    float xPos;
    float yPos;

    //Stores info for what type of cell to generate in a grid
    int[][] rowInfo;

    //Amount of main-path cells in each row
    int mainAmo;
    int lastAmo;

    //Where the main path starts in each row, starts at -1 because that can't be used in an arry
    int lastStart = -1;

    //List of already generated cells
    List<GameObject> generatedCells = new List<GameObject>();

    void Awake()
    {
        rowInfo = new int[rows][];

        //Generates the walls of the level
        WallGen();

        for(int i = 0; i < rowInfo.Length; i++)
        {
            //Sets up each of the members of rowInfo
            rowInfo[i] = new int[columns];

            for(int j = 0; j < rowInfo[i].Length; j++)
            {
                rowInfo[i][j] = 0;
            }

            //Determines where the main path will start on each row
            int mainStart = 0;

            if (i == 0)
            { 
                //For the first Row
                mainStart = Random.Range(0, columns);

                rowInfo[0][mainStart] = 2;
            } else
            {
                //For every other row, generates a random number until the cell above it is part of the main path
                bool reset = false;

                while (!reset)
                {
                    reset = true;

                    mainStart = Random.Range(1, columns);

                    if (rowInfo[i - 1][mainStart] != 1 && rowInfo[i - 1][mainStart] != 2)
                    {
                        reset = false;
                    } else if(mainStart == lastStart && lastAmo != 1)
                    {
                        reset = false;
                    }
                }
            }

            lastStart = mainStart;

            //Determines the amount of main path cells that will be in each row, can't be equal to the last amount if there are more than 2 columns
            mainAmo = Random.Range(1, (columns / 2) + 1);

            bool reset1 = false;

            while(!reset1)
            {
                reset1 = true;

                if(mainAmo == lastAmo && columns > 3)
                {
                    mainAmo = Random.Range(1, (columns / 2) + 1);
                    reset1 = false;
                }
            }

            lastAmo = mainAmo;

            //Determines the direction that the main path will go in, 0 for left, 1 for right
            int dir = Random.Range(0, 2);

            int offset = 0;

            for(int j = 0; j < mainAmo; j++)
            {
                //Makes sure that the changes skip past the beginning and ending of the path
                if(rowInfo[i][j] == 2)
                {
                    offset++;
                } else if(i == rows - 1 && rowInfo[i][mainStart] == 0)
                {
                    rowInfo[i][mainStart] = 3;
                    offset++;
                }
                
                //Changes direction if it's about to run over or under the limit
                if(mainStart + offset > rowInfo[i].Length - 1)
                {
                    dir = 0;
                    offset = 1;
                } else if(mainStart - offset < 0)
                {
                    dir = 1;
                    offset = 1;
                }

                //Sets the next cell in the direction to part of the main path
                if(dir == 1 && rowInfo[i][mainStart + offset] != 2)
                {
                    rowInfo[i][mainStart + offset] = 1;
                    offset++;
                } else if(dir == 0 && rowInfo[i][mainStart - offset] != 2)
                {
                    rowInfo[i][mainStart - offset] = 1;
                    offset++;
                } else
                {
                    offset++;
                }
            }
        }

        //A debug statement that outputs the main path with start and end points
        /*for(int i = 0; i < rowInfo.Length; i++)
        {
            string test = "";

            for(int j = 0; j < rowInfo[0].Length; j++)
            {
                test += rowInfo[i][j].ToString();
            }

            Debug.Log(test);
        }*/

        //Calls the generation functions depending on what type of cell everything is
        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                if(rowInfo[i][j] == 1)
                {
                    MainGenerate(i, j, mainCells);
                } else if(rowInfo[i][j] == 2)
                {
                    MainGenerate(i, j, startingCells);
                } else if(rowInfo[i][j] == 3)
                {
                    MainGenerate(i, j, endCells);
                } else
                {
                    MiscGenerate();
                }
            }
        }

        //Reloads the scene if there's a direct path from top to bottom with no obstacles
        for (int i = 0; i < columns; i++)
        {
            bool clear = true;
            int cell = i;

            for(int row = 0; row < rows; row++)
            {
                if(!generatedCells[cell].GetComponent<CellInfo>().open[0] && row != 0)
                {
                    clear = false;
                } else if(!generatedCells[cell].GetComponent<CellInfo>().open[1] && row != rows - 1)
                {
                    clear = false;
                }

                cell += columns;
            }

            if(clear && rows > 3 && columns > 3)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
        }
    }

    //Generates the walls of the level, all of them moved in by 1 unit so there aren't any paths leading into the walls
    public void WallGen()
    {
        for (int i = 0; i < columns; i++)
        {
            if (i == 0)
            {
                //Generates the left wall of the level
                int xWallPos = -(cellSize - 1);

                int yWallPos;

                for (yWallPos = cellSize; yWallPos > (-1 * rows * cellSize) - (cellSize - 1); yWallPos -= cellSize)
                {
                    Instantiate(wallCell, new Vector3(xWallPos, yWallPos, 0), Quaternion.Euler(0, 0, 0));
                }
            }
            else if (i == columns - 1)
            {
                //Generates the right wall of the level
                int xWallPos = (columns * cellSize) - 1;
                int yWallPos;

                for (yWallPos = cellSize; yWallPos > (-1 * rows * cellSize) - (cellSize - 1); yWallPos -= cellSize)
                {
                    Instantiate(wallCell, new Vector3(xWallPos, yWallPos, 0), Quaternion.Euler(0, 0, 0));
                }
            }
        }

        //Generates the top and bottom walls, y1WallPos is the top, y2 is bottom
        int xOtherWallPos;

        int y1WallPos = cellSize - 1;

        int y2WallPos = -1 * (rows * cellSize) + 1;

        for (xOtherWallPos = 0; xOtherWallPos < columns * cellSize; xOtherWallPos += cellSize)
        {
            Instantiate(wallCell, new Vector3(xOtherWallPos, y1WallPos, 0), Quaternion.Euler(0, 0, 0));

            Instantiate(wallCell, new Vector3(xOtherWallPos, y2WallPos, 0), Quaternion.Euler(0, 0, 0));
        }
    }

    int SelectMainCell(int row, int column, GameObject[] cells)
    {
        //Sets the inital requirements for main cells, if they are connected in any way to other main cells, they have to be open on that side
        bool[] needMainOpen = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            needMainOpen[i] = false;
        }

        //Left
        if (column - 1 >= 0)
        {
            if (rowInfo[row][column - 1] == 1 || rowInfo[row][column - 1] == 3)
            {
                needMainOpen[2] = true;
            }
        }

        //Right
        if (column + 1 <= columns - 1)
        {
            if (rowInfo[row][column + 1] == 1 || rowInfo[row][column + 1] == 2 || rowInfo[row][column + 1] == 3)
            {
                needMainOpen[3] = true;
            }
        }

        //Top
        if (row - 1 >= 0)
        {
            if (rowInfo[row - 1][column] == 1 || rowInfo[row - 1][column] == 2)
            {
                needMainOpen[0] = true;
            }
        }

        //Bottom
        if (row + 1 <= rows - 1)
        {
            if (rowInfo[row + 1][column] == 1 || rowInfo[row + 1][column] == 2 || rowInfo[row + 1][column] == 3)
            {
                needMainOpen[1] = true;
            }
        }

        //Puts the requirements through again and generates the number of the cell to generate
        int cellNum = SelectCell(cells, needMainOpen);

        return cellNum;
    }

    int SelectCell(GameObject[] cells, bool[] open)
    {
        //Selects the number of the cell to generate based on the CellInfo script
        
        //If the cell to the left is open to the right, this one needs to be open to the left
        if (generatedCells.ToArray().Length - 1 >= 0 && !open[2])
        {
            if (generatedCells[generatedCells.ToArray().Length - 1].GetComponent<CellInfo>().open[3])
            {
                open[2] = true;
            }
        }

        //If the cell above is open to the bottom, this one needs to be open to the top
        if (generatedCells.ToArray().Length - columns >= 0 && !open[0])
        {
            if (generatedCells[generatedCells.ToArray().Length - columns].GetComponent<CellInfo>().open[1])
            {
                open[0] = true;
            }
        }

        int cellNum = 0;
        bool reset = false;

        //Decides the number of the cell based on the boolean array of requirements 
        while (!reset)
        {
            reset = true;

            cellNum = Random.Range(0, cells.Length);

            for (int i = 0; i < 4; i++)
            {
                if (!cells[cellNum].GetComponent<CellInfo>().open[i] && open[i])
                {
                    reset = false;
                }
            }
        }

        return cellNum;
    }

    void MainGenerate(int i, int j, GameObject[] cells)
    {   
        //Generates a cell in the main path
        
        //Decides the number of the cell to generate
        int cellNum = SelectMainCell(i, j, cells);

        //Generates the cell at the xPos and yPos variables
        GameObject a = Instantiate(cells[cellNum], new Vector3(xPos, yPos, 0), Quaternion.Euler(0, 0, 0)) as GameObject;

        //Adds the new cell to the list of generated cells and changes the position variables as well as amount in row to reflect the addition
        generatedCells.Add(a);

        amoInRow++;

        if (amoInRow >= columns)
        {
            yPos -= cellSize;
            xPos = 0;
            amoInRow = 0;
        }
        else
        {
            xPos += cellSize;
        }
    }

    void MiscGenerate ()
    {
        //Generates a cell outside of the main path
        
        //Determines the requirements for this cell
        bool[] needMiscOpen = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            needMiscOpen[i] = false;
        }

        //Generates the cell number
        int cellNum = SelectCell(otherCells, needMiscOpen);

        //Generates the cell
        GameObject a = Instantiate(otherCells[cellNum], new Vector3(xPos, yPos, 0), Quaternion.Euler(0, 0, 0)) as GameObject;

        //Adds the new cell to the list of generated cells and changes the position variables to reflect the change
        generatedCells.Add(a);

        amoInRow++;

        if (amoInRow >= columns)
        {
            yPos -= cellSize;
            xPos = 0;
            amoInRow = 0;
        } else
        {
            xPos += cellSize;
        }
    }
}