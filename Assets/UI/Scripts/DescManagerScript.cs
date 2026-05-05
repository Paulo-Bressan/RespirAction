using UnityEngine;
using UnityEngine.UI;

public class DescManagerScript : MonoBehaviour
{
    public GameObject painelDesc1, painelDesc2, painelDesc3;
    public GameObject botaoJogar1, botaoJogar2, botaoJogar3;

    void Start()
    {
        desativarPaineis();
    }

    public void desativarPaineis()
    {
        if (painelDesc1) painelDesc1.SetActive(false);
        if (painelDesc2) painelDesc2.SetActive(false);
        if (painelDesc3) painelDesc3.SetActive(false);

        if (botaoJogar1) botaoJogar1.SetActive(false);
        if (botaoJogar2) botaoJogar2.SetActive(false);
        if (botaoJogar3) botaoJogar3.SetActive(false);
    }

    public void ativarPainel(int n)
    {
        switch(n)
        {
            case 1:
                desativarPaineis();
                if (painelDesc1) painelDesc1.SetActive (true);
                if (botaoJogar1) botaoJogar1.SetActive (true);
                break;
            case 2:
                desativarPaineis();
                if (painelDesc2) painelDesc2.SetActive(true);
                if (botaoJogar2) botaoJogar2.SetActive(true);
                break;
            case 3:
                desativarPaineis();
                if (painelDesc3) painelDesc3.SetActive(true);
                if (botaoJogar3) botaoJogar3.SetActive(true);
                break;
        }
    }
}
