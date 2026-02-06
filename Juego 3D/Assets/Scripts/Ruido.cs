using UnityEngine;

public class RuidoMaker : MonoBehaviour
{
    public Enemigo enemigo; // Arrastra al enemigo aquí

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemigo.HearNoise(transform.position);
        }
    }
}
