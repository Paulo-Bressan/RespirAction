using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class StageHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Órgão revelado no hover")]
    public Image organImage;

    [Header("Opacidade")]
    [Range(0f, 1f)] public float hiddenAlpha = 0f;
    [Range(0f, 1f)] public float visibleAlpha = 1f;

    [Header("Escala do órgão")]
    public float normalScale = 1f;
    public float hoverScale = 1.06f;

    [Header("Velocidade")]
    public float animationSpeed = 8f;

    [Header("Cores do botão")]
    public Color normalButtonColor = Color.white;
    public Color hoverButtonColor = Color.cyan;

    private Image buttonImage;
    private Coroutine currentAnimation;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();

        if (organImage != null)
        {
            SetAlpha(organImage, hiddenAlpha);
            organImage.transform.localScale = Vector3.one * normalScale;
        }

        if (buttonImage != null)
        {
            buttonImage.color = normalButtonColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = hoverButtonColor;
        }

        AnimateTo(visibleAlpha, hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = normalButtonColor;
        }

        AnimateTo(hiddenAlpha, normalScale);
    }

    private void AnimateTo(float targetAlpha, float targetScale)
    {
        if (organImage == null)
        {
            Debug.LogWarning("StageHoverEffect: organImage não foi configurado em " + gameObject.name);
            return;
        }

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(AnimateOrgan(targetAlpha, targetScale));
    }

    private IEnumerator AnimateOrgan(float targetAlpha, float targetScale)
    {
        float startAlpha = organImage.color.a;
        Vector3 startScale = organImage.transform.localScale;
        Vector3 endScale = Vector3.one * targetScale;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * animationSpeed;

            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            SetAlpha(organImage, newAlpha);

            organImage.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            yield return null;
        }

        SetAlpha(organImage, targetAlpha);
        organImage.transform.localScale = endScale;
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}