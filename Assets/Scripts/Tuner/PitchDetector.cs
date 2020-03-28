using NAudio.Wave;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Guitar_Tuner;
using System.Collections;

public class PitchDetectorEvent : UnityEvent<string, float> { }

public class PitchDetector : MonoBehaviour
{
    public float Frequency;
    public string Note;
    public string NoteWithoutOctave;
    public float ErrorTolerance = 0.03f;

    public string CurrentNote;

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

    private int SelectInputDevice()
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

        StartCoroutine(UpdateReading());
    }

    bool lastFrameHadNote = false;
    string lastNote = "";

    [SerializeField]
    float noteSampleDelta;
    float fixedTimeSinceLastNote = 0;

    float begin;

    IEnumerator UpdateReading()
    {
        while (true)
        {
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            Frequency = pitch.Get(buffer);
            Tuple<string, string> notes = GetNote(Frequency);

            if (notes != null)
            {
                Note = notes.Item1;
                NoteWithoutOctave = notes.Item2;
            }

            if (Frequency != 0)
            {
                noteSampleDelta = Time.fixedTime - fixedTimeSinceLastNote;

                fixedTimeSinceLastNote = Time.fixedTime;

                // if the same note has been played
                if (!lastFrameHadNote || Note != lastNote)
                {
                    OnNewNote?.Invoke(Note, Frequency);
                    print("nova nota!!");

                    CurrentNote = NoteWithoutOctave;
                } else
                {
                    CurrentNote = string.Empty;
                }

                begin = Time.fixedTime;
                lastFrameHadNote = true;
                lastNote = Note;
            }
            else if (Time.fixedTime - begin > 0.4)
            {
                lastFrameHadNote = false;
                print("falta nota");

                begin = Time.fixedTime;
                CurrentNote = string.Empty;
            }


            yield return null;
        }
    }

    private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        if (bufferedWaveProvider != null)
        {
            bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            bufferedWaveProvider.DiscardOnBufferOverflow = true;
        }
    }

    private Tuple<string, string> GetNote(float freq)
    {
        float baseFreq;

        foreach (var note in noteBaseFreqs)
        {
            baseFreq = note.Value;

            for (int i = 0; i < 9; i++)
            {
                if (InRange(freq, baseFreq, ErrorTolerance))
                {
                    return new Tuple<string, string>(note.Key + i, note.Key);
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

    public bool GetNoteInput(string note, bool octaveSensitive = true)
    {
        //if (Frequency != 0)
        //{
        //    // new note played
        //    if (!lastFrameHadNote || Note != lastNote)
        //    {
        //        if (octaveSensitive)
        //        {
        //            if (note == Note)
        //            {
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            if (note == NoteWithoutOctave)
        //            {
        //                print("input de nota");
        //                return true;
        //            }
        //        }
        //    }
        //}

        //return false;

        return note == CurrentNote;
    }

    bool isFlatOrSharp(string x) => (x[1] == 'b' || x[1] == '#');
    private bool NotesAreEqualWithoutOctave(string noteA, string noteB)
    {
        if (noteA.Length == noteB.Length)
        {
            if (noteA[0] == noteB[0])
            {
                // if both are flat or sharp
                if (isFlatOrSharp(noteA) == isFlatOrSharp(noteB))
                {
                    return noteA[1] == noteB[1];
                }
                return true;
            }
        }

        return false;
    }


}
