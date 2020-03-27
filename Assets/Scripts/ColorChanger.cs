using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    Renderer renderer;

    Color originalColor;
    Color unsaturatedColor;

    Color textOriginalColor;
    Color textUnsaturatedColor;


    public float percentageOfUnsaturation = 0.50f;

    TMPro.TextMeshPro text;

    void Start()
    {
        // get renderer
        renderer = gameObject.GetComponent<Renderer>();

        // store the original color
        originalColor = renderer.material.color;

        // get the HSV and reduce saturation
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);
        s *= (1f - percentageOfUnsaturation);

        // convert back to RGB and store it
        unsaturatedColor = Color.HSVToRGB(h, s, v);

        // text properties
        text = GetComponentInChildren<TMPro.TextMeshPro>();
        if (text)
        {
            textOriginalColor = text.color;
            textUnsaturatedColor = Color.grey;
        }

        // change the state of the object to unsaturated color
        ChangeColorToUnsaturated();
    }

    public void ChangeColorToUnsaturated()
    {
        renderer.material.color = unsaturatedColor;

        if (text)
        {
            text.color = textUnsaturatedColor;
        }
    }

    public void ChangeColorToOriginal()
    {
        renderer.material.color = originalColor;

        if (text)
        {
            text.color = textOriginalColor;
        }
    }
}
