using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Move3D : MonoBehaviour
{
    private Camera mainCamera;
    private float CameraZDistance;

    public GameObject[] snapArray;
    public GameObject currentSnap;
    private bool playAnimation = false;
    public bool insideSnapArea = false;

    public Vector3 startPosition;
    private Vector3 targetPosition;

    public GameObject audioManager;

    void Start()
    {
        mainCamera = Camera.main;
        CameraZDistance =
            mainCamera.WorldToScreenPoint(transform.position).z; // guarda a distancia da camera

        startPosition = transform.position; // guarda a posição "de repouso"
        
        snapArray = GameObject.FindGameObjectsWithTag("Snap");
    }

    private void Update()
    {
        // quando a peça receber o comando de animação, a função update encarrega de cuidar
        // da movimentação usando a função Lerp (interpolação linear) entre a posição
        // atual e a posição alvo. Quando chegar perto o suficiente, encerra a animação

        if (playAnimation)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                playAnimation = false;
                transform.position = targetPosition;
            }
        }
    }

    void OnMouseDrag()
    {
        // Conversões de posição em tela para posição global para simular
        // o movimento da peça junto do mouse quando clicar e segurar
        // um pouquinho complexo, depois me pergunta que eu passo o vídeo

        if (!playAnimation)
        {
            Vector3 ScreenPosition =
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraZDistance);
            Vector3 NewWorldPosition =
                mainCamera.ScreenToWorldPoint(ScreenPosition);
            NewWorldPosition.y = transform.position.y;

            transform.position = NewWorldPosition;
        }
    }

    private void OnMouseDown()
    {
        if (currentSnap)
        {
            currentSnap.GetComponent<SnapArea>().currentObject = null;
            currentSnap = null;
        }

        audioManager.GetComponent<AudioManager>().PlayGrabSound();
    }

    private void OnMouseUp()
    {
        // quando solta o mouse, verifica se a peça está dentro de um snap
        // se o snap estiver ocupado, manda para a posição inicial
        // se o snap estiver disponível, manda para o centro do snap
        // depois inicia o processo de animação em Update()

        if (insideSnapArea)
        {
            currentSnap = findClosestSnap(transform.position);
            if (currentSnap)
            {
                currentSnap.GetComponent<SnapArea>().currentObject = gameObject;
                targetPosition = currentSnap.transform.position;
            }
            else
            {
                targetPosition = startPosition;
            }
        }
        else
        {
            targetPosition = startPosition;
        }

        audioManager.GetComponent<AudioManager>().PlayReleaseSound();
        playAnimation = true;
    }

    private GameObject findClosestSnap(Vector3 objPos)
    {
        float minDist = float.PositiveInfinity;
        GameObject closestSnap = null;

        foreach (GameObject snap in snapArray)
        {
            float dist = Vector3.Distance(snap.transform.position, objPos);
            if (minDist > dist && snap.GetComponent<SnapArea>().currentObject == null)
            {
                minDist = dist;
                closestSnap = snap;
            }
        }

        return closestSnap;
    }

}