using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

class SimonSaysController : MonoBehaviour
{
    public GameObject[] Cells;
    public AudioSource loseSound;
    public float timeToChoose = 2;
    public TMPro.TextMeshProUGUI Score;
    public TMPro.TextMeshProUGUI Status;

    private SimonSays game;

    private bool playing;

    private float gameSpeed = 1;

    public void NewGame()
    {
        game = new SimonSays(Cells.Length);

        StartCoroutine(Game());
    }

    IEnumerator Game()
    {
        playing = true;
        gameSpeed = 1;

        yield return StartCoroutine(PlayInitialAnimation());

        // each loop is a new turn
        while (playing)
        {
            yield return new WaitForSeconds(1);

            Score.text = game.Sequence.Count.ToString();
            Status.text = "computer turn";

            game.AddTurn();
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(ComputerTurn());

            Status.text = "your turn";
            yield return StartCoroutine(PlayerTurn());
            
            gameSpeed += 0.1f;
        }

        loseSound.Play();
    }

    IEnumerator PlayInitialAnimation()
    {
        Cells[0].GetComponent<AudioSource>().Play();
        Cells[1].GetComponent<AudioSource>().Play();
        Cells[3].GetComponent<AudioSource>().Play();

        for (int i = 0; i < 3; i++)
        {
            foreach (GameObject cell in Cells)
            {
                yield return StartCoroutine(PlayCell(cell, 20f, playSound: false));
            }
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ComputerTurn()
    {
        foreach (int cell in game.Sequence)
        {
            yield return StartCoroutine(PlayCell(Cells[cell], gameSpeed));
        }
    }
    
    IEnumerator PlayCell(GameObject cell, float speed, bool playSound = true)
    {
        ColorChanger cellColor = cell.GetComponent<ColorChanger>();
        AudioSource cellSound = cell.GetComponent<AudioSource>();

        cellColor.ChangeColorToUnsaturated();
        yield return new WaitForSeconds(0.2f * (1 / speed));

        cellColor.ChangeColorToOriginal();
        
        if (playSound)
            cellSound.Play();

        yield return new WaitForSeconds(1 * (1/speed));

        cellColor.ChangeColorToUnsaturated();
        
        yield return new WaitForSeconds(0.5f * (1/speed));
    }

    IEnumerator PlayerTurn()
    {
        foreach (int currentCell in game.Sequence)
        {
            float remainingTime = timeToChoose;

            bool correctCell = false;

            // wait for user input until remaining time reaches zero
            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime; // update remaining time

                int playerInput = GetInputNumber();

                if (playerInput != -1) // if the input is valid
                {
                    // play correspondent cell
                    StartCoroutine(PlayCell(Cells[playerInput], 1.5f));

                    yield return new WaitForSeconds(0.1f);

                    // played the correct cell
                    if (playerInput == currentCell)
                    {
                        correctCell = true;
                    }
                    
                    break;    
                }
                
                yield return null; // wait for a new frame
            }

            // here, the time runned out or the player played something
            if (!correctCell)
            {
                playing = false;
                break;
            }
        }
    }

    int GetInputNumber()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) return 0;
        if (Input.GetKeyUp(KeyCode.Alpha2)) return 1;
        if (Input.GetKeyUp(KeyCode.Alpha3)) return 2;
        if (Input.GetKeyUp(KeyCode.Alpha4)) return 3;

        else return -1;
    }
}
