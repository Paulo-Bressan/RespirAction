using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameObject[] pieceArray;
    private GameObject[] snapArray;
    public int[] piecePosStatusArray;
    public int[] pieceRotStatusArray;
    private int pieceStatusSum;

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
            if (pieceArray[i].transform.position == snapArray[i].transform.position)
            {
                piecePosStatusArray[i] = 1;
                // trigger behavior for GUI elements here
            }
            else
                piecePosStatusArray[i] = 0;
        }

        pieceStatusSum = 0;
        for (int i = 0; i < 5; i++)
            if (piecePosStatusArray[i] == 1) pieceStatusSum++;
    }
    private void Start()
    {
        findObjects();

        piecePosStatusArray = new int[5];
        for (int i = 0; i < 5; i++)
            piecePosStatusArray[i] = 0;

        InvokeRepeating("checkPositions",1f,1f);
    }

    private void Update()
    {
        if(pieceStatusSum >= 5 && gameStatus != 1)
        {
            gameStatus = 1;
            Debug.Log("All pieces correct");
            CancelInvoke();
        }
    }
}
