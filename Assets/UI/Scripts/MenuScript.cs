using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private AudioManagerMenu audioManagerMenu;

    public string cenaPosterior;
    public string cenaAnterior;

    public void Start()
    {
        audioManagerMenu = FindFirstObjectByType<AudioManagerMenu>();
    }
    public void Jogar()
    {
        audioManagerMenu.PlaySelectSound();
        Debug.Log("Carregando MenuFases");
        SceneManager.LoadScene("MenuFases");
    }
 
    public void AbrirFase1()
    {
        audioManagerMenu.PlaySelectSound();
        Debug.Log("Carregando Fase1");
        SceneManager.LoadScene("Cutscene1");
    }

    public void Opcoes()
    {

    }
    
    public void Sair()
    {
        audioManagerMenu.PlaySelectSound();
        Debug.Log("Saiu do jogo");
        Application.Quit();
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
}
