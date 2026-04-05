using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 5f;

    private void LateUpdate(){
        if(target == null){
            Debug.LogError("Target is not assigned to the CameraFollow script.");
            return;
        }
        Vector3 desiredPositon = target.position + offset;
        desiredPositon.z = -10;
        transform.position = Vector3.Lerp(transform.position, desiredPositon, smoothSpeed * Time.deltaTime);
        //transform.position = smoothedPosition;
    }
}
