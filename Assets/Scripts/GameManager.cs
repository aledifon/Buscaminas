using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // Static var which contains the single instance of the Singleton
    private static GameManager gm;
    // Static property used to access to the Singleton
    public static GameManager Gm
    {
        // Trying to access to the GameManager singleton through its property Gm.
        get
        {
            // In case doesn't exist already an instance of the GameManager script
            if (gm == null)
            {
                // In case there is already a GO of type GameManager then assign it
                // to our Singleton instance
                gm = FindAnyObjectByType<GameManager>();

                // In case there is no any GO of type GameManager then we'll create it,
                // then we add it a GameManager Component and then we assign it to our
                // Singleton instance gm.
                if (gm == null)
                {
                    GameObject go = new GameObject("GameManager");
                    gm = go.AddComponent<GameManager>();
                }
            }
            return gm;
        }
    }

    #region UIVars
    [SerializeField] private GameObject gameButton;
    [SerializeField] private GameObject gamePanel;
    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 spacing;    
        
    [SerializeField] private int width;
    public int Width {get { return width;}}
    [SerializeField] private int height;
    public int Height { get { return height; } }
    #endregion 

    // Amount of bombs on the board
    private int bombsAmount;

    // Bidimensional array which contain all the Cells Info
    private ButtonScript[,] map;

    //////////////////////////////////////////////////////////    
    // NOT USED FOR CLASSES INHERITED FROM MONOBEHAVIOUR /////
    //////////////////////////////////////////////////////////

    // Private constructor to avoid the instances creation with "new"
    //private GameManager()  { Debug.Log("GameManager created"); }

    // In case There will be any GameManager script already joint to any GO on the Project
    void Awake()
    {   
        // If doesn't exist already an instance of the GameManager script
        // then saves the current instance on the singleton var.
        if (gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject); // Avoids the GO will be destroyed when the Scene changes
        }            
        // Otherwise, if does already exists an instance of the GameManager script then
        // we'll destroy the GO is attached to
        else
            Destroy(gameObject);

        // Setup the Grid Layout component        
        GridLayoutSetup();

        rectTransform = gamePanel.GetComponent<RectTransform>();

        // Init the bidiomensional array
        map = new ButtonScript[width, height];

        // Generate all the buttons and place them on the Grid Layout
        GenerateButtons();

        CreateBombs();
    }

    void GridLayoutSetup()
    {
        gridLayout = gamePanel.GetComponent<GridLayoutGroup>();

        gridLayout.cellSize = new Vector2(cellSize.x, cellSize.y);
        gridLayout.spacing = new Vector2(spacing.x, spacing.y); 
        
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = width;
    }

    private void GenerateButtons()
    {
        for (int i = 0; i < width * height; i++)
        {
            // Calculated on the Awake Method of Button Script
            // Calculate of coordinates x(columnId) and y(rowId) of every cell            
            int x = i / height; // Rest of division 
            int y = i % height; // Quotient of division

            // 1. Instantiate every Button prefab (GO) into the GamePanel of the Canvas
            // 2. Assign the ButtonScript component of every instanced Button to the array on the [x,y] pos.
            map[x,y] = Instantiate(gameButton, gamePanel.transform).GetComponent<ButtonScript>();
            map[x, y].x = x;
            map[x, y].y = y;

            //Debug.Log("The current cell Id is " + map[x,y].CellId + 
            //        " || The total of cells are " + ButtonScript.NumOfCells +
            //        " || The Coordinates are [" + map[x, y].x + "," + map[x, y].y + "]");
        }            
    }

    #region BombsMethods
    public int CheckBombNumber(int x, int y) 
    {
        int counter = 0;

        // Check if there are bombs on the neighbour cells (left,right,up,down and 4 diagonals)        
        //Left Cells
        if (x > 0)
        {
            if (map[x - 1, y].bomb)                         // Left
                counter++;
            if (y < (height - 1) && map[x - 1, y + 1].bomb) // Left-Up
                    counter++;
            if (y > 0 && map[x - 1, y - 1].bomb)            // Left-Down
                    counter++;           
        }
        // Right Cells
        if (x < (width - 1))
        {
            if (map[x+1, y].bomb)                           // Right
                counter++;
            if (y < (height - 1) && map[x + 1, y + 1].bomb) // Right-up
                    counter++;
            if (y > 0 && map[x + 1, y - 1].bomb)            // Right-Down
                    counter++;
        }
        // Up Cell
        if (y < (height - 1) && map[x, y + 1].bomb)             
                counter++;
        // Down Cell
        if (y > 0 && map[x, y - 1].bomb)             
                counter++;

        return counter;
    }    
    private void NumbersOfBombs()
    {
        // Set the Range between a 20-40% of the Min and Max of Bombs
        bombsAmount = Random.Range((int)(10 * 0.35f), (int)(width * height * 0.4f));

        //// The board has only 10 buttons then it generates only 1 bomb
        //if (width*height <= 10)        
        //    bombsAmount = 1;
        //// The board has between 10-30 buttons then it generates 7 bombs
        //else if (width * height > 10 && width * height < 30 )
        //    bombsAmount = 7;        
    }
    void CreateBombs()
    {
        NumbersOfBombs();        

        for (int i=0;i<bombsAmount;i++)
        {
            int x=0,y=0;

            do
            {
                x = Random.Range(0, width-1);
                y = Random.Range(0, height-1);
            } while (map[x, y].bomb);
            
            map[x, y].bomb = true;

            //Debug.Log("Bomb assigned on [" + map[x, y].x + "," + map[x, y].y + "]" +
            //                "|| Still are pending " + (bombsAmount-(i+1)) + " bombs");
        }
    }
    #endregion
}
