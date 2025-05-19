using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Leaderboards;

public class WordSearchMananger : MonoBehaviour
{
    //todo
    //implament multiplayer - both comp and friendly
    //get it on steam, then make it for mobile
    //ads/ ingame purchases

    #region Vars

    public bool isPlaying = true;
    private int RandomSeed = 5;
    [Header("Time")]
    [SerializeField] float TimeToComplete = 0;
    float timer;
    [SerializeField] TMP_Text InGameTimeText;
    [SerializeField] TMP_Text playedTimeText;
    [SerializeField] TMP_Text addedTimeText;
    float gameTimer = 0;
    public float LerpTime;
    [Space(15)]
    string SelectedLetters;
    [HideInInspector]
    public bool CanSelect = false;
    [Space(15)]
    public Color SelectColor;
    public Color Correct;
    public Color Wrong;
    List<GameObject> SelectedLettersList = new();
    List<GameObject> CorrectLettersList = new();
    [Space(15)]
    private int BoardSize = 15;
    [HideInInspector]
    public int WordsLeft = 0;
    [Space(15)]
    //[HideInInspector]
    public List<string> WordsToFind;
    [Space(15)]
    public TMP_Text nameText;
    public TMP_Text CompleteTimeText;
    [Header("Prefabs")]
    public GameObject LetterPrefab;
    public GameObject WordPrefab;
    [Space(15)]
    public Transform WordSearchGridParent;
    public Transform WordsInWordSearchParent;
    [Space(15)]
    public GameObject OnWordSearchComplete;
    public GameObject OnWordSearchPause;
    public Button PauseButton;
    [Space(15)]
    private char BlankChar = '.';
    public char[,] WordSearchBoard;

    [Header("Score")]
    public int score;
    int scoreToAdd;
    int scoreMultiplier = 0;
    [Space(15)]
    public int scorePerWord = 100;
    public float timeToAdd = 5;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text multiplierText;
    [SerializeField] Image multiplierBar;
    float multiplierTimer = 0;

    //private string URL = "https://randomword.com/";

    #endregion
    private void Start()
    {
        MakeWordSearch();

        timer = TimeToComplete;
    }

    /// <summary>
    /// creates a word search
    /// </summary>
    public void MakeWordSearch()
    {
        //set the size of the board with the grid layout group
        WordSearchGridParent.GetComponent<GridLayoutGroup>().constraintCount = BoardSize;

        //loads the word from the selected puzzle
        LoadWordsToFind();

        //random seed
        RandomSeed = Random.Range(0, 1000000);
        WordSearchBoard = MakeGrid(RandomSeed);

        //make a grid
        MakeUiGrid(WordSearchBoard);

        //tell the user what words to find
        PlaceWhatWordsAreInThePuzzle();

        WordsLeft = HowManyWordsAreLeft();
        //TimeToComplete = 0;
    }

    #region Create Word Search

    /// <summary>
    /// load what words to find
    /// </summary>
    void LoadWordsToFind()
    {
        //WordsToFind = GetDataFromWebpage(URL);
    }

    public char[,] MakeGrid(int seed)
    {
        //make a new char 2d array with board size dimensions
        char[,] newGrid = new char[BoardSize, BoardSize];

        //I was new and I filled board with blank chars, but it does that by default so this is extra
        newGrid = InitBoard();

        //places words on the board
        newGrid = PlaceWordsInTheBoard();

        //if (WantRandomLetters)
            newGrid = FillTheBoardWithRandomChars(newGrid, seed);

        return newGrid;
    }

