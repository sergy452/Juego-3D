using UnityEngine;

public class Escondite : MonoBehaviour

{

    public Enemigo enemigo;

    void OnTriggerEnter(Collider other)

    {

        if (other.CompareTag("Player"))

        {

            // Antes de esconderlo, comprobamos si el bicho lo está viendo

            if (enemigo.PuedeVermeAhora())

            {

                // No le damos el beneficio de estar escondido

                enemigo.isPlayerHidden = true;

                // La lógica del enemigo en ChaseState se encargará del resto

            }

            else

            {

                enemigo.isPlayerHidden = true;

                Debug.Log("Te has escondido con éxito.");

            }

        }

    }

    void OnTriggerExit(Collider other)

    {

        if (other.CompareTag("Player"))

        {

            enemigo.isPlayerHidden = false;

        }

    }

}
