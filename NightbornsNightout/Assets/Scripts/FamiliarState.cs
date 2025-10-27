using UnityEngine;

public class FamiliarState : MonoBehaviour
{
    public enum FState { Walk, Serve, Attack }
    public bool IsServing => state == FState.Serve;

    public Rigidbody2D rb;
    public SpriteRenderer sr;

    public float speed;
    private int direction = 1;
    public GameObject vampireObj;
    public int Bites = 0;

    public GameObject hunter;
    private HunterState hunterState;
    public GameObject questionMark;

    private Vector3 originalScale;

    public FState state = FState.Walk;

    public float detctCD;
    private float detectT = 0f;

    public float attackTime = 1f;

    public float serveTimer;
    public int serveT = 2;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Bites >= 2)
        {
            Instantiate(vampireObj, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        float dt = Time.deltaTime;

        switch (state)
        {
            case FState.Walk:
                sr.color = new Color32(255, 255, 255, 255);
                Move();
                if (detectT > 0f) detectT -= dt;
                attackTime = 1f;
                break;

            case FState.Serve:
                sr.color = new Color32(209, 44, 44, 255);
                break;

            case FState.Attack:
                sr.color = new Color32(255, 253, 153, 255);
                attackTime -= dt;
                if (attackTime <= 0)
                {
                    int rollAttack = Random.Range(0, 4);
                    if (rollAttack > 1 && hunterState != null)
                    {
                        hunterState.TakeDamage(1);
                    }
                    state = FState.Walk;
                    hunter = null;
                    hunterState = null;
                    detectT = detctCD;
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state == FState.Walk && detectT <= 0f)
        {
            if (other.CompareTag("hunter"))
            {
                int rollDetect = Random.Range(0, 4);
                if (rollDetect > 1)
                {
                    state = FState.Attack;
                    hunter = other.gameObject;
                    hunterState = other.GetComponentInParent<HunterState>();
                }
                else
                {
                    Instantiate(questionMark, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);
                }
                detectT = detctCD;
            }
        }

        if (other.CompareTag("Wall"))
        {
            direction *= -1;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, originalScale.y, originalScale.z);
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    public void EnterServe()
    {
        state = FState.Serve;
    }

    public void ExitServe()
    {
        state = FState.Walk;
    }
}
