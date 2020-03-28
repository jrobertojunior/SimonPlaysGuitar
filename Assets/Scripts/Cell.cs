using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cell : MonoBehaviour
{
    private EmissiveBehaviour color;
    private AudioSource audio;
    //private TextMeshPro text;


    public AudioClip AudioClip;
    //public float ColorUnsaturation = 0.3f;

    void Start()
    {
        // set color
        color = gameObject.AddComponent<EmissiveBehaviour>();

        // set audio
        audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.clip = AudioClip;
    }

    public IEnumerator PlayCell(float speed, bool playSound = true, bool stopSoundAtEnd = false)
    {
        color.On();
        yield return new WaitForSeconds(0.2f * (1 / speed));

        //color.ChangeColorToOriginal();

        if (playSound)
            audio.Play();

        yield return new WaitForSeconds(1 * (1 / speed));

        //color.ChangeColorToUnsaturated();

        //yield return new WaitForSeconds(0.5f * (1 / speed));

        if (stopSoundAtEnd)
            audio.Stop();
    }

}
