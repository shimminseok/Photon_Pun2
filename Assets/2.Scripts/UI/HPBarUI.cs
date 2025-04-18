using UnityEngine;
using UnityEngine.UI;


public class HPBarUI : MonoBehaviour
{
    [SerializeField] RectTransform barRect;
    [SerializeField] Image fillImage;
    [SerializeField] Vector3 offset;

    Transform target;
    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(Transform _targetTrans)
    {
        target = _targetTrans;
        transform.SetParent(HealthBarManager.Instance.hpBarCanvas.transform);
    }

    public void UpdatePosion()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);
        barRect.position = screenPos;
    }


    public void UpdateFill(float _cur, float _max)
    {
        fillImage.fillAmount = Mathf.Clamp01(_cur / _max);
    }

    public void UnLink()
    {
        target = null;
        HealthBarManager.Instance.DespawnHealthBar(this);
        fillImage.fillAmount = 1f;
        barRect.position = Vector3.zero;
    }
}