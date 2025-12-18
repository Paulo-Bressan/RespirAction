using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private AudioManagerMenu audioManagerMenu;

    public string cenaPosterior;
    public string cenaAnterior;

    public string cenaBotao1;
    public string cenaBotao2;
    public string cenaBotao3;

    public void Start()
    {
        audioManagerMenu = FindFirstObjectByType<AudioManagerMenu>();
    }
    
    public void Continuar()
    {
        audioManagerMenu.PlaySelectSound();

        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Avancando de " + currentScene.name + " para " + cenaAnterior);

        SceneManager.LoadScene(cenaPosterior);
    }

    public void Voltar()
    {
        audioManagerMenu.PlaySelectSound();

        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Voltando de " + currentScene.name + " para " + cenaAnterior);

        SceneManager.LoadScene(cenaAnterior);

    }

    public void Botao1()
    {
        audioManagerMenu.PlaySelectSound();

        Debug.Log("Carregando cena " + cenaBotao1);

        SceneManager.LoadScene(cenaBotao1);
    }

    public void Botao2()
    {
        audioManagerMenu.PlaySelectSound();

        Debug.Log("Carregando cena " + cenaBotao2);

        SceneManager.LoadScene(cenaBotao2);
    }

    public void Botao3()
    {
        audioManagerMenu.PlaySelectSound();

        Debug.Log("Carregando cena " + cenaBotao3);

        SceneManager.LoadScene(cenaBotao3);
    }

    public void Sair()
    {
        audioManagerMenu.PlaySelectSound();

        Debug.Log("Saiu do jogo");
        Application.Quit();
    }
}
