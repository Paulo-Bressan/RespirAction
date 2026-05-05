using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfeitoDigitacao : MonoBehaviour
{
    public Text campoDeTexto;
    public float velocidade = 0.03f;

    [TextArea(3, 10)] // Isso cria uma caixa de texto maior no Inspector
    public string textoParaDigitar;

    // O Unity chama isso automaticamente quando o objeto aparece
    void OnEnable()
    {
        if (campoDeTexto != null)
        {
            StopAllCoroutines();
            StartCoroutine(Digitar());
        }
    }

    IEnumerator Digitar()
    {
        campoDeTexto.text = "";
        yield return new WaitForSeconds(0.1f); // Pequeno atraso para evitar bugs visuais

        foreach (char letra in textoParaDigitar.ToCharArray())
        {
            campoDeTexto.text += letra;
            yield return new WaitForSeconds(velocidade);
        }
    }
}