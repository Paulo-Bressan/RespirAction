using System.Collections.Generic;
using UnityEngine;

public class RopeDoubleAnchor : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();

    [Header("Configurações da Corda")]
    public Transform startPoint; // Arraste o objeto A aqui
    public Transform endPoint;   // Arraste o objeto B aqui
    public float ropeSegLen = 0.25f;
    public int segmentLength = 35;
    public float lineWidth = 0.1f;
    public int constraintIterations = 50; // Mais iterações = corda mais rígida

    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();

        
        this.lineRenderer.useWorldSpace = false;

        // Inicializa a corda começando no startPoint
        Vector3 ropeStartPoint = startPoint.position;

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    void Update()
    {
        this.DrawRope();
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        // SIMULATION (Gravidade)
        Vector2 forceGravity = new Vector2(0f, -1.5f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        // CONSTRAINTS (Restrições)
        for (int i = 0; i < constraintIterations; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        if (startPoint == null || endPoint == null) return;

        // 1. Ancorar o INÍCIO no objeto startPoint
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = startPoint.position; // Pega posição X,Y do startPoint
        this.ropeSegments[0] = firstSegment;

        // 2. Ancorar o FIM no objeto endPoint
        RopeSegment lastSegment = this.ropeSegments[this.segmentLength - 1];
        lastSegment.posNow = endPoint.position; // Pega posição X,Y do endPoint
        this.ropeSegments[this.segmentLength - 1] = lastSegment;

        // 3. Manter a distância entre os segmentos
        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;

            if (i != 0)
            {
                if (i + 1 == this.segmentLength - 1)
                {
                    firstSeg.posNow -= changeAmount;
                }
                else
                {
                    firstSeg.posNow -= changeAmount * 0.5f;
                    secondSeg.posNow += changeAmount * 0.5f;
                }

                this.ropeSegments[i] = firstSeg;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];

        // Pega a profundidade do objeto pai para referência
        float zDepth = (startPoint != null) ? startPoint.position.z : 0f;

        for (int i = 0; i < this.segmentLength; i++)
        {
            // Pega a posição GLOBAL calculada pela física
            Vector3 worldPos = this.ropeSegments[i].posNow;

            // Aplica o ajuste de profundidade 
            worldPos.z = zDepth - 0.1f;

            ropePositions[i] = this.transform.InverseTransformPoint(worldPos);
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}