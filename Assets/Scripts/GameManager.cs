using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static UnityEditor.ShaderData;

public class GameManager : MonoBehaviour
{
    #region Vars
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

    #region Enums
    private enum LevelSelected { None = -1, Easy, Normal, Hard}
    [SerializeField] private LevelSelected levelSelected = LevelSelected.None;
    private enum PanelSelected {Title, Menu, Options, LevelSelection, Game, Pause, Win}
    [SerializeField] private PanelSelected panelSelected = PanelSelected.Title;
    #endregion

    #region UIVars
    [Header("UIMenuButtons")]
    [SerializeField] private MenuSelectButton menuPanelStartButton;
    [SerializeField] private MenuSelectButton menuPanelOptionsButton;
    [SerializeField] private SelectLevelButton selectLevelEasyButton;
    [SerializeField] private SelectLevelButton selectLevelNormalButton;
    [SerializeField] private SelectLevelButton selectLevelHardButton;
    [Header ("UI")]
    [SerializeField] private GameObject gameButton;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject gamePanel;    
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pauseLevelPanel;
    [SerializeField] private GameObject winPanel;
    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 spacing;

    [SerializeField] private int width;
    //public int Width { get { return width; } }

    [SerializeField] private int height;
    //public int Height { get { return height; } }    
    #endregion 

    [Header ("Bombs")]

    // Amount of bombs on the board
    [SerializeField] private int bombsAmount;
    public int BombsAmount => bombsAmount;

    private int remainingCells;
    public int RemainingCells { get => remainingCells; }

    // Death Flag
    private bool isDied;
    public bool IsDied { get { return isDied; } }

    // Bidimensional array which contain all the Cells Info
    private ButtonScript[,] map;

    [Header("Emoji Images")]
    public Image imageEmoji;
    public Sprite[] images;
    
    // Emoji Update State Timer
    private float elapsedEmojiTime;
    [SerializeField] private float emojiMaxTime = 1f;

    [Header("Fx Audio")]
    [SerializeField] private AudioClip failAudioclip;
    [SerializeField] private AudioClip successAudioclip;
    [SerializeField] private AudioClip greatSuccessAudioclip;
    [SerializeField] private AudioClip winAudioclip;
    [SerializeField] private AudioClip restartAudioclip;
    [SerializeField] private AudioClip rightClickAudioclip;
    [SerializeField] private AudioSource gameFXAudioSource;

    [Header("Menu Audio")]
    [SerializeField] private AudioClip gameAudioclip;
    [SerializeField] private AudioClip pauseAudioclip;
    [SerializeField] private AudioClip titleAudioclip;
    [SerializeField] private AudioClip selectLevelAudioclip;
    [SerializeField] private AudioClip selectFXAudioclip;

    private AudioSource audioSource;
    public bool gameAudioFxIsPlaying => gameFXAudioSource != null && gameFXAudioSource.isPlaying;
    //public bool audiouSourceIsPlaying { get { return audioSource != null && audioSource.isPlaying;} }    

    [Header("Flags")]
    // Emoji flags
    private bool isEmojiCoroutineRunning;
    public bool IsEmojiCoroutineRunning => isEmojiCoroutineRunning;

    // 1st Click Flag
    private bool isFirstClick = true;
    public bool IsFirstClick
    {
        get => isFirstClick;
        set => isFirstClick = value;
    }

    // Pause Flag
    private bool isPaused = false;
    #endregion

    //////////////////////////////////////////////////////////    
    // NOT USED FOR CLASSES INHERITED FROM MONOBEHAVIOUR /////
    //////////////////////////////////////////////////////////

    // Private constructor to avoid the instances creation with "new"
    //private GameManager()  { Debug.Log("GameManager created"); }

