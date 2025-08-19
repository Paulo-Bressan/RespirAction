using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(Collider))]
public class Move3D : MonoBehaviour
{
    private Camera mainCamera;
    private float CameraZDistance;

    public bool insideSnap;
    public GameObject snapArea;
    private bool playAnimation = false;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        mainCamera = Camera.main;
        CameraZDistance =
            mainCamera.WorldToScreenPoint(transform.position).z; // guarda a distancia da camera

        startPosition = transform.position; // guarda a posição "de repouso"
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

        Vector3 ScreenPosition =
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraZDistance);
        Vector3 NewWorldPosition =
            mainCamera.ScreenToWorldPoint(ScreenPosition);
            NewWorldPosition.y = transform.position.y;

        transform.position = NewWorldPosition;
    }

    private void OnMouseUp()
    {
        // quando solta o mouse, verifica se a peça está dentro de um snap
        // se o snap estiver ocupado, manda para a posição inicial
        // se o snap estiver disponível, manda para o centro do snap
        // depois inicia o processo de animação em Update()

        if (insideSnap)
        {
            if (snapArea.GetComponent<SnapArea>().hasObject > 1)
                targetPosition = startPosition;
            else
                targetPosition = snapArea.transform.position;

            playAnimation = true;
        }
    }


}