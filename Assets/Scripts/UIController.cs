using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject InitialViewObject;
    public GameObject OptionsViewObject;

    public float transitionSpeed = 4f;

    CanvasGroup initialView;
    CanvasGroup optionsView;

    private GameObject currentObject;

    void Start()
    {
        initialView = InitialViewObject.GetComponent<CanvasGroup>();
        optionsView = OptionsViewObject.GetComponent<CanvasGroup>();

        currentObject = InitialViewObject;
    }

    public void ChangeToOptionsView()
    {
        ChangeView(OptionsViewObject);
    }

    public void ChangeToInitialView()
    {
        ChangeView(InitialViewObject);
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
