using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSays
{

    public List<int> Sequence { get => sequence; set => sequence = value; }
    private List<int> sequence = new List<int>();

    public bool neverGetRepeatedNumbers = false;

    private int n;

    public SimonSays(int n)
    {
        this.n = n;
    }

    public List<int> AddTurn()
    {
        if (sequence != null)
        {
            int value = Random.Range(0, n);

            if (sequence.Count > 0 && neverGetRepeatedNumbers) {
                while (value == sequence[sequence.Count - 1]) {
                    value = Random.Range(0, n);
                }
            }

            sequence.Add(value);
        }

        return sequence;
    }
}
