using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    private LevelManager levelManager;
    private Move3D move3D;
    private Outline outline;

    public GameObject correctSnap;

    void Start()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
        move3D = GetComponent<Move3D>();
        outline = GetComponent<Outline>();

        if (outline == null) outline = gameObject.AddComponent<Outline>();
        outline.enabled = false;
    }

    void Update()
    {
        if (move3D.currentSnap && outline)
        {
            outline.OutlineWidth = 7f;

            switch (levelManager.IsPieceCorrect(gameObject, correctSnap, 0.1f, 5f))
            {
                case 0:
                    // Peça errada - não acende
                    outline.enabled = false;  
                    break;
                
                case 1:
                    // Peça torta - acende amarelo
                    outline.OutlineColor = Color.yellow;
                    outline.enabled = true;  
                    break;
                case 2:
                    // Peça correta - acende verde
                    outline.OutlineColor = Color.green;
                    outline.enabled = true;   
                    break;
            }
        }
        else
        {
            // Desliga outline se não estiver em snap
            if (outline != null) outline.enabled = false;
        }
    }
}
