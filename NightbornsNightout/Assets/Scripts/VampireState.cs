using UnityEngine;

public class VampireState : MonoBehaviour
{
    public enum VState { Walk, Bat, Feed }
    public Sprite vampireSprite;
    public Sprite batSprite;
    public float speedy = 0f;
    private int directiony = 1;
    public Rigidbody2D rb;
    private Vector3 originalScale;
    public SpriteRenderer sr;
    public float speed = 1.5f;
    private int direction = 1;
    public int hp = 10;
    public float feedTimer = 0f;
    public float feedDuration = 3f;
    private float stateTimer = 0f;
    public float batHeight = 2.4f;
    private float yOrigin;
    public GameObject targetFamiliar;
    public VState state = VState.Walk;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        yOrigin = transform.position.y;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        stateTimer += dt;
        if (hp <= 0) Destroy(gameObject);

        switch (state)
        {
            case VState.Walk:
                sr.color = new Color32(255, 255, 255, 255);
                if (stateTimer > 15f)
                {
                    stateTimer = 0f;
                    EnterBat();
                }
                break;

            case VState.Bat:
                sr.color = new Color32(255, 255, 255, 255);
                if (transform.position.y >= yOrigin + batHeight) directiony = -1;
                if (transform.position.y <= yOrigin) directiony = 1;
                if (stateTimer > 8f)
                {
                    stateTimer = 0f;
                    ExitBat();
                }
                break;

            case VState.Feed:
                sr.color = new Color32(255, 80, 80, 255);
                feedTimer += dt;
                if (feedTimer >= 2f)
                {
                    if (targetFamiliar)
                    {
                        targetFamiliar.SendMessage("EnterServe", SendMessageOptions.DontRequireReceiver);
                        targetFamiliar.SendMessage("ExitServe", SendMessageOptions.DontRequireReceiver);
                        var f = targetFamiliar.GetComponent<FamiliarState>();
                        if (f) f.Bites++;
                        hp = Mathf.Min(10, hp + 1);
                    }
                    feedTimer = 0f;
                    state = VState.Walk;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        float vx = direction * speed;
        float vy = (state == VState.Bat) ? directiony * speedy : 0f;
        rb.linearVelocity = new Vector2(vx, vy);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            direction *= -1;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, originalScale.y, originalScale.z);
        }

        if (state == VState.Walk && other.CompareTag("familiar"))
        {
            targetFamiliar = other.gameObject;
            state = VState.Feed;
            feedTimer = 0f;

        }
    }

    void EnterBat()
    {
        state = VState.Bat;
        stateTimer = 0f;
        sr.sprite = batSprite;
        yOrigin = transform.position.y;
        directiony = 1;
        speedy = 1.5f;
    }

    void ExitBat()
    {
        state = VState.Walk;
        stateTimer = 0f;
        sr.sprite = vampireSprite;
        transform.position = new Vector3(transform.position.x, yOrigin, transform.position.z);
        speedy = 0f;
    }


    public void TakeDamage(int amount)
    {
        hp -= amount;
        sr.color = new Color32(255, 0, 0, 255);
        Invoke(nameof(RecoverColor), 0.3f);
    }

    void RecoverColor()
    {
        sr.color = new Color32(180, 60, 200, 255);
    }
}
