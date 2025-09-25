using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    private LevelManager levelManager;
    private Move3D move3D;
    private Outline outline;

    public GameObject correctSnap;
    private bool insideSnap;

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
        if (move3D.currentSnap) insideSnap = true; else insideSnap = false;
    }

    private void OnMouseUp()
    {
        if (insideSnap && outline != null)
        {
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 7f;

            if (levelManager.IsPieceCorrect(gameObject, correctSnap))
            {
                Debug.Log("deu siiiiiiiiiiiim");
                outline.enabled = true;   // ✅ Peça correta → acende verde
            }
            else
            {
                Debug.Log("n deuuuuuuuuuuu");
                outline.enabled = false;  // ❌ Peça errada → não acende
            }
        }
        else
        {
            // Desliga outline se não estiver em snap
            if (outline != null) outline.enabled = false;
        }
    }
}
