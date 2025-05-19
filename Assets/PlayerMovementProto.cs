using Mirror;
using UnityEngine;

public class PlayerMovementProto : NetworkBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // Log để kiểm tra hàm Update có chạy không
        // Debug.Log("Update called");

        if (!isLocalPlayer)
        {
            // Log để kiểm tra xem có phải local player không
            // Debug.Log("Not local player, returning.");
            return;
        }

        // Log để xác nhận đây là local player
        // Debug.Log("Is local player, processing input.");

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Log giá trị input
        // Debug.Log($"Input: H={horizontalInput}, V={verticalInput}");

        // Chỉ di chuyển nếu có input
        if (horizontalInput != 0 || verticalInput != 0)
        {
            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * moveSpeed * Time.deltaTime;

            // Log vector di chuyển
            // Debug.Log($"Moving by: {movement}");

            transform.position += movement;
        }
    }
}