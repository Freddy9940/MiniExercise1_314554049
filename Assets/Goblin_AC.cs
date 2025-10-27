using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Goblin_AC : MonoBehaviour 
{
    // --- 舊變數 ---
    private NavMeshAgent agent;
    private Animator animator;
    public Transform target;
    public int health = 10;
    private bool isDead = false;

    // --- NEW: 攻擊相關變數 ---
    public float attackRange = 1.5f; // 哥布林的攻擊距離
    public float turnSpeed = 10f; // 轉身面向敵人的速度

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // 【重要】設定 NavMeshAgent 的「停止距離」
        agent.stoppingDistance = attackRange;

        agent.updatePosition = true;

        // 【重要】我們現在要 "手動" 控制旋轉
        agent.updateRotation = false;
    }

    void Update()
    {
        if (isDead) return;
        if (target == null)
        {
            agent.isStopped = true;
            animator.SetFloat("ForwardSpeed", 0f);
            return;
        }

        // --- 核心邏輯：判斷 "距離" ---
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > agent.stoppingDistance)
        {
            // --- 情況 1：我們在 "範圍外" (太遠) ---
            agent.isStopped = false;
            agent.SetDestination(target.position);
            FaceVelocity(); // 轉向 "我們正在走" 的方向
        }
        else
        {
            // --- 情況 2：我們在 "範圍內" (夠近) ---
            agent.isStopped = true; // 停止 agent 移動
            FaceTarget(); // 轉向 "我們的目標"

            animator.SetTrigger("Attack");
        }

        // --- 動畫速度 (與上面邏輯分離) ---
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float forwardSpeed = localVelocity.z;
        animator.SetFloat("ForwardSpeed", forwardSpeed);

        // --- 舊的測試 (使用新版輸入系統) ---
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            TakeDamage(5);
        }
    }

    // --- NEW: 轉向 "目標" 的函數 ---
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // 忽略 Y 軸
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    // --- NEW: 轉向 "速度" 的函數 ---
    void FaceVelocity()
    {
        if (agent.velocity.sqrMagnitude > 0.01f) // 確定有在動
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }


    // --- 舊的傷害與死亡邏輯 (保持不變) ---
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        Debug.Log("哥布林承受 " + damage + " 點傷害, 剩下 " + health + " 血量");
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("哥布林死亡！");
        agent.isStopped = true;
        agent.enabled = false;
        animator.SetTrigger("Die");
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        Destroy(gameObject, 3.0f);
    }
}