using UnityEngine;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    public void Jogar()
    {
        SceneManager.LoadScene("Fase1");


    }
    public void Abrir_opcao()
    {

    }
    public void Fechar_opcao()
    {

    }
    
    public void Fechar_jogo()
    {
        Debug.Log("Saiu do jogo");
        Application.Quit();
    }
}
