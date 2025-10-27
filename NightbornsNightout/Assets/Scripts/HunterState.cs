using UnityEngine;

public class HunterState : MonoBehaviour
{
    public enum HState { Search, Attack, Heal }

    public Rigidbody2D rb;
    public SpriteRenderer sr;

    public float speed = 2f;
    private int direction = 1;
    private Vector3 originalScale;

    public int hp = 2;
    public int bullets = 3;

    public GameObject bulletPrefab;
    public float bulletSpeed = 6f;
    public float bulletLife = 3f;

    public float aimTime = 1f;
    private float aimTimer = 0f;
    private GameObject vampire;

    private float healTimer = 0f;
    private float ammoT = 0f;
    private float hpT = 0f;

    public HState state = HState.Search;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (hp <= 0) Destroy(gameObject);

        switch (state)
        {
            case HState.Search:
                sr.color = new Color32(255, 255, 255, 255);
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
                healTimer += dt;
                if (healTimer >= 9f) {
                    EnterHeal();
                    healTimer = 0f;
                    }
                break;

            case HState.Attack:
                sr.color = new Color32(255, 200, 120, 255);
                rb.linearVelocity = Vector2.zero;
                aimTimer += dt;
                if (aimTimer >= aimTime)
                {
                    Shoot();
                    EnterSearch();
                }
                break;

            case HState.Heal:
                sr.color = new Color32(200, 200, 200, 255);
                rb.linearVelocity = Vector2.zero;
                ammoT += dt;
                healTimer += dt;
                
                if (ammoT >= 0.8f && bullets < 3)
                {
                    ammoT = 0f;
                    bullets++;
                    hpT += dt;
                }
                else if (hpT >= 2f && hp < 2)
                {
                    hpT = 0f;
                    hp++;
                }
                if (bullets >= 3 || healTimer >= 2f) EnterSearch();
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            direction *= -1;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, originalScale.y, originalScale.z);
        }

        if (state == HState.Search && other.CompareTag("vampire"))
        {
            if (bullets <= 0) return;
            vampire = other.gameObject;
            direction = (vampire.transform.position.x >= transform.position.x) ? 1 : -1;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, originalScale.y, originalScale.z);
            EnterAttack();
        }
    }

    void Shoot()
    {
        if (bullets <= 0) return;
        int dirX = direction;
        if (vampire) dirX = (vampire.transform.position.x >= transform.position.x) ? 1 : -1;
        var b = Instantiate(bulletPrefab, new Vector3(transform.position.x, -0.05f, transform.position.z), Quaternion.identity);
        var proj = b.GetComponent<HunterBullet>();
        if (!proj) proj = b.AddComponent<HunterBullet>();
        proj.Init(dirX, bulletSpeed, bulletLife, "vampire");
        bullets = Mathf.Max(0, bullets - 1);
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        sr.color = new Color32(255, 0, 0, 255);
        Invoke(nameof(RecoverColor), 0.3f);
    }

    void RecoverColor()
    {
        sr.color = new Color32(120, 220, 255, 255);
    }

    void EnterSearch()
    {
        state = HState.Search;
        aimTimer = 0f;
        healTimer = Random.Range(1f, 3f);
        vampire = null;
    }

    void EnterAttack()
    {
        state = HState.Attack;
        aimTimer = 0f;
    }

    void EnterHeal()
    {
        state = HState.Heal;
        ammoT = 0f;
        hpT = 0f;
    }
}
