using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class Tower : MonoBehaviour
{
    [SerializeField] InputActionAsset InputActions;
    InputAction interactAction;
    [SerializeField] float attackPower = 10f;
    [SerializeField] float attackInterval = 1f;
    [SerializeField] float attackRange = 5f;
    bool enchanted = false;
    float attackCooldown = 0f;
    [SerializeField] GameObject rangeIndicator;
    bool rangeIndicator_showing = false;
    [SerializeField] GameObject bulletPrefab;

    // Update is called once per frame
    void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown < 0) attackCooldown = 0;

        // Show range
        if (interactAction.WasPressedThisFrame())
        {
            Debug.Log("pressed");
            if (rangeIndicator_showing)
            {
                rangeIndicator.SetActive(false);
                rangeIndicator_showing = false;
            }
            else
            {
                rangeIndicator.transform.localScale = new(attackRange, 0.2f, attackRange);
                rangeIndicator.SetActive(true);
                rangeIndicator_showing = true;
            }
        }

        // Try to attack
        if (attackCooldown == 0)
        {
            GameObject enemy = SearchForEnemy();
            if (!enemy) return;
            // TODO: Attack
            Attack(enemy, attackPower, 25);
            Debug.Log("Attack");
            attackCooldown = attackInterval;
        }
    }

    GameObject SearchForEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> enemiesInRange = new();
        foreach (GameObject enemy in enemies)
        {
            Vector3 towerPosition = transform.position;
            Vector3 enemyPosition = enemy.transform.position;
            towerPosition.y = 0;
            enemyPosition.y = 0;
            if ((enemyPosition - towerPosition).magnitude < attackRange)
            {
                enemiesInRange.Add(enemy);
            }
        }
        if (enemiesInRange.Count == 0) return null;

        GameObject target = null;

        // TODO: choose an enemy
        target = enemiesInRange[Random.Range(0, enemiesInRange.Count)];

        return target;
    }

    void Attack(GameObject target, float damage, float speed)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform);
        bullet.transform.position = transform.position + Vector3.up * 5;
        bullet.GetComponent<Bullet>().Shoot(target, damage, speed);
    }

    void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        interactAction = InputActions.FindAction("Interact");
    }
}
