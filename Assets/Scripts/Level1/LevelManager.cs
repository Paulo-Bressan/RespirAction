using UnityEngine;
using System.Collections;


public class LevelManager : MonoBehaviour
{
    private GameObject[] pieceArray;
    private GameObject[] snapArray;
    public int[] pieceStatusArray;
    private int pieceStatusSum;
    public Dialogue dialogue;
    private bool dialogueEsternalLido = false;
    private bool[] dialogueLido;

    // Referência para o prefab do diafragma
    public GameObject diafragmaPrefab;

    // Variável para controlar se já substituímos as peças
    private bool hasReplacedPieces = false;

    public Transform spawnPoint;

    [SerializeField] private float diafragmaDelay = 2.0f;

    public int gameStatus;

    private void findObjects()
    {
        pieceArray = new GameObject[5];
        pieceArray[0] = GameObject.Find("Esternal");
        pieceArray[1] = GameObject.Find("CostalEsq");
        pieceArray[2] = GameObject.Find("Tendao");
        pieceArray[3] = GameObject.Find("CostalDir");
        pieceArray[4] = GameObject.Find("Lombar");

        snapArray = new GameObject[5];
        snapArray[0] = GameObject.Find("SnapAreaTop");
        snapArray[1] = GameObject.Find("SnapAreaLeft");
        snapArray[2] = GameObject.Find("SnapAreaMid");
        snapArray[3] = GameObject.Find("SnapAreaRight");
        snapArray[4] = GameObject.Find("SnapAreaBot");
    }
    private void checkPositions()
    {
        for (int i = 0; i < 5; i++)
        {
            pieceStatusArray[i] = IsPieceCorrect(pieceArray[i], snapArray[i]);
        }

        pieceStatusSum = 0;
        for (int i = 0; i < 5; i++)
            if (pieceStatusArray[i] == 2) pieceStatusSum++;
    }

    public int IsPieceCorrect(GameObject piece, GameObject snap)
    {
        float tolerance = 1f;
        //Debug.Log("Piece pos = " + piece.transform.position + ", Snap pos = " + snap.transform.position);
        if (Vector3.Distance(piece.transform.position, snap.transform.position) < tolerance)
        {
            if (Quaternion.Angle(piece.transform.rotation, snap.transform.rotation) < tolerance)
                return 2;
            else
                return 1;
        }
        else return 0;
    }

    private void Start()
    {
        findObjects();

        pieceStatusArray = new int[5];
        for (int i = 0; i < 5; i++)
            pieceStatusArray[i] = 0;

        InvokeRepeating("checkPositions", 1f, 1f);

        dialogueLido = new bool[5];
        for (int i = 0; i < 5; i++)
        {
            dialogueLido[i] = false;
        }
    }

    private void Update()
    {
        if (pieceStatusSum >= 5 && gameStatus != 1)
        {
            gameStatus = 1;
            Debug.Log("All pieces correct");
            CancelInvoke();

            StartCoroutine(ReplacePiecesWithDiafragmaWithDelay());
        }
        for (int i = 0; i < 5; i++)
        {
            if (pieceStatusArray[i] == 2 && !dialogueLido[i])
            {
                dialogue.dialogueIndex = i;
                dialogue.readyDialogue = true;
                dialogueLido[i] = true;
            }
        }
    }

    private IEnumerator ReplacePiecesWithDiafragmaWithDelay()
    {
        if (hasReplacedPieces || diafragmaPrefab == null)
        {
            yield break; // Interrompe a corrotina
        }

        Debug.Log("Todas as peças estão corretas. Aguardando o delay para a destruição.");

        // Atraso para a desativação e a criação do diafragma
        yield return new WaitForSeconds(diafragmaDelay);

        // A partir daqui, as ações acontecem imediatamente após o delay

        // Desativa as peças originais
        for (int i = 0; i < 5; i++)
        {
            if (pieceArray[i] != null)
            {
                pieceArray[i].SetActive(false);
            }
        }

        // Instancia o diafragma
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject diafragma = Instantiate(diafragmaPrefab, spawnPosition, spawnRotation);

        // Ajusta a escala e rotação do diafragma
        diafragma.transform.localScale = new Vector3(70f, 70f, 70f);
        diafragma.transform.rotation = Quaternion.Euler(-93.496f, 57.089f, 122.96f);

        Debug.Log("Diafragma montado com sucesso!");
        hasReplacedPieces = true;
    }
}
