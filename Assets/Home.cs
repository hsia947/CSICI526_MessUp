using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public Scene scene;
    public GameObject SettingPanel;
    public Sprite EnableSoundImg;
    public Sprite DisableSoundImg;
    public int page;
    public AudioSource MeunSound;
    public Button SoundButton;

    // Start is called before the first frame update
    void Start()
    {
        scene = FindObjectOfType<Scene>();
        SettingPanel = GameObject.Find("Canvas/SettingPanel");
        

        SettingPanel.SetActive(false);
      
        page = 1;
        scene.EnableSound = true;
        //MeunSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PopSetting()
    {
        if (scene.EnableSound)
        {
            MeunSound.Play(0);
        }
        SettingPanel.SetActive(true);
        

    }
    public void QuitSetting()
    {
        if (scene.EnableSound)
        {
            MeunSound.Play(0);
        }
        SettingPanel.SetActive(false);
    }
    
    public void SoundControl()
    {
        if (scene.EnableSound)
        {
            scene.EnableSound = false;
            SoundButton.image.sprite = EnableSoundImg;
        }
        else
        {
            scene.EnableSound = true;
            SoundButton.image.sprite = DisableSoundImg;
        }
    }

    public void EnterMainPage(int level)
    {
        if (scene.EnableSound)
        {
            MeunSound.Play(0);
        }
        scene.level = level;
        if (level == 1)
        {
            scene.totalTime = 120;
            scene.BinsCleared = 3;
            scene.EnableRainBowDot = false;
            scene.EnableMagicDot = false;
            scene.EnableBonusTime = false;
            scene.ClearColRow = false;
            scene.EnableCloseLoop = false;
            BinX();
        }
        if (level == 2)
        {
            scene.totalTime = 120;
            scene.BinsCleared = 5;
            scene.EnableRainBowDot = false;
            scene.EnableMagicDot = true;
            scene.EnableBonusTime = true;
            scene.ClearColRow = false;
            scene.EnableCloseLoop = true;
            BinX();
        }
        if (level == 3)
        {
            scene.totalTime = 90;
            scene.BinsCleared = 5;
            scene.EnableRainBowDot = false;
            scene.EnableMagicDot = true;//magic dot
            scene.EnableBonusTime = true;
            scene.ClearColRow = true;
            scene.EnableCloseLoop = true;
            BinX();
        }
        
        SceneManager.LoadScene("MainPage");

    }
    public void BinX()
    {
        scene.BinAFix = 30;
        scene.BinBFix = 30;
        scene.BinCFix = 30;
        scene.BinDFix = 30;
        scene.BinEFix = 30;
    }
}
