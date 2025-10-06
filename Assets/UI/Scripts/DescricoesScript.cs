using UnityEngine;
using UnityEngine.UI;

public class BotaoMostrarImagens : MonoBehaviour
{
    public Image imagem1;
    public Image imagem2;

    void Start()
    {
        imagem1.enabled = false;
        imagem2.enabled = false;
    }

    public void MostrarImagens()
    {
        imagem1.enabled = true;
        imagem2.enabled = true;
    }
}
