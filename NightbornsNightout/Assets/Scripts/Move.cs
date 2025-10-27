using UnityEngine;

public class Move : MonoBehaviour
{

    public float speed;
    public float speedy = 0;
    private int direction = 1;
    private int directiony = 1;
    public Rigidbody2D rb;

    private Vector3 originalScale;
    
    void Start()
    {
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
       rb.linearVelocity = new Vector2(direction * speed, directiony * speedy);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))

        {
            direction *= -1;
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x) * direction,
                originalScale.y,
                originalScale.z
            );
        }
        
        if (other.CompareTag("Fly"))

        {
            directiony *= -1;
            
        }
    }
}
