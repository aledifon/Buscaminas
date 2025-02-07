using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    // To know if the button has a Zero bombs or not
    private bool bombsAroundChecked;
    public bool BombsAroundChecked 
    { 
        get {return bombsAroundChecked;}
        set { bombsAroundChecked = value;}
    } 

    // GO Components
    private Button buttonPrefab;
    private Image buttonImage;

    [SerializeField] private Sprite bombImage;

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
       buttonImage = GetComponent<Image>();
    }

    #region ButtonMethods
    public void Click()
    {        
        // If the button has already been clicked then exit the Click method
        if (!buttonPrefab.interactable)
            return;

        // Check if we acces to this method either through an OnClick event or through
        // a method calling from another script
        GameObject objectSender = EventSystem.current?.currentSelectedGameObject;

        IsThereABomb(objectSender);

        //if (!GameManager.Gm.ExplodeAll)
        //    GameManager.Gm.PlaySuccessAudioClip();
    }
    void IsThereABomb(GameObject sender)
    {        
        //if (sender == gameObject)
        //    Debug.Log("M�todo Click llamado desde el bot�n en la UI.");
        //else
        //    Debug.Log("M�todo Click llamado desde otro script.");

        // Disable the button when has been clicked once
        buttonPrefab.interactable = false;

        // Avoid finding a bomb in the 1st click
        // (Give a chance to the player)
        if(GameManager.Gm.IsFirstClick)
        {            
            GameManager.Gm.IsFirstClick = false;
            if (bomb)
                bomb = false;
        }            

        // The bomb will only be set in case we arrive here through a OnClick Event
        if (bomb && (sender == gameObject || GameManager.Gm.Die))
        {
            SetBombText();
            ChangeColorText(6);
            SetBombImage();
            GameManager.Gm.ExplodeMap();

            // Show the Sad Emoji        
            GameManager.Gm.SetSadEmoji();
            // Play the Fail Audio clip
            GameManager.Gm.PlayFailAudioClip();
        }
        else
        {
            // Save the number of bombs around the button
            // Call to the GameManager to check all the cells arond our button
            int numOfBombsAround = GameManager.Gm.CheckBombNumber(x, y);

            // Set the amount of bombs around the clicked button as the button text            
            SetBombsAroundText(numOfBombsAround);

            // Update the number of Remaining Cells for being clicked
            GameManager.Gm.RemainingCells--;
            Debug.Log("A total of " + GameManager.Gm.RemainingCells + " pending cells out of " +
                       (GameManager.Gm.Width * GameManager.Gm.Height - GameManager.Gm.BombsAmount) + 
                       " cells" );
        }        
    }
    public void SetBombImage()
    {
        buttonImage.color = Color.red;
        buttonImage.sprite = bombImage;

        ColorBlock colorBlock = new ColorBlock();
        colorBlock = buttonPrefab.colors;
        colorBlock.disabledColor = new Color(
                                    colorBlock.disabledColor.r,
                                    colorBlock.disabledColor.g,
                                    colorBlock.disabledColor.b,
                                    1f);
        buttonPrefab.colors = colorBlock;
    }
    public void SetBombText()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "*";
        // Set the button image as a Bomb
    }
    public void SetBombsAroundText(int bombsAround)
    {
        // Set the amount of bombs around the clicked button as the button text            
        if (bombsAround > 0)
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bombsAround.ToString();
            ChangeColorText(bombsAround);
            // Update the Emoji to 'Ups' expression
            GameManager.Gm.SetUpsEmoji();
            // Play the Win Audio Clip
            GameManager.Gm.PlaySuccessAudioClip();
        }        
            
        // If the button has no bombs around then we'll check all the bombs around this one.
        else
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";            
            GameManager.Gm.ClickAround(x, y);                       
            // Update the Emoji to 'Glasses' expression            
            GameManager.Gm.SetGlassesEmoji();
            // Play the Win Audio Clip
            GameManager.Gm.PlayWinAudioClip();
        }
    }

    void ChangeColorText(int num)
    {
        switch (num)
        {
            case 1:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.blue;
                break;
            case 2:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
                break;
            case 3:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                break;
            case 4:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.cyan;
                break;
            case 5:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                break;
            case 6:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                break;
            case 7:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
                break;
            case 8:
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.grey;
                break;
            default:
                break;
        }
    }
    #endregion
}
