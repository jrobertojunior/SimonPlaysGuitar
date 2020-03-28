using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveBehaviour : MonoBehaviour
{
    Renderer renderer;
    Material material;
    Color baseColor;

    public float Min = 0.1f;
    public float Max = 1f;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
        baseColor = material.color;

        SetEmission(Min);
    }

    public void On()
    {
        StartCoroutine(OnCoroutine());
    }

    IEnumerator OnCoroutine()
    {
        float emission = Max;

        while (emission > Min)
        {
            emission -= Time.deltaTime;
            SetEmission(emission);

            yield return null;
        }
    }

    private void SetEmission(float emission)
    {
        Mathf.Clamp(emission, Min, Max);

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        material.SetColor("_EmissionColor", finalColor);
    }
}
