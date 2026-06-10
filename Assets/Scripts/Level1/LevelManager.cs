using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    private GameObject[] pieceArray;
    public GameObject[] snapArray;
    public GameObject audioManager;
    public int[] pieceStatusArray;
    private int pieceStatusSum;
    public Dialogue dialogue;
    //private bool dialogueEsternalLido = false;
    private bool[] dialogueLido;

    public GameObject diafragma;
    public Transform spawnPoint;
    public string nomeProximaCena;
    //private bool hasReplacedPieces = false;


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
            pieceStatusArray[i] = IsPieceCorrect(pieceArray[i], snapArray[i], 0.1f, 30f);
        }

        pieceStatusSum = 0;
        for (int i = 0; i < 5; i++)
            if (pieceStatusArray[i] == 2) pieceStatusSum++;
    }

    public int IsPieceCorrect(GameObject piece, GameObject snap, float posTolerance, float rotTolerance)
    {
        //Debug.Log("Piece pos = " + piece.transform.position + ", Snap pos = " + snap.transform.position);
        if (Vector3.Distance(piece.transform.position, snap.transform.position) < posTolerance)
        {
            if (Quaternion.Angle(piece.transform.rotation, snap.transform.rotation) < rotTolerance)
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

            //StartCoroutine(ReplacePiecesWithDiafragmaWithDelay());

            StartCoroutine(LoadSceneWithDelay());
        }
        for (int i = 0; i < 5; i++)
        {
            if (pieceStatusArray[i] == 2 && !dialogueLido[i])
            {
                dialogue.dialogueIndex = i;
                dialogue.readyDialogue = true;
                dialogueLido[i] = true;

                if (audioManager)
                    audioManager.GetComponent<AudioManagerScene>().PlaySound(2);
                else
                    Debug.Log("MISSING AUDIO MANAGER");
            }
        }
    }

    /*
    private IEnumerator ReplacePiecesWithDiafragmaWithDelay()
    {
        if (hasReplacedPieces)
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

        // Ajusta a escala e rotação do diafragma
        diafragma.transform.localScale = new Vector3(70f, 70f, 70f);
        diafragma.transform.position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        diafragma.transform.rotation = Quaternion.Euler(-93.496f, 57.089f, 122.96f);

        Debug.Log("Diafragma montado com sucesso!");
        hasReplacedPieces = true;
    }
    */

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(diafragmaDelay);
        SceneManager.LoadScene(nomeProximaCena);
    }
}
