using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellAnimation : MonoBehaviour
{
    private Renderer renderer;
    private Material material;
    private TextMeshPro text;
    private Color baseColor;

    public float Min = 0.1f;
    public float Max = 1f;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
        baseColor = material.color;

        text = gameObject.GetComponentInChildren<TMPro.TextMeshPro>();

        SetEmission(Min);
    }

    public void Activate(bool fadeOut = false, bool appear = true)
    {
        StartCoroutine(ActivateCoroutine(fadeOut, appear));
    }

    IEnumerator ActivateCoroutine(bool fadeOut = false, bool appear = true)
    {
        float emission = Max;

        float alpha = appear ? 1 : 0;

        while (emission > Min)
        {
            emission -= Time.deltaTime;
            SetEmission(emission);

            if (fadeOut)
            {
                alpha -= Time.deltaTime * 1.5f;
                SetAlpha(material, alpha);

                if (text != null)
                {
                    SetAlpha(text, alpha);
                }
            }

            yield return null;
        }
    }

    public IEnumerator ShowCell()
    {
        if (material != null)
        {
            float alpha = material.color.a;

            while (alpha < 1)
            {
                alpha += Time.deltaTime;
                SetAlpha(material, alpha);

                if (text != null)
                {
                    SetAlpha(text, alpha);
                }

                yield return null;
            }
        }
    }

    private void SetEmission(float emission)
    {
        emission = Mathf.Clamp(emission, Min, Max);

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        material.SetColor("_EmissionColor", finalColor);
    }

    private void SetAlpha(Material material, float alpha)
    {
        alpha = Mathf.Clamp(alpha, 0, 1);
        material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
    }

    private void SetAlpha(TextMeshPro text, float alpha)
    {
        alpha = Mathf.Clamp(alpha, 0, 1);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }
}
