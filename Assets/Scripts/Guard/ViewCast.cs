using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCast : MonoBehaviour
{
     public float coneAngle = 55f; 
    public int rayCount = 10; 
    public float rayDistance = 5f;

    [Header("Detection Settings")]
    public LayerMask detectionMask;

    void Update()
    {
        DrawVisionCone();
    }

    void DrawVisionCone()
    {
        Vector2 forward = transform.up;

        float halfAngle = coneAngle / 2f;
        float startAngle = -halfAngle;
        float angleStep = coneAngle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, currentAngle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, rayDistance, detectionMask);

            Vector2 endPoint = hit ? hit.point : (Vector2)transform.position + dir * rayDistance;

            Debug.DrawLine(transform.position, endPoint, Color.red);

            if (hit.collider != null)
            {
                
            if(hit.collider.tag == "Player")
            {
                    Debug.Log("Player");
            }
            }
        }
    }
}
