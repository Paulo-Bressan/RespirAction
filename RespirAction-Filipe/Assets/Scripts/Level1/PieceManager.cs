using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Jobs;

public class PieceManager : MonoBehaviour
{
    private GameObject[] pieceArray;

    private float startPosX = 0.5F;
    private float startPosZ = 0.6F;
    private float pieceDistanceX = 0.4F;
    private float pieceDistanceZ = -0.4F;

    private Vector3[] initializeTransforms()
    {
        Vector3[] transformArray = new Vector3[12];

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 3; j++)
            {
                transformArray[(3 * i) + j] = 
                    new Vector3(startPosX + j * pieceDistanceX, 1.7F, startPosZ + i * pieceDistanceZ);

                //Debug.Log("transformArray [" + (3 * i + j) + "] coordinate is (" +
                //    transformArray[3 * i + j].x + " | " + transformArray[3 * i + j].z + ")");
            }

        return transformArray;
    }
    private void shuffleTransforms(Vector3[] transformArray)
    {
        for (int t = 0; t < transformArray.Length; t++)
        {
            Vector3 tmp = transformArray[t];
            int r = Random.Range(t, transformArray.Length);
            transformArray[t] = transformArray[r];
            transformArray[r] = tmp;
        }
    }

    private void shuffleRotation(GameObject piece)
    {
        float randomX = (Random.Range(0, 3) * 90);
        float randomY = (Random.Range(0, 3) * 90);
        float randomZ = (Random.Range(0, 3) * 90);

        piece.transform.Rotate(randomX, randomY, randomZ);
    }

    private void Start()
    {
        pieceArray = GameObject.FindGameObjectsWithTag("Peca");
        //for (int i = 0; i < pieceArray.Length; i++)
        //    Debug.Log(pieceArray[i].name);

        Vector3[] transformArray = initializeTransforms();

        shuffleTransforms(transformArray);

        for (int i = 0; i < 12; i++)
        {
            pieceArray[i].transform.position = transformArray[i];
            pieceArray[i].GetComponent<Move3D>().startPosition = transformArray[i];
            shuffleRotation(pieceArray[i]);
        }
    }
}
