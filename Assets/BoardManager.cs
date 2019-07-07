using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    //TODO
    //Programatically center camera

    public Text binAText;
    public Text binBText;
    public Text binCText;
    public Text binDText;
    public Text binEText;

    public Text ScoreText;

    public float totalTime;
    private float intervalTime = 1;

    public Text CountDownText;
    public bool DragRainBowDot;
  
    public int BinAEmpty;
    public int BinBEmpty;
    public int BinCEmpty;
    public int BinDEmpty;
    public int BinEEmpty;
    private int EmptyBins;
    
    public Scene scene;
    
    private bool IsPause;
    public Text PauseText;
    public Text LevelText;

    private int DoAddTimeA;
    private int DoAddTimeB;
    private int DoAddTimeC;
    private int DoAddTimeD;
    private int DoAddTimeE;

    private GameObject AddTimeText;
    private GameObject tempCanvas;
    public Button PauseButton;
    public Sprite ImagePause;
    public Sprite ImageUnpause;

    private float interval = 1;
    
    private int numBins = 5;
    private bool[] ShowTime;
    private Font arial;
    private GameObject tempBoardManager;
    private GameObject DotCanvas;
    private GameObject DotCanvasCleared;
    public List<GameObject> rainbowDotList;

    public struct Circle
    {
        public int color;
        public GameObject circleObject;
        //public CircleHandler CH;
        public int dropDistance;
        public int matchNumber;
    }
    public GameObject rainBowDot;
    public GameObject tilePreFab;
    public int width;
    public int height;
    public int offset = 5;
    public Circle[,] board;
    public Vector2[,] tilesPos;
    public GameObject[,] backgroundTiles;
    private Circle circleClone = new Circle();
    private int randomX;
    private int randomY;
    public int[] dropDistance;
    public float xMin;
    public float yMin;
    public float xMax;
    public float yMax;
    [SerializeField] private Material cRed;
    [SerializeField] private Material cYellow;
    [SerializeField] private Material cGreen;
    [SerializeField] private Material cBlue;
    [SerializeField] private Material cPurple;
    [SerializeField] private Material cWhite;

    public Sprite ImageRedSqure;
    public Sprite ImageGreenUp;
    public Sprite ImagePurpleDown;
    public Sprite ImageYellowLeft;
    public Sprite ImageBlueRight;

    public Sprite ImageDiamand;
    List<int> BinZeroList = new List<int>();
    public AudioSource MatchingSound;
    public AudioSource CloseLoopSound;

    // Start is called before the first frame update
    void Start()
    {
        board = new Circle[height, width];
        tilesPos = new Vector2[height, width];
        backgroundTiles = new GameObject[height, width];
        dropDistance = new int[width];
        randomX = Random.Range(0, width);
        randomY = Random.Range(0, height);
        rainbowDotList = new List<GameObject>();
       
        scene = FindObjectOfType<Scene>();
       
        totalTime = scene.totalTime;

        CountDownText.text = string.Format("{0:D2}:{1:D2}", (int)totalTime / 60, (int)totalTime % 60);
        //CountUpText.text = string.Format("{0:D2}:{1:D2}", (int)totalTime2 / 60, (int)totalTime2 % 60);

        scene.BinA = scene.BinAFix;
        scene.BinB = scene.BinBFix;
        scene.BinC = scene.BinCFix;
        scene.BinD = scene.BinDFix;
        scene.BinE = scene.BinEFix;
        DragRainBowDot = false;
        BinAEmpty = 0;
        BinBEmpty = 0;
        BinCEmpty = 0;
        BinDEmpty = 0;
        BinEEmpty = 0;

        IsPause = false;
        LevelText.text = "Level " + scene.level;
        
        DoAddTimeA = 2;
        DoAddTimeB = 2;
        DoAddTimeC = 2;
        DoAddTimeD = 2;
        DoAddTimeE = 2;


        ShowTime = new bool[numBins];
        //ShowTime = false;
        for (int i = 0; i < numBins; i++)
        {
            ShowTime[i] = false;
        }
        
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        tempBoardManager = GameObject.Find("BoardManager");
        tempCanvas = GameObject.Find("BoardManager/Canvas");

        DotCanvas = new GameObject();
        DotCanvas.name = "DotCanvas";
        DotCanvas.AddComponent<Canvas>();
        DotCanvas.AddComponent<CanvasScaler>();
        DotCanvas.AddComponent<GraphicRaycaster>();
        DotCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        DotCanvas.transform.parent = tempBoardManager.transform;

        DotCanvasCleared = new GameObject();
        DotCanvasCleared.name = "DotCanvasCleared";
        DotCanvasCleared.AddComponent<Canvas>();
        DotCanvasCleared.AddComponent<CanvasScaler>();
        DotCanvasCleared.AddComponent<GraphicRaycaster>();
        DotCanvasCleared.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        DotCanvasCleared.transform.parent = DotCanvas.transform;



        InitBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
        binAText.text = scene.BinA.ToString();
        binBText.text = scene.BinB.ToString();
        binCText.text = scene.BinC.ToString();
        binDText.text = scene.BinD.ToString();
        binEText.text = scene.BinE.ToString();

        scene.Score = 30*5-(scene.BinA + scene.BinB + scene.BinC + scene.BinD + scene.BinE);
        ScoreText.text = "Score: "+scene.Score.ToString();

        EmptyBins = BinAEmpty + BinBEmpty + BinCEmpty + BinDEmpty + BinEEmpty;



        if (scene.EnableBonusTime==true)
        {
            DoAddTimeA = AddBonusTime(scene.BinA, DoAddTimeA, 0);
            DoAddTimeB = AddBonusTime(scene.BinB, DoAddTimeB, 1);
            DoAddTimeC = AddBonusTime(scene.BinC, DoAddTimeC, 2);
            DoAddTimeD = AddBonusTime(scene.BinD, DoAddTimeD, 3);
            DoAddTimeE = AddBonusTime(scene.BinE, DoAddTimeE, 4);
        }
        

        if (EmptyBins >= scene.BinsCleared)
        {
            scene.IsWin = true;
            SceneManager.LoadScene("Result");
        }


        //count down timer
        if (totalTime > 0 && DragRainBowDot==false && IsPause==false)
        {
            intervalTime -= Time.deltaTime;
            //ShowBonusTime();

            interval -= Time.deltaTime;
            /////////////////////////////
            if (scene.EnableBonusTime==true)
            {
                for (int i = 0; i < numBins; i++)
                {
                    if (interval > 0 && ShowTime[i] == true)
                    {
                        if (AddTimeText == null)
                        {
                            ShowBonusTime(i);
                        }

                        if (interval != 5)
                        {
                            interval = 5;
                        }
                        ShowTime[i] = false;

                    }
                    if (interval <= 0 && ShowTime[i] == false)
                    {
                        interval += 5;
                        Destroy(AddTimeText);
                    }
                }
            }
            

        /////////////////////////////////////
            if (intervalTime <= 0)
            {
                intervalTime += 1;
                totalTime--;

                CountDownText.text = string.Format("{0:D2}:{1:D2}", (int)totalTime / 60, (int)totalTime % 60);
            }
        }

        if (totalTime == 0)
        {

            totalTime = -1;
            scene.IsWin = false;
            SceneManager.LoadScene("Result");

        }

    }
    public void ClearInvalidDot()//when bin hit 0
    {
        BinZeroList.Clear();

        if (BinAEmpty == 1)
        {
            BinZeroList.Add(0);
        }
        if (BinBEmpty == 1)
        {
            BinZeroList.Add(1);
        }
        if (BinCEmpty == 1)
        {
            BinZeroList.Add(2);
        }
        if (BinDEmpty == 1)
        {
            BinZeroList.Add(3);
        }
        if (BinEEmpty == 1)
        {
            BinZeroList.Add(4);
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                foreach (int item in BinZeroList)
                {
                    Debug.Log(item);
                    if (board[x, y].color == item)
                    {
                        Destroy(board[x, y].circleObject);
                        board[x, y].circleObject = null;
                    }
                }

            }
        }
       
    }

    public void ShowBonusTime(int i)
    {

        AddTimeText = new GameObject();
        AddTimeText.transform.parent = tempCanvas.transform;
        AddTimeText.name = "Show Bonus Time";
        AddTimeText.AddComponent<Text>();
        AddTimeText.AddComponent<RectTransform>();
        AddTimeText.GetComponent<Text>().text = "Bonus Time 5s";    
        AddTimeText.GetComponent<Text>().fontSize = 60;
        AddTimeText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        AddTimeText.GetComponent<Text>().font = arial;
        AddTimeText.GetComponent<RectTransform>().localPosition = new Vector2(0, 800);
        AddTimeText.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 200);
        if (i == 0)
        {
            AddTimeText.GetComponent<Text>().color = Color.red;
        }
        if (i == 1)
        {
            AddTimeText.GetComponent<Text>().color = Color.blue;
        }
        if (i == 2)
        {
            AddTimeText.GetComponent<Text>().color = Color.green;
        }
        if (i == 3)
        {
            AddTimeText.GetComponent<Text>().color = Color.yellow;
        }
        if (i == 4)
        {
            AddTimeText.GetComponent<Text>().color = Color.magenta;
        }

    }

    public int AddBonusTime(int BinX, int DoAddTimeX, int i)
    {
        if (BinX<=15 && BinX>0 && DoAddTimeX==2)
        {
            totalTime += 5;

            ShowTime[i] = true;
            DoAddTimeX--;
            return DoAddTimeX;
         
        }
        else if (BinX==0 && DoAddTimeX==1)
        {
            totalTime += 5;

            ShowTime[i] = true;
            DoAddTimeX--;
            return DoAddTimeX;
        }
        else
        {
            return DoAddTimeX;
        }
     
    }

    public void EnterHomePage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home");
    }

    public void EnterMainPage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainPage");
    }

    public void Pause()
    {
        if (IsPause==false)
        {
            IsPause = true;
            //PauseText.text = "Unpause";
            PauseButton.image.sprite = ImageUnpause;
            
            DotCanvas.SetActive(false);
            Time.timeScale = 0f;

        }
        else
        {
            IsPause = false;
            //PauseText.text = "Pause";
            PauseButton.image.sprite = ImagePause;
            DotCanvas.SetActive(true);
            Time.timeScale = 1f;
        }
        
    }
    private void InitBoard()
    {
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {   
                
                SpawnCircle(x, y, height);
            }
        }
        SkyfallGems();
        xMin = tilesPos[0, 0].x;
        yMin = tilesPos[0, 0].y;
        xMax = tilesPos[width - 1, height - 1].x;
        yMax = tilesPos[width - 1, height - 1].y;
    }

    public void SpawnCircle(int x, int y, int drop)
    {
        GameObject circle;

        circle = (GameObject)Instantiate(Resources.Load("Circle"), new Vector2(x, y + offset), Quaternion.identity);
        GameObject backgroundTile = Instantiate(tilePreFab, new Vector2(x, y+offset-drop), Quaternion.identity) as GameObject;
        //////////////////////////////////
        circle.transform.parent = DotCanvas.transform;
        circle.name = "C( " + x + ", " + y + " )";
        backgroundTile.transform.parent = DotCanvas.transform;
        backgroundTile.name = "( " + x + ", " + y + " )";
        backgroundTile.GetComponent<BackgroundTile>().col = x;
        backgroundTile.GetComponent<BackgroundTile>().row = y;
        circle.GetComponent<CircleHandler>().col = x;
        circle.GetComponent<CircleHandler>().row = y;

        if (board[x, y].circleObject != null)
        {
            Destroy(board[x, y].circleObject);
        }
        backgroundTiles[x, y] = backgroundTile;
        tilesPos[x, y] = new Vector2(x, y + offset - drop);

        board[x, y] = new Circle
        {
            circleObject = circle,
            dropDistance = drop,
    
            matchNumber = 0
        };

        SetColor(x, y);
    }

    public void SpawnCircle2(int x, int y, int drop)
    {
        GameObject circle;
     
        circle = (GameObject)Instantiate(Resources.Load("Circle"), new Vector2(x, y + offset), Quaternion.identity);
    
        circle.transform.parent = DotCanvasCleared.transform;
        circle.name = "C( " + x + ", " + y + " )";
        circle.GetComponent<CircleHandler>().col = x;
        circle.GetComponent<CircleHandler>().row = y;

        if (board[x, y].circleObject != null)
        {
            Destroy(board[x, y].circleObject);
        }              
       
        board[x, y] = new Circle
        {
            circleObject = circle,
            dropDistance = drop,
     
            matchNumber = 0
        };

        SetColor(x, y);
    }
    public void CreateRainbowDot(int x, int y)
    {
        GameObject rainbowDot = Instantiate(rainBowDot, new Vector2(x, y-3), Quaternion.identity) as GameObject;
        rainbowDot.transform.parent = DotCanvasCleared.transform;
        rainbowDot.name = "RC( " + x + ", " + y + " )";
        board[x, y].circleObject = rainbowDot;

        if (scene.EnableRainBowDot==false && scene.EnableMagicDot==true)//magic dot
        {        
            board[x, y].circleObject.transform.GetComponent<Renderer>().material = cWhite;
            rainbowDotList.Add(board[x, y].circleObject);
            board[x, y].color = -1;

            board[x, y].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageDiamand;
        }
        if (scene.EnableRainBowDot == true && scene.EnableMagicDot == false)//rainbow dot
        {         
            SetColor(x, y);
            board[x, y].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageDiamand;
        }

    }

    private void SetColor(int x, int y)
    {

        int randomColor = Random.Range(0, 5);

        //List<int> ColorList = new List<int>();

        //if (BinAEmpty==0)
        //{
        //    ColorList.Add(0);
        //}

        //if (BinBEmpty == 0)
        //{
        //    ColorList.Add(1);
        //}

        //if (BinCEmpty == 0)
        //{
        //    ColorList.Add(2);
        //}

        //if (BinDEmpty == 0)
        //{
        //    ColorList.Add(3);
        //}

        //if (BinEEmpty == 0)
        //{
        //    ColorList.Add(4);
        //}

        //int RandomIndex = Random.Range(0, ColorList.Count);//get a random index from color list

        //int randomColor = ColorList[RandomIndex];


        switch (randomColor)
        {
            case 0: // fire
                board[x, y].circleObject.transform.GetComponent<Renderer>().material = cRed;
                board[x, y].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageRedSqure;
                break;
            case 1: // water
                board[x, y].circleObject.transform.GetComponent<Renderer>().material = cBlue;
                board[x, y].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageBlueRight;
                break;
            case 2: // wood
                board[x, y].circleObject.transform.GetComponent<Renderer>().material = cGreen;
                board[x, y].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageGreenUp;
                break;
            case 3: // light
                board[x, y].circleObject.transform.GetComponent<Renderer>().material = cYellow;
                board[x, y].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageYellowLeft;
                break;
            case 4: // dark
                board[x, y].circleObject.transform.GetComponent<Renderer>().material = cPurple;
                board[x, y].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImagePurpleDown;
                break;
        }
        board[x, y].color = randomColor;
    }

    public void SkyfallGems()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y].dropDistance > 0)
                {            
                    board[x, y].circleObject.GetComponent<CircleHandler>().InitDrop(board[x, y].dropDistance);                 
                    board[x, y].dropDistance = 0;
                }
            }
        }
    }
    public bool CheckClosedLoop(List<(int, int, Vector2)> selection, int addCol, int addRow)
    {
        int colComp = 0;
        int rowComp = 0;
        for (int i = 0; i < selection.Count - 1; i++)
        {
            colComp = selection[i].Item1;
            rowComp = selection[i].Item2;
            if (CheckAdjacent(addCol, addRow, colComp, rowComp))
            {
                return true;
            }

        }
        return false;
    }
    public bool CheckAdjacent(int colLast, int rowLast, int colComp, int rowComp)
    {
        if (colLast == colComp + 1 && rowLast == rowComp)
        {
            return true;
        }
        else if (colLast == colComp - 1 && rowLast == rowComp)
        {
            return true;
        }
        else if (colLast == colComp && rowLast == rowComp - 1)
        {
            return true;
        }
        else if (colLast == colComp && rowLast == rowComp + 1)
        {
            return true;
        }

        else { return false; }

    }

    public void DropDots()
    {

        int count = 0;
        for (int col = 0; col < width; col++)
        {

            count = 0;
            for (int row = 0; row < height; row++)
            {
                if (board[col, row].circleObject == null)
                {
                    count++;
                }
                else if (count > 0)
                {

                    board[col, row - count].circleObject = board[col, row].circleObject;
                    board[col, row - count].color = board[col, row].color;

                    if (board[col, row].circleObject.GetComponent<RainBowDotHandler>() == null)
                    {
                       
                        board[col, row].circleObject.GetComponent<CircleHandler>().row = row - count;
                        board[col, row].circleObject.GetComponent<CircleHandler>().SwapCircle();
                    }
                    else
                    {
                       
                        board[col, row].circleObject.GetComponent<RainBowDotHandler>().row = row - count;
                     
                        board[col, row].circleObject.GetComponent<RainBowDotHandler>().DropDot();
                    }
                    board[col, row].circleObject = null;
                    board[col, row].color = 0;
                }
            }
            dropDistance[col] = count;
        }

    }

    public void RefillBoard()
    {
        
        for (int x = 0; x < dropDistance.Length; x++)
        {
            int curRow = 0;

            for (int y = 0; y < dropDistance[x]; y++)
            {
                SpawnCircle2(x, height - curRow - 1, height);
                curRow++;
            }

        }
        SkyfallGems();
    }
    public void ReplaceRainbowDot(int col, int row)
    {
        Material temp = board[col, row].circleObject.transform.GetComponent<Renderer>().material;

        int GetColor = board[col, row].color;

        rainbowDotList.Remove(board[col, row].circleObject);
        Destroy(board[col, row].circleObject);
        board[col, row].circleObject = null;
        GameObject rDot = Instantiate(Resources.Load("Circle"), new Vector2(col, row - 3), Quaternion.identity) as GameObject;
        
        rDot.transform.parent = DotCanvasCleared.transform;
        rDot.name = "( " + col + ", " + row + " )";
        rDot.GetComponent<CircleHandler>().col = col;
        rDot.GetComponent<CircleHandler>().row = row;
        board[col,row].circleObject = rDot;
        board[col, row].circleObject.transform.GetComponent<Renderer>().material = temp;

        if (GetColor==0)
        {
            board[col, row].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageRedSqure;

        }
        else if (GetColor==3)
        {
            board[col, row].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageYellowLeft;
        }
        else if (GetColor==1)
        {
            board[col, row].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageBlueRight;
        }
        else if (GetColor==4)
        {
            board[col, row].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImagePurpleDown;
        }
        else if (GetColor==2)
        {
            board[col, row].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageGreenUp;
        }
        else//color=-1, white
        {
            board[col, row].circleObject.transform.GetComponent<SpriteRenderer>().sprite = ImageDiamand;
        }
       

    }
    public void CheckRainbowDotConnected(int col, int row, List<(int, int)> temp)
    {
        if (col >= width || col < 0 || row >= height || row < 0) return;
        if (board[col, row].circleObject == null || board[col, row].circleObject.GetComponent<RainBowDotHandler>() == null) return;
        if (board[col, row].circleObject.GetComponent<RainBowDotHandler>().isRainbowDot == true)
        {
            board[col, row].circleObject.GetComponent<RainBowDotHandler>().isRainbowDot = false;
            board[col, row].circleObject.GetComponent<RainBowDotHandler>().isTouched = true;
            temp.Add((col, row));
            CheckRainbowDotConnected(col + 1, row, temp);
            CheckRainbowDotConnected(col - 1, row, temp);
            CheckRainbowDotConnected(col, row + 1, temp);
            CheckRainbowDotConnected(col, row - 1, temp);
        }
    }
    public List<List<(int,int)>> CheckRainbowDotList()
    {   List < List<(int, int)>> res = new List< List<(int, int)>>();
        if (rainbowDotList.Count == 0) return res;
        foreach (GameObject g in rainbowDotList)
        {
            List<(int, int)> t = new List<(int, int)>();
            CheckRainbowDotConnected(g.GetComponent<RainBowDotHandler>().col, g.GetComponent<RainBowDotHandler>().row, t);
            if (t.Count > 1)
            {
               res.Add(t);
     
            }
            else
            {
                g.GetComponent<RainBowDotHandler>().isRainbowDot = true;
                g.GetComponent<RainBowDotHandler>().isTouched = false;
            }
        }
        return res;
    }
    public void AutoMatchRainbowDot(List<List<(int, int)>> list)
    {
        foreach(List<(int,int)> line in list){
            foreach ((int, int) dot in line)
            {
                HelperAutoMatchRainbowDot(dot.Item1, dot.Item2);
                rainbowDotList.Remove(board[dot.Item1, dot.Item2].circleObject);
            
                Destroy(board[dot.Item1, dot.Item2].circleObject);
                board[dot.Item1, dot.Item2].circleObject = null;
            }
        }
    }
    public void HelperAutoMatchRainbowDot(int col, int row)
    {
        for (int i =0; i <　width; i++)
        {
            if((board[col, i].circleObject!=null &&board[col, i].circleObject.GetComponent<RainBowDotHandler>() == null)
                ||(board[col, i].circleObject != null && board[col, i].circleObject.GetComponent<RainBowDotHandler>() != null 
                && board[col, i].circleObject.GetComponent<RainBowDotHandler>().isTouched==false))
            {
                if (board[col,i].circleObject.GetComponent<RainBowDotHandler>()!=null && board[col,i].circleObject.GetComponent<RainBowDotHandler>().isTouched==false)
                {
                    rainbowDotList.Remove(board[col, i].circleObject);
                }
                
                SetBinRowCol(col, i);
                Destroy(board[col, i].circleObject);
                board[col, i].circleObject = null;
            }

            if ((board[i, row].circleObject != null && board[i, row].circleObject.GetComponent<RainBowDotHandler>() == null)
                || (board[i, row].circleObject != null && board[i, row].circleObject.GetComponent<RainBowDotHandler>() != null
                && board[i, row].circleObject.GetComponent<RainBowDotHandler>().isTouched == false))
            {
                if (board[i, row].circleObject.GetComponent<RainBowDotHandler>() != null && board[i, row].circleObject.GetComponent<RainBowDotHandler>().isTouched == false)
                {
                    rainbowDotList.Remove(board[i, row].circleObject);
                }
                
                SetBinRowCol(i, row);
                Destroy(board[i, row].circleObject);
                board[i, row].circleObject = null;
            }
        }

        
    }
    public void SetBinRowCol(int col, int row)
    {
        if (board[col, row].color == 0)
        {
            scene.BinA--;
            if (scene.BinA <= 0)
            {
                scene.BinA = 0;
                BinAEmpty = 1;
            }
        }
        else if (board[col, row].color == 1)
        {
            scene.BinB--;
            if (scene.BinB <= 0)
            {
                scene.BinB = 0;
                BinBEmpty = 1;
            }
        }
        else if (board[col, row].color == 2)
        {
            scene.BinC--;
            if (scene.BinC <= 0)
            {
                scene.BinC = 0;
                BinCEmpty = 1;
            }
        }
        else if (board[col, row].color == 3)
        {
            scene.BinD--;
            if (scene.BinD <= 0)
            {
                scene.BinD = 0;
                BinDEmpty = 1;
            }
        }
        else if (board[col, row].color == 4)
        {
            scene.BinE--;
            if (scene.BinE <= 0)
            {
                scene.BinE = 0;
                BinEEmpty = 1;
            }
        }
    }
    /*public void SetBinCountText()
    {
        binAText.text = "Count: " + binA.ToString();
        binBText.text = "Count: " + binB.ToString();
        binCText.text = "Count: " + binC.ToString();
        binDText.text = "Count: " + binD.ToString();
        binEText.text = "Count: " + binE.ToString();
    }*/
}
