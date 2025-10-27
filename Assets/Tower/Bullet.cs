using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 targetPosition = new(0, 0, 0);
    bool flying = false;
    float damage = 10f;
    float speed = 1f;

    public void Shoot(GameObject target, float _damage, float _speed)
    {
        targetPosition = target.transform.position;
        transform.LookAt(target.transform);
        flying = true;
        damage = _damage;
        speed = _speed;
    }

    void Land()
    {
        flying = false;
        DealDamage();
        Debug.Log("Land");
        Destroy(gameObject);
    }

    void DealDamage()
    {
        // TODO: Target take damage
    }

    // Update is called once per frame
    void Update()
    {
        if (flying)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, targetPosition, speed * Time.deltaTime);
            if (transform.position == targetPosition) Land();
        }
    }
}
