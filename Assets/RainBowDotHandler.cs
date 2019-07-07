using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainBowDotHandler : CircleHandler
{
    private Text RainBowDotTimerText;
    private GameObject tempCanvas;
    private Font arial;
    private GameObject RainBowDotTimer;
    private RectTransform rectTransform;
    private float intervalTime = 1;
    private float RainBowDotTotalTime = 5;
    public bool isTouched = false;
    
    // Start is called before the first frame update
    public override void OnMouseDown()
    {
        Vector2 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       
        board.DragRainBowDot = true;

        tempCanvas = GameObject.Find("BoardManager/Canvas");
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        RainBowDotTimer = new GameObject();
        RainBowDotTimer.transform.parent = tempCanvas.transform;
        RainBowDotTimer.name = "RainbowDot Timer";

        RainBowDotTimer.AddComponent<Text>();
        RainBowDotTimerText = RainBowDotTimer.GetComponent<Text>();

        RainBowDotTimerText.fontSize = 80;
        RainBowDotTimerText.font = arial;
        RainBowDotTimerText.fontStyle = FontStyle.Bold;
        RainBowDotTimerText.alignment = TextAnchor.MiddleCenter;
        RainBowDotTimerText.color = Color.white;

        rectTransform = RainBowDotTimerText.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector2(-10, 550);
        rectTransform.sizeDelta = new Vector2(600, 200);
    }
    public override void OnMouseDrag()
    {
        Vector2 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragPosition = new Vector2(Mathf.Clamp(dragPosition.x, board.xMin, board.xMax), Mathf.Clamp(dragPosition.y, board.yMin, board.yMax));
        transform.position = new Vector2(dragPosition.x, dragPosition.y);
        board.DragRainBowDot = true;

        RainBowDotTimerText.text = string.Format("{0:D2}:{1:D2}", (int)RainBowDotTotalTime / 60, (int)RainBowDotTotalTime % 60);
        if (RainBowDotTotalTime > 0)
        {
            intervalTime -= Time.deltaTime;

            if (intervalTime <= 0)
            {
                intervalTime += 1;
                RainBowDotTotalTime--;

                RainBowDotTimerText.text = string.Format("{0:D2}:{1:D2}", (int)RainBowDotTotalTime / 60, (int)RainBowDotTotalTime % 60);
            }
        }
        else
        {
            OnMouseUp();
        }
        

    }
    public override void OnMouseUp()
    {

        transform.position = this.board.tilesPos[col, row];
        board.DragRainBowDot = false;
        Destroy(RainBowDotTimer);

        if (scene.EnableRainBowDot == false && scene.EnableMagicDot == true)//magic dot
        {
            if (scene.ClearColRow==false)
            {
                board.ReplaceRainbowDot(col, row);
            }
            else
            {
                List<(int, int)> temp = new List<(int, int)>();

                board.CheckRainbowDotConnected(col, row, temp);

                if (temp.Count > 1)
                {

                    CleanRainbowDot(temp);
                    List<List<(int, int)>> list = null;
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
                    board.RefillBoard();
                    return;
                }
                else
                {

                    board.ReplaceRainbowDot(col, row);
                }
            }
            //List<(int, int)> temp = new List<(int, int)>();
            
            //board.CheckRainbowDotConnected(col, row, temp);
            
            //if (temp.Count > 1)
            //{
               
            //    CleanRainbowDot(temp);        
            //    List<List<(int, int)>> list = null;
            //    while (true)
            //    {
            //        board.DropDots();
            //        list = board.CheckRainbowDotList();
                    
            //        if (list.Count > 0)
            //        {
                       
            //            board.AutoMatchRainbowDot(list);
            //        }
            //        else
            //        {
            //            break;
            //        }

            //    }
            //    board.RefillBoard();
            //    return;
            //}
            //else
            //{
            
            //    board.ReplaceRainbowDot(col, row);
            //}
        }

        if (scene.EnableRainBowDot == true && scene.EnableMagicDot == false)//rainbow dot
        {
            
            board.ReplaceRainbowDot(col, row);
            
        }
  
    }
    public void DropDot()
    {
        board = FindObjectOfType<BoardManager>();
        transform.position = this.board.tilesPos[col, row];
    }
    private void OnTriggerEnter2D(Collider2D oDot)
    {
        
        int colorTemp = board.board[oDot.GetComponent<BackgroundTile>().col, oDot.GetComponent<BackgroundTile>().row].color;
        GameObject otherDot = board.board[oDot.GetComponent<BackgroundTile>().col, oDot.GetComponent<BackgroundTile>().row].circleObject;
        board.board[oDot.GetComponent<BackgroundTile>().col, oDot.GetComponent<BackgroundTile>().row].circleObject = this.gameObject;
        board.board[oDot.GetComponent<BackgroundTile>().col, oDot.GetComponent<BackgroundTile>().row].color = board.board[col, row].color;
        board.board[col, row].circleObject = otherDot;
        board.board[col, row].color = colorTemp;
        otherDot.GetComponent<CircleHandler>().row = row;
        otherDot.GetComponent<CircleHandler>().col = col;
        otherDot.GetComponent<CircleHandler>().SwapCircle();
        row = oDot.GetComponent<BackgroundTile>().row;
        col = oDot.GetComponent<BackgroundTile>().col;
        

    }
 
}
