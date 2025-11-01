using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    public List<GameObject> guardNodes;
    
    public float moveSpeed = 3f;
    public float turnSpeed = 360f; // degrees per second for fast turns
    public float sweepTurnSpeed = 60f; // degrees per second for slow scanning
    public float waitAtPeak = 1f; // how long to pause at sweep extremes

    private int currentNodeIndex = 0;
    private bool forward = true;
    private bool isSweeping = false;

    private void Start()
    {
        if (guardNodes == null || guardNodes.Count == 0)
        {
            enabled = false;
            return;
        }

        transform.position = guardNodes[0].transform.position;
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        if (guardNodes.Count == 1)
        {
            yield return SweepAtNode(guardNodes[0]);
            yield break;
        }

        while (true)
        {
            GameObject targetNode = guardNodes[currentNodeIndex];
            Nodes nodeData = targetNode.GetComponent<Nodes>();

            if (Vector2.Distance(transform.position, targetNode.transform.position) > 0.1f)
                yield return MoveToNode(targetNode);

            if (nodeData != null && nodeData.Angle > 0)
                yield return SweepAtNode(targetNode);

            if (forward)
                currentNodeIndex++;
            else
                currentNodeIndex--;

            // Reverse direction if reached ends
            if (currentNodeIndex >= guardNodes.Count)
            {
                currentNodeIndex = guardNodes.Count - 2;
                forward = false;
            }
            else if (currentNodeIndex < 0)
            {
                currentNodeIndex = 1;
                forward = true;
            }

            yield return null;
        }
    }

    private IEnumerator MoveToNode(GameObject node)
    {
        Vector3 target = node.transform.position;

        // Turn quickly to face the node
        yield return RotateTowards(target, turnSpeed);

        // Move until close enough
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SweepAtNode(GameObject node)
    {
        Nodes nodeData = node.GetComponent<Nodes>();
        float sweepAngle = nodeData != null ? nodeData.Angle : 0f;

        if (sweepAngle <= 0f)
            yield break;

        isSweeping = true;

        Quaternion centerRot = transform.rotation;
        Quaternion leftRot = Quaternion.Euler(0, 0, centerRot.eulerAngles.z + sweepAngle);
        Quaternion rightRot = Quaternion.Euler(0, 0, centerRot.eulerAngles.z - sweepAngle);

        while (isSweeping)
        {
            // Sweep right
            yield return RotateTowards(rightRot, sweepTurnSpeed);
            yield return new WaitForSeconds(waitAtPeak);

            // Sweep left
            yield return RotateTowards(leftRot, sweepTurnSpeed);
            yield return new WaitForSeconds(waitAtPeak);

            // Sweep back to center
            yield return RotateTowards(centerRot, sweepTurnSpeed);
            if (guardNodes.Count > 1)
            {
                yield return new WaitForSeconds(waitAtPeak);
            }

            // For one node, loop forever; for multiple, only do once per node
            if (guardNodes.Count > 1)
                break;
        }

        isSweeping = false;
    }

    private IEnumerator RotateTowards(Vector3 targetPos, float speed)
    {
        Vector3 direction = targetPos - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // for 2D up-facing
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        yield return RotateTowards(targetRotation, speed);
    }

    private IEnumerator RotateTowards(Quaternion targetRotation, float speed)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
            yield return null;
        }
    }
}