    #region UnityAPI
    // In case There will be any GameManager script already joint to any GO on the Project
    private void Awake()
    {   
        // If doesn't exist already an instance of the GameManager script
        // then saves the current instance on the singleton var.
        if (gm == null)
        {
            gm = this;
            //DontDestroyOnLoad(gameObject); // Avoids the GO will be destroyed when the Scene changes
        }            
        // Otherwise, if does already exists an instance of the GameManager script then
        // we'll destroy the GO is attached to
        else
            Destroy(gameObject);

        isDied = false;

        //// Setup the Grid Layout component        
        //GridLayoutSetup();

        rectTransform = gamePanel.GetComponent<RectTransform>();

        audioSource = GetComponent<AudioSource>();

        // Start playing Title Screen Audio
        PlayTitleAudioClip();

        //// Setup the Grid Layout component        
        //GridLayoutSetup();

        //StartGame();

        // Enable the TitlePanel Screen
        titlePanel.SetActive(true);        
        // Update the Panel Selected State
        panelSelected = PanelSelected.Title;
    }

    private void Update()
    {
        //if (panelSelected == PanelSelected.Game)
        CheckEmojiState();

        // Check also if we are on the Pause Panel or not
        if ((panelSelected == PanelSelected.Game || panelSelected == PanelSelected.Pause) && Input.GetKeyDown(KeyCode.P))
        {
            TooglePause();
        }
    }
    #endregion

    #region Public_Methods
    #region ButtonMethods
    public void OnTitleScreenClick()
    {
        // Disable  the TitlePanel Screen & Enable the Menu Panel
        titlePanel.SetActive(false);
        menuPanel.SetActive(true);

        // Update the Panel Selected State
        panelSelected = PanelSelected.Menu;        
    }
    public void OnToLevelSelectorButtonClick()
    {
        // Disable  the Menu Panel & Enable the SelectLevel Panel
        menuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);

        // Fade the Menu Panel buttons
        menuPanelStartButton.FadeButton();
        menuPanelOptionsButton.FadeButton();

        // Start playing Select Level Screen Audio
        PlaySelectLevelAudioClip();

