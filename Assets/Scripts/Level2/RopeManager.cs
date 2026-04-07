using UnityEngine;

public class RopeManager : MonoBehaviour
{
    [Tooltip("Camera")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Objeto da corda")]
    [SerializeField] private GameObject ropeRaw;

    [Tooltip("Transform começo da corda")]
    [SerializeField] private Transform ropeA;

    [Tooltip("Transform destino da corda")]
    [SerializeField] private Transform ropeB;

    // referencias
    private NodeClickManager nodeClickManager;
    private RopeDoubleAnchor ropeDoubleAnchor;

    // uso interno
    private GameObject pulledNode;

    void Start()
    {
        nodeClickManager = gameObject.GetComponent<NodeClickManager>();
        if (!nodeClickManager)
            Debug.Log("[CORDA] Problema ao achar nodeclickmanager");

        if (!mainCamera || !ropeRaw || !ropeA || !ropeB)
            Debug.Log("[CORDA] Componentes essenciais faltando");

        ropeDoubleAnchor = ropeRaw.GetComponent<RopeDoubleAnchor>();
    }

    void Update()
    {
        pulledNode = nodeClickManager.getPulledNode();

        if (pulledNode)
        {
            ropeRaw.GetComponent<LineRenderer>().enabled = true;
            ropeA.position = new Vector3
                (pulledNode.transform.position.x,
                pulledNode.transform.position.y,
                -0.15f);
        }
        else
        {
            ropeRaw.GetComponent<LineRenderer>().enabled = false;
            ropeA.position = new Vector3
                (mainCamera.ScreenToWorldPoint(Input.mousePosition).x,
                mainCamera.ScreenToWorldPoint(Input.mousePosition).y,
                -0.15f);
        }

        ropeB.position = new Vector3
                (mainCamera.ScreenToWorldPoint(Input.mousePosition).x,
                mainCamera.ScreenToWorldPoint(Input.mousePosition).y,
                -0.15f);

        ropeDoubleAnchor.ropeSegLen = Vector3.Distance(ropeA.position, ropeB.position) / 14;
    }
}
