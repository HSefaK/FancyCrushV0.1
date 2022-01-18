using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    int score;
    public int matches = 4;
    public int maxScore = 10000;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        UIHandler.instance.updateMatchText(matches);
        UIHandler.instance.updateScoreText(score);
    }

    public void updateScore(int amount)
    {
        score += amount;
        UIHandler.instance.updateScoreText(score);
    }
    public void updateMatach()
    {
        matches--;
        UIHandler.instance.updateMatchText(matches);

        if(matches<= 0)
        {
            UIHandler.instance.activateWinPanel();
        }
    }

    public int readScore()
    {
        return score;
    }
}