        // Update the Panel Selected State
        panelSelected = PanelSelected.LevelSelection;
    }
    public void OnExitGameButtonClick()
    {
        // Disable  the Menu Panel & Enable the SelectLevel Panel
        //gamePanel.SetActive(false);
        backgroundPanel.SetActive(false);
        levelSelectPanel.SetActive(true);

        // Unset the Level Selected
        levelSelected = LevelSelected.None;

        // Start playing Select Level Screen Audio
        PlaySelectLevelAudioClip();

        // Update the Panel Selected State
        panelSelected = PanelSelected.LevelSelection;
    }

    public void OnToOptionsButtonClick()
    {
        // Disable  the Menu Panel & Enable the Options Panel
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);

        // Fade the Menu Panel buttons
        menuPanelStartButton.FadeButton();
        menuPanelOptionsButton.FadeButton();

        // Update the Panel Selected State
        panelSelected = PanelSelected.Options;
    }
    public void OnExitOptionsScreenClick()
    {
        // Disable  the Options Panel & Enable the Menu panel
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);        

        // Update the Panel Selected State
        panelSelected = PanelSelected.Menu;
    }

    public void OnSelectModeClick()
    {        
        // Checked which Level Button has been clicked and set the corresponding Level Mode
        GameObject clickedObject = EventSystem.current?.currentSelectedGameObject;        
        if (clickedObject != null && clickedObject.name == "EasyButton")
        {
            Debug.Log("Easy Mode Button was pressed");
            // Level Selection
            levelSelected = LevelSelected.Easy;
        }
        else if (clickedObject != null && clickedObject.name == "NormalButton")
        {
            Debug.Log("Normal Mode Button was pressed");
            // Level Selection
            levelSelected = LevelSelected.Normal;
        }
        else if (clickedObject != null && clickedObject.name == "HardButton")
        {
            Debug.Log("Hard Mode Button was pressed");
            // Level Selection
            levelSelected = LevelSelected.Hard;
        }        
        else
        {
            Debug.Log("The clicked object is null");
            return;
        }

        // Setup the Grid Layout component        
        GridLayoutSetup();

        // Setup the Game Panel Rect Transform
        GamePanelSetup();

        // Disable  the SelectLevel Panel & Enable the Game Panel      
        levelSelectPanel.SetActive(false);
        //gamePanel.SetActive(true);
        backgroundPanel.SetActive(true);

        // Fade the Menu Panel buttons
        selectLevelEasyButton.FadeButton();
        selectLevelNormalButton.FadeButton();
        selectLevelHardButton.FadeButton();

        // Update the Panel Selected State
        panelSelected = PanelSelected.Game;

        // Create the Board Panel with the corresponding cells and bombs number
        // according to the Level difficulty
        StartGame();

        // Start playing Gamen Audio
        PlayGameAudioClip();
    }            
        
    #endregion
    public void Replay()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //PlayRestartAudioClip();
        StartCoroutine(nameof(RestartScene));
    }
    public void UpdateRemainingCells(bool isIncrement)
    {
        if (isIncrement)        
            remainingCells++;                    
        else        
            remainingCells--;                    
        Debug.Log("A total of " + remainingCells + " pending cells out of " +
                       (width * height - bombsAmount) + " cells");

        if (isDied == false && remainingCells == 0)
        {
                // Hides the Title pannel & Show the Win Pannel
                titlePanel.SetActive(false);
                winPanel.SetActive(true);

                // Update the Panel Selected State
                panelSelected = PanelSelected.Win;

                // Play the Win Game Audio
                PlayWinAudioClip();
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
            if (map[x + 1, y].bomb)                           // Right
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

        map[x, y].BombsAroundChecked = true;

        return counter;
    }

    // Open all the buttons with no-bomb around the clicked button
    public void ClickAround_Old(int x, int y)
    {
        int xTargetCell = x, yTargetCell = y;
        bool cellToCheck = false;

        // 1. Check the bombs of all the buttons around it (CheckBombNumber(x,y))
        for (int i = 0; i < 8; i++)
        {
            cellToCheck = false;

            switch (i)
            {
                // Left
                case 0:
                    xTargetCell = x - 1;
                    yTargetCell = y;
                    if (x > 0 &&
                        !map[xTargetCell, yTargetCell].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
                // Left-up
                case 1:
                    xTargetCell = x - 1;
                    yTargetCell = y + 1;
                    if (x > 0 && y < (height - 1) &&
                        !map[xTargetCell, yTargetCell].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
                // Up
                case 2:
                    xTargetCell = x;
                    yTargetCell = y + 1;
                    if (y < (height - 1) &&
                        !map[x, y + 1].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
                // Right-up
                case 3:
                    xTargetCell = x + 1;
                    yTargetCell = y + 1;
                    if (x < (width - 1) && y < (height - 1) &&
                        !map[xTargetCell, yTargetCell].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
                // Right
                case 4:
                    xTargetCell = x + 1;
                    yTargetCell = y;
                    if (x < (width - 1) &&
                        !map[xTargetCell, yTargetCell].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
                // Right-down
                case 5:
                    xTargetCell = x + 1;
                    yTargetCell = y - 1;
                    if (x < (width - 1) && y > 0 &&
                        !map[xTargetCell, yTargetCell].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
                // Down
                case 6:
                    xTargetCell = x;
                    yTargetCell = y - 1;
                    if (y > 0 &&
                        !map[xTargetCell, yTargetCell].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
                // Left-down
                case 7:
                    xTargetCell = x - 1;
                    yTargetCell = y - 1;
                    if (x > 0 && y > 0 &&
                        !map[xTargetCell, yTargetCell].bomb &&
                        !map[xTargetCell, yTargetCell].BombsAroundChecked)
                        cellToCheck = true;
                    break;
            }
            if (cellToCheck)
            {
                // If the requirements are met there will be check the bombs
                // around the current Target cell
                int targetCellBombs = CheckBombNumber(xTargetCell, yTargetCell);

                // Write the number of bombs around the new Target Cell
                // In case bombs>0 the cell text will be set as the number of bombs
                // Oherwise, if bombs = 0 --> the cell text will be empty and
                // 'ClickAround' will be called again for the new Cell To check
                map[xTargetCell, yTargetCell].SetBombsAround(targetCellBombs);
            }
        }
    }

    // Open all the buttons with no-bomb around the clicked button
    public void ClickAround(int x, int y)
    {
        //Left Cells
        if (x > 0)
        {
            // Left
            map[x - 1, y].Click();
            // Left-Up
            if (y < (height - 1))
                map[x - 1, y + 1].Click();
            // Left-Down
            if (y > 0)
                map[x - 1, y - 1].Click();
        }
        // Right Cells
        if (x < (width - 1))
        {
            // Right
            map[x + 1, y].Click();
            // Right-up
            if (y < (height - 1))
                map[x + 1, y + 1].Click();
            // Right-Down
            if (y > 0)
                map[x + 1, y - 1].Click();
        }
        // Up Cell
        if (y < (height - 1))
            map[x, y + 1].Click();
        // Down Cell
        if (y > 0)
            map[x, y - 1].Click();
    }

    public void ExplodeMap()
    {
        // Explode & Show all the bombs of the level and the rest of cells
        isDied = true;
        foreach (ButtonScript button in map)
            button.Click();

        //// Show the Sad Emoji        
        //SetSadEmoji();

        //// Play the Fail Audio clip
        //PlayFailAudioClip();
    }
    #endregion

    #region AudioMenuMethods
    public void PlayGameAudioClip()
    {
        audioSource.volume = 0.3f;
        PlayAudioClip(gameAudioclip);
    }
    public void StopAudioSourceClip()
    {
        audioSource.Stop();
    }
    public void PlayTitleAudioClip()
    {
        audioSource.volume = 0.5f;
        PlayAudioClip(titleAudioclip);
    }
    public void PlaySelectLevelAudioClip()
    {
        audioSource.volume = 0.3f;
        PlayAudioClip(selectLevelAudioclip);
    }
    #endregion

    #region AudioFXMethods
    
    public void PlayPauseAudioClip()
    {
        audioSource.volume = 1f;
        PlayAudioClip(pauseAudioclip);
    }
    public void PlaySelectionFXAudioClip()
    {
        audioSource.volume = 1f;
        PlayAudioClip(selectFXAudioclip);
    }
    public void PlaySuccessAudioClip()
    {
        gameFXAudioSource.volume = 1f;
        PlayAudioGameFXClip(successAudioclip);
    }    
    public void PlayGreatSuccessAudioClip()
    {
        gameFXAudioSource.volume = 1f;
        PlayAudioGameFXClip(greatSuccessAudioclip);
    }
    public void PlayWinAudioClip()
    {
        gameFXAudioSource.volume = 1f;
        PlayAudioGameFXClip(winAudioclip);
    }
    public void PlayFailAudioClip()
    {
        gameFXAudioSource.volume = 1f;
        PlayAudioGameFXClip(failAudioclip);
    }
    public void PlayRestartAudioClip()
    {
        //audioSource.Stop();
        gameFXAudioSource.volume = 1f;
        PlayAudioGameFXClip(restartAudioclip);
    }
    public void PlayRightClickAudioClip()
    {
        gameFXAudioSource.volume = 0.3f;
        PlayAudioGameFXClip(rightClickAudioclip);
    }
    #endregion

    #region EmojiMethods
    public void SetNormalEmoji()
    {
        imageEmoji.sprite = images[0];
        elapsedEmojiTime = 0f;
    }
    public void SetSadEmoji()
    {
        imageEmoji.sprite = images[1];
        elapsedEmojiTime = 0f;
    }
    public void SetGlassesEmoji()
    {
        imageEmoji.sprite = images[2];
        elapsedEmojiTime = 0f;
    }
    public void SetUpsEmoji()
    {
        imageEmoji.sprite = images[3];
        elapsedEmojiTime = 0f;
    }
    #endregion
    #endregion

    #region Private_Methods
    #region SetupMethods
    private void GridLayoutSetup()
    {
        // Set the corresponding cellSize and spacing in func. of the Level Selected
        // Easy Mode
        if (levelSelected == LevelSelected.Easy)
        {
            cellSize = new Vector2(100, 100);
            spacing = new Vector2(5, 5);
            height = 8;
            width = 8;
        }
        // Normal or Hard Modes
        else 
        {
            cellSize = new Vector2(50, 50);
            spacing = new Vector2(3, 3);
            height = 16;
            // Normal Mode
            if (levelSelected == LevelSelected.Normal)
                width = 16;
            // Hard Mode
            else
                width = 30;
        }

        gridLayout = gamePanel.GetComponent<GridLayoutGroup>();

        gridLayout.cellSize = new Vector2(cellSize.x, cellSize.y);
        gridLayout.spacing = new Vector2(spacing.x, spacing.y); 
        
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = width;
    }
    private void GamePanelSetup()
    {
        // For Easy and Normal modes
        if (levelSelected <= LevelSelected.Normal)
            rectTransform.sizeDelta = new Vector2(980, rectTransform.sizeDelta.y);
        // For Hard Mode
        else
            rectTransform.sizeDelta = new Vector2(1650, rectTransform.sizeDelta.y);
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
    private void CreateBombs()
    {
        //NumbersOfBombs();

        for (int i = 0; i < bombsAmount; i++)
        {
            int x = 0, y = 0;

            do
            {
                x = Random.Range(0, width - 1);
                y = Random.Range(0, height - 1);
            } while (map[x, y].bomb);

            map[x, y].bomb = true;

            //Debug.Log("Bomb assigned on [" + map[x, y].x + "," + map[x, y].y + "]" +
            //                "|| Still are pending " + (bombsAmount-(i+1)) + " bombs");
        }
    }
    private void NumbersOfBombs()
    {
        int numOfCells = width * height;

        // Set the Range between a 15-20% of the Min and Max of Bombs
        bombsAmount = Random.Range((int)(numOfCells * 0.15f), (int)(numOfCells * 0.20f));
        //bombsAmount = 10; // WinPanel Test

        //// The board has only 10 buttons then it generates only 1 bomb
        //if (width*height <= 10)        
        //    bombsAmount = 1;
        //// The board has between 10-30 buttons then it generates 7 bombs
        //else if (width * height > 10 && width * height < 30 )
        //    bombsAmount = 7;        
    }
    private void StartGame()
    {        
        // Reset the boolean flags (isDied & isFirstClick)
        ResetFlags();        
        // Set the initial Emoji
        SetNormalEmoji();

        // Set randomly the amount of bombs for every round
        NumbersOfBombs();

        // Init the bidiomensional array
        map = new ButtonScript[width, height];

        // The number of Cells to open in order to win
        remainingCells = (width * height) - bombsAmount;

        // Destroy all the GamePanel children (All buttons instances) in case it has any children.
        if (gamePanel.transform.childCount > 0)
            DestroyGamePanelChildren();

        // Generate all the buttons and place them on the Grid Layout
        GenerateButtons();
        // Create randomly all the bombs of the level
        CreateBombs();

        // Set the success Audioclip
        //audioSource = GetComponent<AudioSource>();
        //audioSource.clip = successAudioclip;
    }
    private void DestroyGamePanelChildren()
    {
        foreach(Transform child in gamePanel.transform)
            Destroy(child.gameObject);
    }
    private void ResetFlags()
    {
        // Reset the Died & FirstClick Flags
        isDied = false;
        isFirstClick = true;
    }
    #endregion

    #region Pause
    private void TooglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;                // Stops the game (stop the physics and pending updates which are time dependent)
            audioSource.Pause();
            pauseLevelPanel.SetActive(true);
            // Update the Panel Selected State
            panelSelected = PanelSelected.Pause;
        }
        else
        {
            Time.timeScale = 1f;                // Resumes the game
            audioSource.Play();
            pauseLevelPanel.SetActive(false);
            panelSelected = PanelSelected.Game;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator RestartScene()
    {
        PlayRestartAudioClip();
        yield return new WaitWhile (() => gameAudioFxIsPlaying);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);        

        // Create again the Board Panel
        StartGame();
        // Start playing Gamen Audio again
        PlayGameAudioClip();
    }
    #endregion

    #region AudioManager
    private void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
    private void PlayAudioGameFXClip(AudioClip audioClip)
    {
        gameFXAudioSource.clip = audioClip;
        if (!gameFXAudioSource.isPlaying)
            gameFXAudioSource.Play();
    }
    #endregion

    #region EmojiUpdate
    private void CheckEmojiState()
    {
        // Keeps a diffent emoji than the default one for only 2s
        if (!isDied && imageEmoji.sprite != images[0])
        {
            elapsedEmojiTime += Time.deltaTime;
            
            if (elapsedEmojiTime >= emojiMaxTime)
            {
                imageEmoji.sprite = images[0];
                elapsedEmojiTime = 0f;
            }                                            
        }
    }
    #endregion
    #endregion
}
