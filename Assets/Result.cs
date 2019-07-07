using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    public Text ResultText;
    public Scene scene;
    public Text ScoreText;
    public AudioSource WinSound;
    public AudioSource LoseSound;
    // Start is called before the first frame update
    void Start()
    {
        scene = FindObjectOfType<Scene>();
        
        ScoreText.text = "Score: "+scene.Score.ToString();
        if (scene.IsWin == true)
        {
            ResultText.text = "You win level " + scene.level;
            
            if (scene.EnableSound)
            {
                WinSound.Play(0);
            }

        }
        else
        {
            ResultText.text = "You lose level " + scene.level;
            
            if (scene.EnableSound)
            {
                LoseSound.Play(0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterHomePage()
    {

        SceneManager.LoadScene("Home");
    }

    public void EnterMainPage()
    {
        SceneManager.LoadScene("MainPage");
    }
}
