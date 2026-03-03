using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private AudioManagerMenu audioManagerMenu;

    [Tooltip("Nome da cena que deve ser carregada ao precionar retorno")]
    [SerializeField] private string cenaPosterior;
    [Tooltip("Nome da cena que deve ser carregada ao precionar continuar")]
    [SerializeField] private string cenaAnterior;

    [Tooltip("Nome da cena que deve ser carregada ao precionar o botão 1")]
    [SerializeField] private string cenaBotao1;
    [Tooltip("Nome da cena que deve ser carregada ao precionar o botão 2")]
    [SerializeField] private string cenaBotao2;
    [Tooltip("Nome da cena que deve ser carregada ao precionar o botão 3")]
    [SerializeField] private string cenaBotao3;
    [Tooltip("Nome da cena que deve ser carregada ao precionar o botão 4")]
    [SerializeField] private string cenaBotao4;
    [Tooltip("Nome da cena que deve ser carregada ao precionar o botão 5")]
    [SerializeField] private string cenaBotao5;
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

    public void Botao4()
    {
        audioManagerMenu.PlaySelectSound();

        Debug.Log("Carregando cena " + cenaBotao4);

        SceneManager.LoadScene(cenaBotao4);
    }

    public void Botao5()
    {
        audioManagerMenu.PlaySelectSound();

        Debug.Log("Carregando cena " + cenaBotao5);

        SceneManager.LoadScene(cenaBotao5);
    }

    public void Sair()
    {
        audioManagerMenu.PlaySelectSound();

        Debug.Log("Saiu do jogo");
        Application.Quit();
    }
}