    /// <summary>
    /// fills the board with the blank char
    /// </summary>
    char[,] InitBoard()
    {
        char[,] newGrid = new char[BoardSize, BoardSize];

        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                newGrid[i, j] = BlankChar;
            }
        }

        return newGrid;
    }

    /// <summary>
    /// places a word on the board
    /// </summary>
    /// <param name="placeForwards"></param>
    /// <param name="word"></param>
    /// <param name="startX"></param>
    /// <param name="StartY"></param>
    /// <param name="xSlope"></param>
    /// <param name="ySlope"></param>
    /// <param name="newGrid"></param>
    void PlaceWord(bool placeForwards, string word, int startX, int StartY, int xSlope, int ySlope, char[,] newGrid)
    {
        //forwards
        if (placeForwards)
        {
            //places the word you want by a slope of x / y
            for (int i = 0; i < word.Length; i++)
            {
                newGrid[startX + i * xSlope, StartY + i * ySlope] = word[i];
            }
        }
        //backwards
        else
        {
            //places the word you want by a slope of x / y, but backwards
            for (int i = 0; i < word.Length; i++)
            {
                newGrid[startX + i * xSlope, StartY + i * ySlope] = word[word.Length - 1 - i];
            }
        }
    }

    /// <summary>
    /// places the word that are in the list on the board
    /// </summary>
    char[,] PlaceWordsInTheBoard()
    {
        char[,] newGrid = new char[BoardSize, BoardSize];

        //from the list of words
        foreach (string word in WordsToFind)
        {
            bool canPlace = true;

            while (canPlace)
            {
                //random cords x and y in the board
                int placeCordsX = Random.Range(0, BoardSize);
                int placeCordsY = Random.Range(0, BoardSize);

                //0 - horizontal
                //1 - Vertical
                //2 - Pos Diagonal
                //3 - Neg Diagonal
                int howToPlace = Random.Range(0, 2);


                //the placing methods are pretty much the same, just build for what they need to do
                void PlaceHorizontal()
                {
                    // check if you can place the word
                    if (word.Length + placeCordsX < BoardSize)
                    {
                        if (IsValidForHorz(true, word, placeCordsX, placeCordsY, newGrid))
                        {
                            //place it forwards!
                            PlaceWord(true, word, placeCordsX, placeCordsY, 1, 0, newGrid);
                            canPlace = false;
                        }

                        //check if you want the word to be placed forwards or backwards
                        //if (Random.Range(0, 2) == 0)
                        //{
                        //    //check if the word is valid
                            
                        //}
                        //else
                        //{
                        //    //place it backwards!
                        //    if (IsValidForHorz(false, word, placeCordsX, placeCordsY, newGrid))
                        //    {
                        //        PlaceWord(false, word, placeCordsX, placeCordsY, 1, 0, newGrid);
                        //        canPlace = false;
                        //    }
                        //}
                    }
                }

                void PlaceVert()
                {
                    if (word.Length + placeCordsY < BoardSize)
                    {
                        if (IsValidForVert(true, word, placeCordsX, placeCordsY, newGrid))
                        {

                            PlaceWord(true, word, placeCordsX, placeCordsY, 0, 1, newGrid);
                            canPlace = false;
                        }

                        //if (Random.Range(0, 2) == 0)
                        //{
                            
                        //}
                        //else
                        //{
                        //    if (IsValidForVert(false, word, placeCordsX, placeCordsY, newGrid))
                        //    {
                        //        PlaceWord(false, word, placeCordsX, placeCordsY, 0, 1, newGrid);
                        //        canPlace = false;
                        //    }
                        //}
                    }
                }

                void PositiveDiagonals()
                {
                    if (word.Length + placeCordsX < BoardSize && word.Length + placeCordsY < BoardSize)
                    {
                        if (isValidForDiagonalPos(true, word, placeCordsX, placeCordsY, newGrid))
                        {
                            PlaceWord(true, word, placeCordsX, placeCordsY, 1, 1, newGrid);
                            canPlace = false;
                        }

                        //if (Random.Range(0, 2) == 0)
                        //{
                            
                        //}
                        //else
                        //{
                        //    if (isValidForDiagonalPos(false, word, placeCordsX, placeCordsY, newGrid))
                        //    {
                        //        PlaceWord(false, word, placeCordsX, placeCordsY, 1, 1, newGrid);
                        //        canPlace = false;
                        //    }
                        //}
                    }
                }

                void NegativeDiagonals()
                {
                    if (word.Length + placeCordsX < BoardSize && word.Length + placeCordsY < BoardSize && placeCordsX > word.Length && placeCordsY > word.Length)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            if (IsValidForDiagonalNeg(true, word, placeCordsX, placeCordsY, newGrid))
                            {
                                PlaceWord(true, word, placeCordsX, placeCordsY, 1, -1, newGrid);
                                canPlace = false;
                            }
                        }
                        else
                        {
                            if (IsValidForDiagonalNeg(false, word, placeCordsX, placeCordsY, newGrid))
                            {
                                PlaceWord(false, word, placeCordsX, placeCordsY, 1, -1, newGrid);
                                canPlace = false;
                            }
                        }
                    }
                }

                if (howToPlace == 0)
                    PlaceHorizontal();
                if (howToPlace == 1)
                    PlaceVert();
                //if (howToPlace == 2)
                //    PositiveDiagonals();
                //if (howToPlace == 3)
                //    NegativeDiagonals();
            }
        }

        return newGrid;
    }

    #region IsValids
    //checks if the word can be placed, i think there might be a way to get this under one method but I gave up and this worked so happens
    bool IsValidForHorz(bool checkForward, string word, int startX, int startY, char[,] grid)
    {
        if (checkForward)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX + i, startY]))
                    if ((int)grid[startX + i, startY] != (int)(word[i]))
                        return false;
            }
        }
        else
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX + i, startY]))
                    if ((int)grid[startX + i, startY] != (int)(word[word.Length - 1 - i]))
                        return false;
            }
        }
        return true;
    }
    bool IsValidForVert(bool checkForward, string word, int startX, int startY, char[,] grid)
    {
        if (checkForward)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX, startY + i]))
                    if ((int)grid[startX, startY + i] != (int)(word[i]))
                        return false;
            }
        }
        else
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX, startY + i]))
                    if ((int)grid[startX, startY + i] != (int)(word[word.Length - 1 - i]))
                        return false;
            }
        }

        return true;
    }
    bool isValidForDiagonalPos(bool checkForward, string word, int startX, int startY, char[,] grid)
    {
        if (checkForward)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX + i, startY + i]))
                    if ((int)grid[startX + i, startY + i] != (int)(word[i]))
                        return false;
            }
        }
        else
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX + i, startY + i]))
                    if ((int)grid[startX + i, startY + i] != (int)(word[word.Length - 1 - i]))
                        return false;
            }
        }


        return true;
    }
    bool IsValidForDiagonalNeg(bool checkForward, string word, int startX, int startY, char[,] grid)
    {
        if (checkForward)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX + i, startY - i]))
                    if ((int)grid[startX + i, startY - i] != (int)(word[i]))
                        return false;
            }
        }
        else
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(grid[startX + i, startY - i]))
                    if ((int)grid[startX + i, startY - i] != (int)(word[word.Length - 1 - i]))
                        return false;
            }
        }
        return true;
    }

    #endregion

    /// <summary>
    /// takes any place that is not fill in already and turns it into a random letter
    /// </summary>
    char[,] FillTheBoardWithRandomChars(char[,] grid, int seed)
    {
        //this is used to fill the board with random letters after you get the wanted words down
        char[,] newGrid = grid;

        Random.InitState(seed);
        
        //go through board
        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                //if there is not a letter at the spot
                if (!char.IsLetter(newGrid[row, col]))
                {
                    //place a random letter
                    newGrid[row, col] = RandomLetter();
                }
            }
        }
        return newGrid;
    }

    /// <summary>
    /// returns a random lower case letter
    /// </summary>
    /// <returns></returns>
    char RandomLetter()
    {
        //ascii range for upper case letters
        //https://en.cppreference.com/w/cpp/language/ascii
        return (char)Random.Range(97, 123);
    }

    /// <summary>
    /// makes the Ui elements that appear on the screen
    /// </summary>
    public void MakeUiGrid(char[,] grid)
    {
        //spawns in the letters that make the grid you play on
        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                GameObject letter = SpawnLetter();

                letter.GetComponent<WordSearchLetterController>().row = row;
                letter.GetComponent<WordSearchLetterController>().col = col;

                //checks if you need to update the text on the letter
                if (grid[row, col] != BlankChar)
                {
                    letter.GetComponentInChildren<TMP_Text>().text = grid[row, col].ToString();
                }
                else
                {
                    letter.GetComponentInChildren<TMP_Text>().text = BlankChar.ToString();
                }
            }
        }
    }

    // houses the spawning of the letter
    GameObject SpawnLetter()
    {
        //pro tip: learn about grid layout groups. they are super helpful for all ui
        GameObject letter = Instantiate(LetterPrefab, Vector3.zero, Quaternion.identity, WordSearchGridParent);
        //letter.GetComponentInChildren<TMP_Text>().rectTransform.sizeDelta = letter.GetComponent<RectTransform>().sizeDelta * 4;

        return letter;
    }

    /// <summary>
    /// sets the words that are on the side
    /// </summary>
    void PlaceWhatWordsAreInThePuzzle()
    {
        //theres are the words that you are looking for displayed on the left side of the board
        foreach (string i in WordsToFind)
        {
            GameObject word = Instantiate(WordPrefab, Vector3.zero, Quaternion.identity, WordsInWordSearchParent);
            word.transform.localScale = Vector3.zero;

            word.GetComponentInChildren<TMP_Text>().text = i;
        }

        //lerp theses bad boys in
        //ALSO GET DOTWEEN FROM UNITY ASSET STORE - ITS THE BEST THING EVER
        foreach (Transform i in WordsInWordSearchParent) 
        {
            //i.DOScale(Vector3.one, LerpTime).SetEase(Ease.InOutQuint);
            i.localScale = Vector3.one;
        }
    }

    #endregion

    #region Finding a Word
    /// <summary>
    /// highlights what the player is selecting
    /// </summary>
    void FillInSelectedLetters()
    {
        if (SelectedLettersList.Count > 0)
        {
            //set the color of the selected letter list to the select color
            foreach (GameObject i in SelectedLettersList)
            {
                i.GetComponentInChildren<Image>().color = SelectColor;
            }
        }
    }

    /// <summary>
    /// clears the highlights and reset letters status
    /// </summary>
    public void ClearHighlights()
    {
        //return to defaults
        foreach (GameObject i in SelectedLettersList)
        {
            i.GetComponentInChildren<Image>().color = i.GetComponent<WordSearchLetterController>().DefaultColor;
            i.GetComponent<WordSearchLetterController>().isSelected = false;
        }

        //clear list
        SelectedLettersList.Clear();
    }

    /// <summary>
    /// adds the letter to the selected string
    /// </summary>
    /// <param name="letter"></param>
    public void AddLetterToSelected(GameObject letter)
    {
        SelectedLetters += WordSearchBoard[letter.GetComponent<WordSearchLetterController>().row, letter.GetComponent<WordSearchLetterController>().col].ToString();
    }

    /// <summary>
    /// if the letters isnt added to list, add it
    /// </summary>
    /// <param name="letter"></param>
    public void AddToSelected(GameObject letter)
    {
        if (!SelectedLettersList.Contains(letter)) 
        {
            SelectedLettersList.Add(letter);
        }
    }

    /// <summary>
    /// check if the string made returns a word in the to find array
    /// </summary>
    /// <returns></returns>
    public bool CheckIfLettersMakeAWord()
    {
        string reverseSelected = null;

        if (SelectedLetters == null) return false;

        //flips the word around
        if (SelectedLetters.Length > 0)
        {
            char[] reverseCharArray = SelectedLetters.ToCharArray();
            System.Array.Reverse(reverseCharArray);
            reverseSelected = new string(reverseCharArray);
        }
        else
            return false;

        //is there a word that is forwards or backwards in the list?
        if (WordsToFind.Contains(SelectedLetters) || WordsToFind.Contains(reverseSelected)) 
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Check Word Info
    /// <summary>
    /// makes the word correct
    /// </summary>
    void MakesLettersOnBoardCorrect()
    {
        foreach (GameObject i in CorrectLettersList)
        {
            i.GetComponentInChildren<Image>().color = Correct;
            i.GetComponent<WordSearchLetterController>().isSelected = false;
        }

        
    }

    /// <summary>
    /// makes a correct word to find the correct color on a correct find
    /// </summary>
    void MakesListWordCorrect()
    {
        string reverseSelected = null;

        char[] reverseCharArray = SelectedLetters.ToCharArray();
        System.Array.Reverse(reverseCharArray);
        reverseSelected = new string(reverseCharArray);

        foreach (Transform i in WordsInWordSearchParent)
            if (i.GetComponentInChildren<TMP_Text>().text == SelectedLetters || i.GetComponentInChildren<TMP_Text>().text == reverseSelected)
                i.GetComponentInChildren<TMP_Text>().color = Correct;

        AddScore();
        AddMultiplier();
    }

    void AddMultiplier()
    {
        timer += timeToAdd;
        multiplierTimer = timeToAdd * 2;

        scoreText.rectTransform.localScale = new Vector3(2f, 2f, 2f);
        LeanTween.scale(scoreText.gameObject, Vector3.one, 0.5f).setEaseOutBounce();

        addedTimeText.text = "+" + timeToAdd.ToString() + "s";

        addedTimeText.rectTransform.localScale = new Vector3(0, 1, 1);
        LeanTween.scale(addedTimeText.gameObject, new Vector3(1, 1, 1), 0.15f).setOnStart(() =>
        {
            LeanTween.alphaCanvas(addedTimeText.GetComponent<CanvasGroup>(), 1, 0.1f);
        }).setOnComplete(() =>
        {
            LeanTween.scale(addedTimeText.gameObject, new Vector3(0, 1, 1), 0.15f).setDelay(3f).setOnStart(() =>
            {
                LeanTween.alphaCanvas(addedTimeText.GetComponent<CanvasGroup>(), 0, 0.1f);
            });
        });
    }

    /// <summary>
    /// returns how many words are left to find
    /// </summary>
    /// <returns></returns>
    int HowManyWordsAreLeft()
    {
        int total = 0;
        foreach (Transform i in WordsInWordSearchParent)
            if (i.GetComponentInChildren<TMP_Text>().color != Correct)
                total++;

        return total;
    }

    /// <summary>
    /// what to do whne a word search is complete
    /// </summary>
    void OnPuzzleComplete(bool time)
    {
        if (!isPlaying) return;

        SumScore();
        //OnWordSearchComplete.transform.DOScale(Vector3.one, LerpTime);
        //OnWordSearchComplete.transform.localScale = Vector3.one;
        LeanTween.scale(OnWordSearchComplete, Vector3.one, 0.5f).setEaseInOutCirc().setDelay(0.75f);
        nameText.text = DataController.Instance.GetUserData().username;
        CompleteTimeText.text = "Parabéns! Sua pontuação é: " + GetFinalScore() + " pontos!";
        DataController.Instance.SaveUserData(GetFinalScore());

        //SaveDataAsync();
    }
    async void SaveDataAsync()
    {
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync("Online_Players", GetFinalScore());
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// what to do when the selected letters IS a word
    /// 
    /// </summary>
    void IsAWord()
    {
        foreach (GameObject i in SelectedLettersList)
        {
            CorrectLettersList.Add(i);
        }

        MakesListWordCorrect();
        MakesLettersOnBoardCorrect();

        WordsLeft = HowManyWordsAreLeft();

        if (WordsLeft == 0)
        {
            OnPuzzleComplete(false);
        }

        SelectedLettersList.Clear();
        CanSelect = false;
        SelectedLetters = null;
    }

    /// <summary>
    /// what to do when the selected letters is NOT a word
    /// </summary>
    IEnumerator NotAWord()
    {
        CanSelect = false;

        foreach (GameObject i in SelectedLettersList)
        {
            i.GetComponentInChildren<Image>().color = Wrong;
        }

        yield return new WaitForSeconds(.15f);

        ClearHighlights();

        MakesLettersOnBoardCorrect();

        SelectedLetters = null;
    }

    /// <summary>
    /// the act of getting the words on the board, mouse input
    /// </summary>
    void SelectLettersOnBoard()
    {
        if (Input.GetMouseButtonDown(0))
            CanSelect = true;
        if (Input.GetMouseButtonUp(0))
        {
            if (CheckIfLettersMakeAWord())
                IsAWord();
            else
                StartCoroutine(NotAWord());
        }
    }

    #endregion

    #region Pause Menu
    void OpenPauseMenu()
    {
        isPlaying = false;
        //OnWordSearchPause.transform.DOScale(Vector3.one, LerpTime / 2);

        OnWordSearchPause.transform.localScale = Vector3.one;
    }

    void ClosePauseMenu()
    {
        //OnWordSearchPause.transform.DOScale(Vector3.zero, LerpTime / 3);
        OnWordSearchPause.transform.localScale = Vector3.zero;
        isPlaying = true;
    }

    public void OpenOrCloseMenu()
    {
        if (isPlaying)
            OpenPauseMenu();
        else
            ClosePauseMenu();
    }

    public void LoadPuzzle()
    {
        //StartCoroutine(FindFirstObjectByType<TransitionController>().MoveScenes("WordSearch", "Loading Word Search...", true));
    }

    public void ToMainMenu()
    {
        //StartCoroutine(FindFirstObjectByType<TransitionController>().MoveScenes("MainMenu", "Loading Main Menu...", false));
    }
    #endregion

    private void FixedUpdate()
    {
        if (WordsLeft > 0 && isPlaying && timer > 0)
        {
            timer -= Time.deltaTime;
            gameTimer += Time.deltaTime;

            //string secs = TimeToComplete % 60 <= 10 ? "0" + (TimeToComplete % 60).ToString("F0") : (TimeToComplete % 60).ToString("F0");
            //string mins = (TimeToComplete / 60).ToString("F0");

            float minutes = Mathf.FloorToInt(timer / 60);
            float seconds = Mathf.FloorToInt(timer % 60);

            float playedMinutes = Mathf.FloorToInt(gameTimer / 60);
            float playedSeconds = Mathf.FloorToInt(gameTimer % 60);

            InGameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            playedTimeText.text = string.Format("{0:00}:{1:00}", playedMinutes, playedSeconds);

            //InGameTimeText.text = mins + " : " + secs;
        } else if (timer <= 0)
        {
            OnPuzzleComplete(true);
            isPlaying = false;
        }

            scoreText.text = (score + scoreToAdd).ToString();
        if (scoreMultiplier > 0)
        {
            multiplierText.text = scoreMultiplier.ToString() + "x";
            multiplierBar.transform.parent.gameObject.SetActive(true);
            multiplierBar.fillAmount = multiplierTimer / (timeToAdd * 2);
        }
        else
        {
            multiplierText.text = "";
            multiplierBar.transform.parent.gameObject.SetActive(false);
        }

        if (WordsLeft == 0)
            isPlaying = false;

        // Substituir pelo novo Input System / Touch Script
        if (Input.GetMouseButton(0))
            FillInSelectedLetters();
    }

    public void ReturnToMenu()
    {
        DataController.Instance.SetStandby();
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            SumScore();
        //    WantRandomLetters = !WantRandomLetters;

        //if (Input.GetKeyDown(KeyCode.Escape))
        //    OpenOrCloseMenu();

        SelectLettersOnBoard();

        if (multiplierTimer > 0)
            multiplierTimer -= Time.deltaTime;
        else
        {
            if(scoreMultiplier > 0)
                SumScore();
            multiplierTimer = 0;
        }
    }

    void AddScore()
    {
        scoreMultiplier++;
        scoreToAdd = (scoreMultiplier * scorePerWord) * scoreMultiplier;
    }

    void SumScore()
    {
        score += scoreToAdd;
        scoreToAdd = 0;
        scoreMultiplier = 0;
    }

    int GetFinalScore()
    {
        int currentScore = score - Mathf.FloorToInt(gameTimer);
        return currentScore;
    }
}
