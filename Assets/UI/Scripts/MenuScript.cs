using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private AudioManagerMenu audioManagerMenu;

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
     
    public void Voltar()
    {
        audioManagerMenu.PlaySelectSound();

        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Voltando de " + scene.name);

        
        switch (scene.name)
        {
            case "Fase1":
                Debug.Log("para Menu");
                SceneManager.LoadScene("Menu");
            break;

            case "Fase2":
                // fase 2, etc
            break;
        }
        
    }
}
