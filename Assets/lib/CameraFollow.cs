using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;
    
    [Header("跟随设置")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("边界限制（可选）")]
    public bool useBounds = false;
    public float minX, maxX;
    public float minY, maxY;
    
    void LateUpdate()
    {
        if (target == null)
            return;
        
        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;
        
        // 平滑跟随
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // 如果启用边界限制
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }
        
        transform.position = smoothedPosition;
    }
}
