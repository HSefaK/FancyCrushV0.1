using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    public Text scoreText;
    public Text matchText;
    public Image starMeter;

    [Header("Win Panel")]
    public GameObject winPanel;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public Text winScoreText;

    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        //updateScoreText(0);
        //updateMatchText(30);
        winPanel.SetActive(false);
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
    }
    public void updateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString("D6");
        updateStarMeter(score);
    }

    public void updateMatchText(int matches)
    {
        matchText.text = "Mathces Left: " + matches.ToString("D2");
    }

    void updateStarMeter(int score)
    {
        float currentScore =(float) score / (float)GameManager.instance.maxScore;
        starMeter.rectTransform.localScale = new Vector3(starMeter.rectTransform.localScale.x,currentScore,starMeter.rectTransform.localScale.z);
    }

    public void activateWinPanel()
    {
        Board.instance.stopGame();
        winPanel.SetActive(true);
        winScoreText.text = "Score: " + GameManager.instance.readScore().ToString("D5");
        StartCoroutine(activateStars());
    }

    IEnumerator activateStars()
    {
        
        if(GameManager.instance.readScore() > (float)(GameManager.instance.maxScore * 0.33f))
        {
            star1.SetActive(true);
        }
        yield return new WaitForSeconds(0.25f);
        if (GameManager.instance.readScore() > (float)(GameManager.instance.maxScore * 0.66f))
        {
            star2.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        if (GameManager.instance.readScore() > (float)(GameManager.instance.maxScore * 1f))
        {
            star3.SetActive(true);
        }
        yield return new WaitForSeconds(1f);

    }
}
