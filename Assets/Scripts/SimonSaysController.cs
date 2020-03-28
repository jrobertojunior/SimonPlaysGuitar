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

    public Cell[] Cells; 
    public AudioSource loseSound;
    public float timeToChoose = 2;
    public TMPro.TextMeshProUGUI Score;
    public TMPro.TextMeshProUGUI Status;

    public bool PlayAudioOnInput = true;

    private SimonSays game;

    private PitchDetector pitchDetector;

    private bool playing;

    private float gameSpeed = 1;

    void Start()
    {
        pitchDetector = gameObject.AddComponent<PitchDetector>();
        FreePlay();
    }

    public void NewGame()
    {
        game = new SimonSays(Cells.Length);
        StopAllCoroutines();

        StartCoroutine(Game());
    }

    public void FreePlay()
    {
        StopAllCoroutines();

        StartCoroutine(FreePlaying());
    }

    IEnumerator FreePlaying()
    {
        while (true)
        {
            // get user input
            int playerInput = GetInputNote();

            if (playerInput != -1)
            {
                print(playerInput);

                // play correspondent cell
                StartCoroutine(Cells[playerInput].PlayCell(1.5f, playSound: PlayAudioOnInput));
            }

            yield return null;
        }
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
        FreePlay();
    }

    IEnumerator PlayInitialAnimation()
    {
        Cells[0].GetComponent<AudioSource>().Play();
        Cells[1].GetComponent<AudioSource>().Play();
        //Cells[3].GetComponent<AudioSource>().Play();

        for (int i = 0; i < 3; i++)
        {
            foreach (Cell cell in Cells)
            {
                yield return StartCoroutine(cell.PlayCell(20f, playSound: false));
            }
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ComputerTurn()
    {
        foreach (int cellNumber in game.Sequence)
        {
            yield return StartCoroutine(Cells[cellNumber].PlayCell(gameSpeed));
        }

        foreach (var cell in Cells)
        {
            cell.GetComponent<AudioSource>().Stop();
        }

        //yield return new WaitForSeconds(1);
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

                //int playerInput = GetInputNumber();
                int playerInput = GetInputNote();

                if (playerInput != -1) // if the input is valid
                {
                    // play correspondent cell
                    StartCoroutine(Cells[playerInput].PlayCell(1.5f, playSound: PlayAudioOnInput));

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
        // todo: change to key down with check for repetition
        if (Input.GetKeyUp(KeyCode.Alpha1)) return 0;
        if (Input.GetKeyUp(KeyCode.Alpha2)) return 1;
        if (Input.GetKeyUp(KeyCode.Alpha3)) return 2;
        if (Input.GetKeyUp(KeyCode.Alpha4)) return 3;

        else return -1;
    }

    int GetInputNote()
    {
        if (pitchDetector.GetNoteInput("D", octaveSensitive: false)) return 0;
        if (pitchDetector.GetNoteInput("F#", octaveSensitive: false)) return 1;
        if (pitchDetector.GetNoteInput("G", octaveSensitive: false)) return 2;
        if (pitchDetector.GetNoteInput("A", octaveSensitive: false)) return 3;

        //if (pitchDetector.NoteWithoutOctave == "D") return 0;
        //if (pitchDetector.NoteWithoutOctave == "F#") return 1;
        //if (pitchDetector.NoteWithoutOctave == "G") return 2;
        //if (pitchDetector.NoteWithoutOctave == "A") return 3;

        return -1;
    }
}
