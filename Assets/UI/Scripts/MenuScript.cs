using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void Jogar()
    {
        Debug.Log("Carregando MenuFases");
        SceneManager.LoadScene("MenuFases");
    }
 
    public void AbrirFase1()
    {
        Debug.Log("Carregando Fase1");
        SceneManager.LoadScene("Fase1");
    }

    public void Opcoes()
    {

    }
    
    public void Sair()
    {
        Debug.Log("Saiu do jogo");
        Application.Quit();
    }
     
    public void Voltar()
    {
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
