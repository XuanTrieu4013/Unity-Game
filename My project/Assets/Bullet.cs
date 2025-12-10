using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    public void SetDirection(Vector2 dir)
    {
        rb.velocity = dir.normalized * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime); // tự hủy sau lifeTime giây
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu va chạm với Player thì hủy đạn
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}