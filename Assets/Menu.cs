using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] Text loseText;
    Game game;

    private void Start()
    {
        game = FindObjectOfType<Game>();
    }

    public void LetAiStart()
    {
        if(game != null)
        {
            game.AIStartMove();
            if(startMenu)
            startMenu.gameObject.SetActive(false);
        }
    }

    public void YouStart()
    {
        if(game)
        game.Restart();
        if(startMenu)
        startMenu.gameObject.SetActive(false);
    }

    public void ShowLoseScreen(int value)
    {
        if(value > 0)
        {
            loseText.text = "AI WINS";
        }

        if(value < 0)
        {
            loseText.text = "YOU WIN";
        }

        if (value == 0)
        {
            loseText.text = "IT'S A TIE";
        }
        loseMenu.SetActive(true);
    }

    public void ShowStartMenu()
    {
        if(startMenu)
        startMenu.SetActive(true);
    }

    public void PlayAgain()
    {
        loseMenu.SetActive(false);
        ShowStartMenu();
        game.Restart();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
