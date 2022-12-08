using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceTransform : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private bool startRight = false;
    [SerializeField]
    private bool startTop = false;

    void SetPosition()
    {
        Vector2 screenPos = new Vector2(startRight ? Camera.main.pixelWidth - offset.x : offset.x, startTop ? Camera.main.pixelHeight - offset.y : offset.y);


        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, offset.z));

        transform.position = worldPosition;
    }

    private void OnDrawGizmosSelected()
    {
        SetPosition();
    }
}
