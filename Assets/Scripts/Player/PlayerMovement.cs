using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f; // 跳躍力量
    private Rigidbody rb;
    private Animator animator;

    public Transform cameraTransform; // 拖入 Main Camera
    private Vector3 moveInput;

    private bool isGrounded; // 判斷是否在地面

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 先用相對攝影機的方向計算
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveInput = (forward * moveZ + right * moveX).normalized;

        // 設定動畫參數
        if (animator != null)
        {
            animator.SetFloat("VerticalSpeed", moveZ);
            animator.SetFloat("HorizenSpeed", moveX);
            
            animator.SetBool("IsJumping", !isGrounded);
        }

        // 按空白鍵跳
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // 水平移動
        Vector3 horizontalMove = new Vector3(moveInput.x, 0f, moveInput.z);
        rb.MovePosition(rb.position + horizontalMove * moveSpeed * Time.fixedDeltaTime);
    }

    // 檢測玩家是否在地面
    private void OnCollisionStay(Collision collision)
    {
        // 假設地板 tag 是 "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
