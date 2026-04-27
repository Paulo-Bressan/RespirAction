using System.Collections;
using UnityEngine;

public class MenuFasesAlt : MonoBehaviour
{
    private MenuScript menuScript;

    void Start()
    {
        menuScript = gameObject.GetComponent<MenuScript>();
    }

    public void loadLevel(int boxID)
    {
        switch (boxID)
        {
            case 0: Debug.Log("[MENU FASES] Fase invalida, abortando"); 
                break; // nada
            case 1: menuScript.Botao1(); break;
            case 2: menuScript.Botao2(); break;
            case 3: menuScript.Botao3(); break;
            case 4: menuScript.Botao4(); break;
            case 5: menuScript.Botao5(); break;
        }
    }
}
