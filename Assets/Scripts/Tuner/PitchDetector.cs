using NAudio.Wave;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Guitar_Tuner;

public class PitchDetectorEvent : UnityEvent<string, float> { }

public class PitchDetector : MonoBehaviour
{
    public float Frequency;
    public string Note;
    public float ErrorTolerance = 0.05f;

    public PitchDetectorEvent OnNewNote = new PitchDetectorEvent();

    BufferedWaveProvider bufferedWaveProvider = null;
    Dictionary<string, float> noteBaseFreqs = new Dictionary<string, float>()
            {
                { "C", 16.35f },
                { "C#", 17.32f },
                { "D", 18.35f },
                { "Eb", 19.45f },
                { "E", 20.60f },
                { "F", 21.83f },
                { "F#", 23.12f },
                { "G", 24.50f },
                { "G#", 25.96f },
                { "A", 27.50f },
                { "Bb", 29.14f },
                { "B", 30.87f },
            };

    IWaveProvider stream;
    Pitch pitch;

    byte[] buffer;
    int bytesRead;


    public int SelectInputDevice()
    {
        int inputDevice = 0;
        bool isValidChoice = false;

        do
        {
            Console.Clear();
            Console.WriteLine("Please select input or recording device: ");

            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                Console.WriteLine(i + ". " + WaveInEvent.GetCapabilities(i).ProductName);
            }

            Console.WriteLine();

            try
            {
                if (int.TryParse(Console.ReadLine(), out inputDevice))
                {
                    isValidChoice = true;
                    Console.WriteLine("You have chosen " + WaveInEvent.GetCapabilities(inputDevice).ProductName + ".\n");
                }
                else
                {
                    isValidChoice = false;
                }
            }
            catch
            {
                throw new ArgumentException("Device # chosen is out of range.");
            }

        } while (isValidChoice == false);

        return inputDevice;
    }

    void Start()
    {
        WaveInEvent waveIn = new WaveInEvent();

        // todo: choose input device
        waveIn.DeviceNumber = 0;
        waveIn.WaveFormat = new WaveFormat(44100, 1);
        waveIn.DataAvailable += WaveIn_DataAvailable;

        bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);

        // begin record
        waveIn.StartRecording();

        stream = new Wave16ToFloatProvider(bufferedWaveProvider);
        pitch = new Pitch(stream);

        buffer = new byte[8192];


        //// stop recording
        //waveIn.StopRecording();
        //waveIn.Dispose();
    }

    bool lastFrameHadNote = false;
    string lastNote;

    void Update()
    {
        bytesRead = stream.Read(buffer, 0, buffer.Length);

        Frequency = pitch.Get(buffer);
        Note = GetNote(Frequency);

        if (Frequency != 0)
        {
            // if the same note has been played
            if (!lastFrameHadNote || Note != lastNote)
            {
                OnNewNote?.Invoke(Note, Frequency);
            }

            lastFrameHadNote = true;
            lastNote = Note;
        }
        else
        {
            lastFrameHadNote = false;
        }
    }

    void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        if (bufferedWaveProvider != null)
        {
            bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            bufferedWaveProvider.DiscardOnBufferOverflow = true;
        }
    }

    public string GetNote(float freq)
    {
        float baseFreq;

        foreach (var note in noteBaseFreqs)
        {
            baseFreq = note.Value;

            for (int i = 0; i < 9; i++)
            {
                if (InRange(freq, baseFreq, ErrorTolerance))
                {
                    return note.Key + i;
                }

                baseFreq *= 2;
            }
        }

        return null;
    }

    private bool InRange(float freq, float targetFreq, float error)
    {
        if (Math.Abs(freq - targetFreq) < targetFreq * error)
            return true;

        return false;
    }
}
