using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private static int numOfCells;
    public static int NumOfCells {  get { return numOfCells; } } 

    private int cellId;
    public int CellId {  get { return cellId; } }

    // Know the button positions
    public int x;
    public int y;

    // To know if the button has a bomb or not
    [SerializeField] public bool bomb;
    private void Awake()
    {
        numOfCells++;
        cellId = numOfCells - 1;

        // CALCULATED ON THE GameManager.cs instead
        // Calculate of coordinates x(columnId) and y(rowId) of every cell                    
        //x = numOfCells / GameManager.Gm.Height;         // Rest of division 
        //y = numOfCells % GameManager.Gm.Height;         // Quotient of division

        //Debug.Log("The current cell Id is " + cellId + " || The total of cells are " + numOfCells + 
        //            " || The Coordinates are [" + x + "," + y +"]");        
    }

    #region ButtonMethods
    public void Click()
    {

    }
    void IsThereABomb()
    {
        if (bomb)
        {
            //Game Over
        }
        else
        {
            // Call to the GameManager to check the rest of cells
            GameManager.Gm.CheckBombNumber(x,y);
        }
    }
    void ChangeColor()
    {

    }

    #endregion
}
