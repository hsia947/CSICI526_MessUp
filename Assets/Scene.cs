using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public int level;
    public float totalTime;
    public int BinsCleared;
    public bool EnableRainBowDot;
    public bool EnableMagicDot;
    public int BinA;
    public int BinB;
    public int BinC;
    public int BinD;
    public int BinE;
    public int Score;
    public int BinAFix;
    public int BinBFix;
    public int BinCFix;
    public int BinDFix;
    public int BinEFix;
    public bool EnableBonusTime;
    public bool ClearColRow;
    public bool IsWin;
    public bool EnableCloseLoop;
    public bool EnableSound;
    public bool SoundVol;
    // Start is called before the first frame update
    void Start()
    {
       
        GameObject.DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
