using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSays
{

    public List<int> Sequence { get => sequence; set => sequence = value; }
    private List<int> sequence = new List<int>();

    private int n;

    public SimonSays(int n)
    {
        this.n = n;
    }

    public List<int> AddTurn()
    {
        if (sequence != null)
        {
            sequence.Add(Random.Range(0, n));
        }

        return sequence;
    }
}
