using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 0.1f;
    [SerializeField] private Vector2 xBounds = new Vector2(-10f, 10f);
    [SerializeField] private Vector2 zBounds = new Vector2(-10f, 10f);
    private Vector3 dragOrigin;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDrag();
#else
        HandleTouchDrag();
#endif

        ClampCameraPosition();
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragOrigin.x, 0, -pos.y * dragOrigin.y);

        transform.Translate(move, Space.World);
        dragOrigin = Input.mousePosition;
    }

    private void HandleTouchDrag()
    {
        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            dragOrigin = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector2 convertVec2 = new Vector2(dragOrigin.x, dragOrigin.y);
            Vector3 pos = cam.ScreenToViewportPoint(touch.position - convertVec2);
            Vector3 move = new Vector3(-pos.x, 0, -pos.y) * dragSpeed;

            transform.Translate(move, Space.World);
            dragOrigin = touch.position;
        }
    }

    void ClampCameraPosition()
    {
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, xBounds.x, xBounds.y);
        clampedPos.z = Mathf.Clamp(clampedPos.z, zBounds.x, zBounds.y);
        transform.position = clampedPos;
    }
}
