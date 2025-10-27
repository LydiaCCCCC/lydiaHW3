using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject vampire;
    [SerializeField] private GameObject familiar;
    [SerializeField] private GameObject hunter;
    [SerializeField] private int maxFamiliars = 3;
    [SerializeField] private int maxHunters = 4;

    
    [SerializeField] private float hunterInterval = 10f;  
    [SerializeField] private float familiarInterval = 5f; 
    private float hunterTimer = 0f;
    private float familiarTimer = 0f;

    
    void Start()
    {
        
       // SpawnVampire(RandomPos());
        SpawnVampire(RandomPos());
        SpawnFamiliar(RandomPos());
        SpawnHunter(RandomPos());
        SpawnHunter(RandomPos());

        
        
    }

    void Update()
    {
        float dt = Time.deltaTime;

        Insurance();
        
        hunterTimer += dt;
        if (hunterTimer >= hunterInterval)
        {
            hunterTimer = 0f;
            if (CountTag("hunter") < maxHunters)
                SpawnHunter(RandomPos());
        }

        familiarTimer += dt;
        if (familiarTimer >= familiarInterval)
        {
            familiarTimer = 0f;
            if (CountTag("familiar") < maxFamiliars)
                SpawnFamiliar(RandomPos());
        }

        
        
    }

   
    int CountTag(string tag) => GameObject.FindGameObjectsWithTag(tag).Length;

    Vector3 RandomPos()
    {
        return new Vector3(
            Random.Range(-6.17f, 6.5f),
            -1.25f,
            0f
        );
    }

    void Insurance()
    {
        if (CountTag("familiar") <= 0 )
            SpawnFamiliar(RandomPos());
        if (CountTag("hunter") <= 0 )
            SpawnHunter(RandomPos());

        if (CountTag("vampire") <= 0)
        {
            if (CountTag("familiar") <= 0)
                SpawnFamiliar(RandomPos());
            Convert();
        }
    }

   
    public GameObject SpawnVampire(Vector3 pos)
    {
        return Instantiate(vampire, pos, Quaternion.identity, null);
    }

    public GameObject SpawnFamiliar(Vector3 pos)
    {
        
        if (CountTag("familiar") >= maxFamiliars) return null;
        return Instantiate(familiar, pos, Quaternion.identity, null);
    }

    public GameObject SpawnHunter(Vector3 pos)
    {
       
        if (CountTag("hunter") >= maxHunters) return null;
        return Instantiate(hunter, pos, Quaternion.identity, null);
    }

   
    void Convert()
    {
        var fams = GameObject.FindGameObjectsWithTag("familiar");
        if (fams.Length == 0) return;

        var pick = fams[Random.Range(0, fams.Length)];
        Vector3 pos = pick.transform.position;

        SpawnVampire(pos);
        Destroy(pick);
    }

    
}
