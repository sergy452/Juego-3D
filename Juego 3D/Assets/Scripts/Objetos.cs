using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Objetos : MonoBehaviour
{
    Protagonista protagonista;
    // Este método se activa cuando algo entra en el área del objeto
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si lo que nos tocó tiene el tag "Player"
        if (other.CompareTag("Player") && gameObject != null)
        {
            Protagonista scriptProta = other.GetComponent<Protagonista>();
            if (scriptProta != null)
            {
                scriptProta.objetosRecogidos += 1;
                Debug.Log("¡Objeto recogido! Total: " + scriptProta.objetosRecogidos);
                Destroy(gameObject);
            }
        }
    }
}
