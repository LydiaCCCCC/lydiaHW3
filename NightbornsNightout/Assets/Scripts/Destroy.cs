using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float lifetime = 1f;
    public AudioSource audioSource;
    public AudioClip sound;

    void Start()
    {
        Invoke(nameof(Explode), lifetime);
    }

    void Explode()
    {
        audioSource.PlayOneShot(sound);
        Destroy(gameObject);
    }
}
