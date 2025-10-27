using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Goblin_AC : MonoBehaviour 
{
    // --- ���ܼ� ---
    private NavMeshAgent agent;
    private Animator animator;
    public Transform target;
    public int health = 10;
    private bool isDead = false;

    // --- NEW: ���������ܼ� ---
    public float attackRange = 1.5f; // �����L�������Z��
    public float turnSpeed = 10f; // �ਭ���V�ĤH���t��

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // �i���n�j�]�w NavMeshAgent ���u����Z���v
        agent.stoppingDistance = attackRange;

        agent.updatePosition = true;

        // �i���n�j�ڭ̲{�b�n "���" �������
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

        // --- �֤��޿�G�P�_ "�Z��" ---
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > agent.stoppingDistance)
        {
            // --- ���p 1�G�ڭ̦b "�d��~" (�ӻ�) ---
            agent.isStopped = false;
            agent.SetDestination(target.position);
            FaceVelocity(); // ��V "�ڭ̥��b��" ����V
        }
        else
        {
            // --- ���p 2�G�ڭ̦b "�d��" (����) ---
            agent.isStopped = true; // ���� agent ����
            FaceTarget(); // ��V "�ڭ̪��ؼ�"

            animator.SetTrigger("Attack");
        }

        // --- �ʵe�t�� (�P�W���޿����) ---
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float forwardSpeed = localVelocity.z;
        animator.SetFloat("ForwardSpeed", forwardSpeed);

        // --- �ª����� (�ϥηs����J�t��) ---
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            TakeDamage(5);
        }
    }

    // --- NEW: ��V "�ؼ�" ����� ---
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // ���� Y �b
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    // --- NEW: ��V "�t��" ����� ---
    void FaceVelocity()
    {
        if (agent.velocity.sqrMagnitude > 0.01f) // �T�w���b��
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }


    // --- �ª��ˮ`�P���`�޿� (�O������) ---
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        Debug.Log("�����L�Ө� " + damage + " �I�ˮ`, �ѤU " + health + " ��q");
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("�����L���`�I");
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