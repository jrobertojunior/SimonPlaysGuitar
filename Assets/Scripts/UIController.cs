using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject InitialView;
    public GameObject OptionsView;
    public GameObject AboutView;

    public float transitionSpeed = 4f;

    CanvasGroup initialViewGroup;
    CanvasGroup optionsViewGroup;

    private GameObject currentObject;

    void Start()
    {
        initialViewGroup = InitialView.GetComponent<CanvasGroup>();
        optionsViewGroup = OptionsView.GetComponent<CanvasGroup>();

        currentObject = InitialView;
    }

    public void ChangeToOptionsView()
    {
        ChangeView(OptionsView);
    }

    public void ChangeToInitialView()
    {
        ChangeView(InitialView);
    }
    public void ChangeToAboutView()
    {
        ChangeView(AboutView);
    }

    private void ChangeView(GameObject next)
    {
        StartCoroutine(ChangeViewCoroutine(next));
    }

    IEnumerator ChangeViewCoroutine(GameObject next)
    {
        yield return StartCoroutine(FadeOutUI(currentObject.GetComponent<CanvasGroup>()));
        currentObject.SetActive(false);

        next.SetActive(true);
        yield return StartCoroutine(FadeInUI(next.GetComponent<CanvasGroup>()));

        currentObject = next;
    }



    IEnumerator FadeInUI(CanvasGroup group)
    {
        group.alpha = 0;
        float d = Time.deltaTime * transitionSpeed;

        while (group.alpha < 1)
        {
            group.alpha = group.alpha + d < 1 ? group.alpha + d : 1;

            yield return null;
        }
    }

    IEnumerator FadeOutUI(CanvasGroup group)
    {
        group.alpha = 1;
        float d = Time.deltaTime * transitionSpeed;

        while (group.alpha > 0)
        {
            group.alpha = group.alpha - d > 0 ? group.alpha - d : 0;

            yield return null;
        }
    }
}
