using UnityEngine;
using Oculus.Interaction.Input;

public class HandPinchLocomotion : MonoBehaviour
{
    public HandRef rightHand;
    public Transform handDirection;
    public Transform playerBody;
    public float moveSpeed = 1.5f;
    public bool invert = false;

    void Update()
    {
        if (rightHand == null || handDirection == null) return;

        if (rightHand.GetFingerIsPinching(HandFinger.Index))
        {
            float yaw = handDirection.eulerAngles.y;
            Vector3 forward = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            if (invert) forward = -forward;

            Rigidbody rb = playerBody.GetComponent<Rigidbody>();
            rb.MovePosition(playerBody.position + forward * moveSpeed * Time.deltaTime);
        }
    }
}
