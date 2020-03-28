using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cell : MonoBehaviour
{
    private CellAnimation cellAnimation;
    private AudioSource audio;
    public AudioClip AudioClip;


    void Start()
    {
        // set animation
        cellAnimation = gameObject.AddComponent<CellAnimation>();

        // set audio
        audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.clip = AudioClip;
    }

    public IEnumerator PlayCell(float speed, bool playSound = true, bool stopSoundAtEnd = false, bool fadeOut = false, bool appear = true)
    {
        cellAnimation.Activate(fadeOut, appear);

        if (playSound)
            audio.Play();

        yield return new WaitForSeconds(1 * (1 / speed));

        if (stopSoundAtEnd)
            audio.Stop();
    }
}
