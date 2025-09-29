using UnityEngine;
<<<<<<< HEAD

=======
using UnityEngine.SceneManagement;
>>>>>>> 8bc9831bf71deeb6b1aa395d3cd6cf1da0aac75f
public class menu : MonoBehaviour
{
    public void Jogar()
    {
<<<<<<< HEAD
=======
        SceneManager.LoadScene("MenuFases");

>>>>>>> 8bc9831bf71deeb6b1aa395d3cd6cf1da0aac75f

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
