using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowsPlayer : MonoBehaviour
{
    [SerializeField]
    private PlayerControl control;
    [SerializeField]
    private float aheadCameraDistance = 15f;
    [SerializeField]
    private float verticalOffset = 0f;

    [SerializeField]
    private float smoothTimeX = 0.2f;
    [SerializeField]
    private float smoothTimeY = 0.1f;

    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;


    private Vector2 target;


    // Update is called once per frame
    void LateUpdate()
    {
        switch (control.GetDirection())
        {
            case Direction.left:
                target = new Vector2(control.transform.position.x - aheadCameraDistance, control.transform.position.y);
                break;

            case Direction.right:
                target = new Vector2(control.transform.position.x + aheadCameraDistance, control.transform.position.y);
                break;

            default:
                target = control.gameObject.transform.position;
                break;
        }

        float posX = Mathf.SmoothDamp(transform.position.x, target.x, ref xVelocity, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, target.y + verticalOffset, ref yVelocity, smoothTimeY);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(control.transform.position.x + aheadCameraDistance, control.transform.position.y), 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(control.transform.position.x - aheadCameraDistance, control.transform.position.y), 0.5f);
    }
}
