using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform playerBody; // Player 本體，用來左右旋轉

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 鎖住滑鼠在畫面中間
    }

    void Update()
    {
        // 讀取滑鼠輸入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 控制上下視角 (Camera 自己)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 限制不要翻頭
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 控制左右轉身 (Player 本體)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
