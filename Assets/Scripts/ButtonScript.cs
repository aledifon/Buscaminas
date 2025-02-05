using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    // GO Components
    private Button buttonPrefab;

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

       buttonPrefab = GetComponent<Button>();

    }

    #region ButtonMethods
    public void Click()
    {
        IsThereABomb();
    }
    void IsThereABomb()
    {
        // Disable every button when it has been clicked at least once
        buttonPrefab.interactable = false;

        if (bomb)
        {
            
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "*";
            // Set the button image as a Bomb
        }                                
        else
        {            
            // Save the number of bombs around the button
            // Call to the GameManager to check all the cells arond our button
            int numOfBombsAround = GameManager.Gm.CheckBombNumber(x, y);

            // Set the amount of bombs around the clicked button as the button text
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = numOfBombsAround.ToString();
        }                                
    }
    void ChangeColor()
    {

    }
    #endregion
}
