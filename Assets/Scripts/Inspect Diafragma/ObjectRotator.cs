using UnityEngine;
using UnityEngine.EventSystems; // Importante para checar UI

public class ObjectRotator : MonoBehaviour
{
    [Header("Rotação Manual")]
    [Tooltip("A velocidade com que o objeto rotaciona com o mouse.")]
    public float rotationSpeed = 150f;

    [Header("Auto-Rotação")]
    [Tooltip("A velocidade da rotação automática.")]
    public float autoRotateSpeed = 20f;
    
    [Header("Detecção")]
    [Tooltip("A câmera principal (necessária para o raycast e zoom)")]
    public Camera mainCamera; // << CORREÇÃO: Variável declarada aqui
    
    // Controla se a auto-rotação está ligada ou desligada
    private bool isAutoRotating = true;

    // --- Variáveis para "Clique vs Arraste" ---
    private Vector3 mousePressPosition;
    // Quantos pixels o mouse pode mover antes de ser considerado "arraste"
    private float mouseMoveThreshold = 5f; 

    void Start()
    {
        // Pega a câmera automaticamente se não for definida
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Se o painel de descrição estiver aberto, PARE TUDO.
        // Isso impede que o objeto gire ou receba cliques.
        if (DescriptionPanelManager.Instance.IsPanelOpen())
        {
            return; 
        }

        // --- Lógica do 'Interruptor' de Auto-RotaÇÃO (Espaço) ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAutoRotating = !isAutoRotating;
        }

        // --- Lógica do Zoom ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        mainCamera.transform.Translate(0, 0, scroll * 10f, Space.Self);


        // --- Lógica de Rotação e Clique ---

        // 1. Rotação Manual (Drag)
        if (Input.GetMouseButton(0)) // Botão pressionado (arrastando)
        {
            // Só rotaciona se o mouse tiver se movido o suficiente
            if (Vector3.Distance(Input.mousePosition, mousePressPosition) > mouseMoveThreshold)
            {

                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

                transform.Rotate(Vector3.up, -mouseX, Space.World);
                transform.Rotate(Vector3.right, mouseY, Space.World);
            }

        }
        // 2. Auto-Rotação
        else if (isAutoRotating)
        {
            transform.Rotate(Vector3.up, autoRotateSpeed * Time.deltaTime, Space.World);
        }

        // --- Lógica de Detecção de Clique ---

        // 3. Detecta o PRESSIONAR do mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Guarda a posição inicial do clique
            mousePressPosition = Input.mousePosition;
        }

        // 4. Detecta o SOLTAR do mouse
        if (Input.GetMouseButtonUp(0))
        {
            // Se o mouse mal se moveu (foi um clique, não um arraste)
            if (Vector3.Distance(Input.mousePosition, mousePressPosition) < mouseMoveThreshold)
            {
                // Foi um clique!
                HandleClick();
            }
        }
    }

    //  função para lidar com o clique
    void HandleClick()
    {
        // Não faça nada se o mouse estiver sobre a UI 
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Verifica se o objeto clicado tem o script 'InteractablePart'
            InteractablePart part = hit.collider.GetComponent<InteractablePart>();

            if (part != null)
            {
                // Acertamos! Mostra o painel com a descrição daquela parte.
                DescriptionPanelManager.Instance.ShowPanel(part.description);
            }
        }
    }
}