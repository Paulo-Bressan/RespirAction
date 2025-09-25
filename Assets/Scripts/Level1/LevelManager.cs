using UnityEngine;
using System.Collections;
using Unity.Mathematics;


public class LevelManager : MonoBehaviour
{
    private GameObject[] pieceArray;
    private GameObject[] snapArray;
    private Vector3[] rotationArray;
    public int[] piecePosStatusArray;
    public int[] pieceRotStatusArray;
    private int piecePosStatusSum;
    private int pieceRotStatusSum;
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
            float distance = Vector3.Distance(pieceArray[i].transform.position, snapArray[i].transform.position);
            if (distance < 0.1f) // Se estiver muito próximo (0.1 unidades)
            {
                piecePosStatusArray[i] = 1;
            }
            else
                piecePosStatusArray[i] = 0;
        }

        piecePosStatusSum = 0;
        for (int i = 0; i < 5; i++)
            if (piecePosStatusArray[i] == 1) piecePosStatusSum++;
    }

    public bool IsPieceCorrect(GameObject piece, GameObject snap)
    {
        float tolerance = 0.1f;
        return Vector3.Distance(piece.transform.position, snap.transform.position) < tolerance;
    }

    private void Start()
    {
        findObjects();

        piecePosStatusArray = new int[5];
        pieceRotStatusArray = new int[5];
        for (int i = 0; i < 5; i++)
        {
            piecePosStatusArray[i] = 0;
            pieceRotStatusArray[i] = 0;
        }
        
        rotationArray = new Vector3[5];
        rotationArray[0] = new Vector3(34.095932f, 180f, 5.00895658e-06f);
        rotationArray[1] = new Vector3(273.892548f, -0.000100612931f, 89.3782806f);
        rotationArray[2] = new Vector3(300.359772f, -2.3708375e-05f, 180.000015f);
        rotationArray[3] = new Vector3(276.527863f, 0.893019617f, 271.638153f);
        rotationArray[4] = new Vector3(288.009247f, 358.962616f, 181.835861f);

        InvokeRepeating("checkPositions", 1f, 1f);

        dialogueLido = new bool[5];
        for (int i = 0; i < 5; i++)
        {
            dialogueLido[i] = false;
        }
    }

    private void Update()
    {
        if (piecePosStatusSum >= 5 && gameStatus != 1)
        {
            gameStatus = 1;
            Debug.Log("All pieces correct");
            CancelInvoke();

            StartCoroutine(ReplacePiecesWithDiafragmaWithDelay());
        }
        for (int i = 0; i < 5; i++)
        {
            if (piecePosStatusArray[i] == 1 && !dialogueLido[i])
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
