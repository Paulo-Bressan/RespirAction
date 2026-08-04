using TMPro;
using UnityEngine;

public class QuizFontManager : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset fonteQuiz;

    private void Awake()
    {
        AplicarFonte();
    }

    private void OnValidate()
    {
        AplicarFonte();
    }

    [ContextMenu("Aplicar fonte em todos os textos")]
    public void AplicarFonte()
    {
        if (fonteQuiz == null)
        {
            return;
        }

        TMP_Text[] textos = GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text texto in textos)
        {
            texto.font = fonteQuiz;
        }
    }
}
