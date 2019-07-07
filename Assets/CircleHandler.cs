using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleHandler : MonoBehaviour
{
    private Vector2 dropTarget = Vector2.zero;
    private Vector2 touchPos;

    public int row;
    public int col;
    public BoardManager board;
    public List<(int, int, Vector2)> selection;
    private bool isClosedLoop = false;
    public int currentLineColor;
    public bool isRainbowDot = false;
    public Scene scene;

    // Start is called before the first frame update
    public virtual void Start()
    {
        board = FindObjectOfType<BoardManager>();
        selection = new List<(int, int, Vector2)>();
        scene = FindObjectOfType<Scene>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnMouseDown()
    {
        HandleSelection(col, row, board.board[col, row].color);
    }

    public virtual void OnMouseDrag()
    {
        Vector2 dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        HandleSelection(Mathf.RoundToInt(dragPos.x), Mathf.RoundToInt(dragPos.y + 3), currentLineColor);

    }

    public virtual void OnMouseUp()
    {
        SpriteRenderer renderer;
        if (selection.Count > 1)
        {
            switch (currentLineColor)
            {
                case 0:

                    scene.BinA = scene.BinA - selection.Count();
                    if (scene.BinA <= 0)
                    {
                        scene.BinA = 0;
                        board.BinAEmpty = 1;
                    }

                    break;
                case 1:

                    scene.BinB = scene.BinB - selection.Count();
                    if (scene.BinB <= 0)
                    {
                        scene.BinB = 0;
                        board.BinBEmpty = 1;
                    }

                    break;
                case 2:

                    scene.BinC = scene.BinC - selection.Count();
                    if (scene.BinC <= 0)
                    {
                        scene.BinC = 0;
                        board.BinCEmpty = 1;
                    }

                    break;
                case 3:

                    scene.BinD = scene.BinD - selection.Count();
                    if (scene.BinD <= 0)
                    {
                        scene.BinD = 0;
                        board.BinDEmpty = 1;
                    }

                    break;
                case 4:

                    scene.BinE = scene.BinE - selection.Count();
                    if (scene.BinE <= 0)
                    {
                        scene.BinE = 0;
                        board.BinEEmpty = 1;
                    }

                    break;
            }

            foreach ((int, int, Vector2) circle in selection)
            {
                renderer = board.backgroundTiles[circle.Item1, circle.Item2].GetComponent<SpriteRenderer>();
                renderer.color = new Color(0, 0, 0);
                if (board.board[circle.Item1, circle.Item2].circleObject != null
                    && board.board[circle.Item1, circle.Item2].circleObject.GetComponent<RainBowDotHandler>() != null)
                {
                    board.rainbowDotList.Remove(board.board[circle.Item1, circle.Item2].circleObject);
                }

                Destroy(board.board[circle.Item1, circle.Item2].circleObject);
                board.board[circle.Item1, circle.Item2].circleObject = null;

            }

            if (isClosedLoop && scene.EnableCloseLoop)
            {

               
                int rainbowCol = selection[selection.Count - 1].Item1;
                int rainbowRow = selection[selection.Count - 1].Item2;
                board.CreateRainbowDot(rainbowCol, rainbowRow);
                board.board[rainbowCol, rainbowRow].circleObject.GetComponent<RainBowDotHandler>().col = rainbowCol;
                board.board[rainbowCol, rainbowRow].circleObject.GetComponent<RainBowDotHandler>().row = rainbowRow;

                if (scene.EnableRainBowDot == false && scene.EnableMagicDot == true)//magic dot
                {

                    board.board[rainbowCol, rainbowRow].circleObject.GetComponent<RainBowDotHandler>().isRainbowDot = true;
                    List<(int, int)> temp = new List<(int, int)>();


                    if (scene.ClearColRow == true)
                    {
                        board.CheckRainbowDotConnected(rainbowCol, rainbowRow, temp);
                        if (temp.Count > 1)
                        {
                            CleanRainbowDot(temp);
                        }
                        else
                        {
                            board.board[rainbowCol, rainbowRow].circleObject.GetComponent<RainBowDotHandler>().isRainbowDot = true;
                            board.board[rainbowCol, rainbowRow].circleObject.GetComponent<RainBowDotHandler>().isTouched = false;
                            temp.Clear();
                        }
                    }

                }

            }
            selection.Clear();
            List<List<(int, int)>> list = null;

            board.ClearInvalidDot();

            if (scene.ClearColRow == false)
            {
                board.DropDots();
            }
            else
            {
                while (true)
                {
                    board.DropDots();

                    list = board.CheckRainbowDotList();
                    if (list.Count > 0)
                    {

                        board.AutoMatchRainbowDot(list);
                    }
                    else
                    {
                        break;
                    }

                }
            }


            //add sound
            if(scene.EnableCloseLoop && isClosedLoop)
            {
                board.CloseLoopSound.Play(0);
            }
            else
            {
                board.MatchingSound.Play(0);
            }

            board.RefillBoard();
        }

        else
        {
            renderer = board.backgroundTiles[col, row].GetComponent<SpriteRenderer>();
            renderer.color = new Color(0, 0, 0);
            selection.Clear();
        }


    }

    public virtual void SwapCircle()
    {
        transform.position = board.tilesPos[col, row];
    }

    private void HandleSelection(int col, int row, int color)
    {
        if (selection.Count == 0)
        {

            currentLineColor = color;
            selection.Add((col, row, new Vector2(0, 0)));
            SpriteRenderer renderer = board.backgroundTiles[col, row].GetComponent<SpriteRenderer>();
            renderer.color = new Color(255, 255, 255);
        }
        else
        {
            if (currentLineColor == -1) currentLineColor = board.board[col, row].color;
            if ((board.board[col, row].color == currentLineColor || board.board[col, row].color == -1) && isAdjacent(selection, col, row))
            {
                SpriteRenderer renderer;
                int preCol = selection[selection.Count - 1].Item1;
                int preRow = selection[selection.Count - 1].Item2;
                Vector2 curDiff = DiffHelper(col, row, preCol, preRow);
                Vector2 preVector = selection[selection.Count - 1].Item3;
                if (curDiff.x + preVector.x == 0 && curDiff.y + preVector.y == 0)
                {
                    selection.RemoveAt(selection.Count - 1);
                    if (selection.Count == 1) currentLineColor = board.board[selection[0].Item1, selection[0].Item2].color;
                    renderer = board.backgroundTiles[preCol, preRow].GetComponent<SpriteRenderer>();
                    renderer.color = new Color(0, 0, 0);
                }
                else
                {
                    isClosedLoop = isClosedLoop ? true : board.CheckClosedLoop(selection, col, row);
                    selection.Add((col, row, curDiff));
                    renderer = board.backgroundTiles[col, row].GetComponent<SpriteRenderer>();
                    renderer.color = new Color(255, 255, 255);

                }

            }

        }
    }
    private Vector2 DiffHelper(int curCol, int curRow, int preCol, int preRow)
    {
        int diffY = curRow - preRow;
        int diffX = curCol - preCol;
        return new Vector2(diffX, diffY);
    }
    public bool isAdjacent(List<(int, int, Vector2)> selection, int col, int row)
    {
        if (selection[selection.Count - 1].Item1 == col + 1 && selection[selection.Count - 1].Item2 == row)
        {
            return true;
        }
        else if (selection[selection.Count - 1].Item1 == col - 1 && selection[selection.Count - 1].Item2 == row)
        {
            return true;
        }
        else if (selection[selection.Count - 1].Item1 == col && selection[selection.Count - 1].Item2 == row - 1)
        {
            return true;
        }
        else if (selection[selection.Count - 1].Item1 == col && selection[selection.Count - 1].Item2 == row + 1)
        {
            return true;
        }

        else { return false; }
    }

    public void InitDrop(int dropDistance)
    {
        dropTarget = new Vector2(transform.position.x, transform.position.y - dropDistance);
        StartCoroutine(DropCircle());
    }

    public IEnumerator DropCircle()
    {
        WaitForSeconds frameTime = new WaitForSeconds(0.01f);
        Vector2 startPos = transform.position;
        float lerpPercent = 0;

        while (lerpPercent <= 1)
        {
            transform.position = Vector2.Lerp(startPos, dropTarget, lerpPercent);
            lerpPercent += 0.05f;
            yield return frameTime;
        }

        transform.position = dropTarget;
    }
    public void CleanRainbowDot(List<(int, int)> list)
    {
        foreach ((int, int) dot in list)
        {
            board.HelperAutoMatchRainbowDot(dot.Item1, dot.Item2);
            board.rainbowDotList.Remove(board.board[dot.Item1, dot.Item2].circleObject);
            Destroy(board.board[dot.Item1, dot.Item2].circleObject);
            board.board[dot.Item1, dot.Item2].circleObject = null;
        }
    }


    /*void OnMouseOver()
    {
        touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(touchPos);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("HIA THERE");
        }
        Debug.Log("HELLO");
    }*/
}
