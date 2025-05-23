using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform followTarget;

    [Header("Offsets & Smoothing")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    [Tooltip("Higher = snappier")]
    public float smoothTime = 0.2f;

    [Header("Y-Follow")]
    public bool yFollow = true;
    public Vector2 yBounds = new Vector2(-Mathf.Infinity, Mathf.Infinity);

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (followTarget == null) return;

        Vector3 desiredPos = followTarget.position + offset;

        if (!yFollow)
            desiredPos.y = transform.position.y;
        else
            desiredPos.y = Mathf.Clamp(desiredPos.y, yBounds.x, yBounds.y);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
    }
}
