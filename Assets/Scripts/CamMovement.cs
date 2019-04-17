// Camera movement to follow player was learned from YouTube tutorial
// https://www.youtube.com/watch?v=MFQhpwc6cKE&t=3s
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 10f;
    public Vector3 offset;

    private void FixedUpdate()
    {
        if (player.position.z < -50)
        {
            offset.y = 9;
            offset.z = -16;
            transform.rotation = Quaternion.Euler(22,0,0);
        }
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
