using UnityEngine;

public class HunterBullet : MonoBehaviour
{
    private int dirX = 1;
    private float speed = 6f;
    private float life = 3f;
    private string vampireTag = "vampire";
    private float t = 0f;
    private Rigidbody2D rb;

    public void Init(int dir, float spd, float lifetime, string targetTag)
    {
        dirX = dir >= 0 ? 1 : -1;
        speed = spd;
        life = lifetime;
        vampireTag = targetTag;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t >= life) Destroy(gameObject);
        rb.linearVelocity = new Vector2(dirX * speed, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(vampireTag))
        {
            other.SendMessage("TakeDamage", 2, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
