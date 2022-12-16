using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceTransform : MonoBehaviour
{
    [SerializeField]
    private Vector2 refResolution = new Vector2(1920, 1080);
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private bool startRight = false;
    [SerializeField]
    private bool startTop = false;

    void SetPosition()
    {
        Camera cam = Camera.main;
        Rect camSize = cam.pixelRect;

        Vector3 scaledOffset = Vector2.Scale(offset, Vector2.Scale(new Vector2(camSize.width, camSize.height), new Vector2(1 / refResolution.x, 1 / refResolution.y)));
        
        Vector2 screenPos = new Vector2(startRight ? cam.pixelWidth - scaledOffset.x : scaledOffset.x, startTop ? cam.pixelHeight - scaledOffset.y : scaledOffset.y);


        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, offset.z));

        transform.position = worldPosition;
    }

    private void OnDrawGizmosSelected()
    {
        SetPosition();
    }
}
