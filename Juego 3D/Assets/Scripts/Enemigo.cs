using UnityEngine;

using UnityEngine.AI;

public class Enemigo : MonoBehaviour

{

    public enum State { Wander, Chase, Investigate, Search }

    [Header("Estado Actual")]

    public State currentState = State.Wander;

    [Header("Referencias")]

    public Transform player;

    public LayerMask obstacleMask;

    private NavMeshAgent agent;

    private MeshRenderer meshRenderer;

    [Header("Configuración de Visión")]

    public float visionRange = 15f;

    public float visionAngle = 50f;

    [Header("Configuración de Tiempos")]

    public float searchDuration = 5f;

    public float chaseRetainTime = 2f;

    private float timer = 0f;

    [HideInInspector] public bool isPlayerHidden = false;

    private Vector3 lastKnownPosition;

    void Awake()

    {

        agent = GetComponent<NavMeshAgent>();

        meshRenderer = GetComponent<MeshRenderer>();

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void Update()

    {

        switch (currentState)

        {

            case State.Wander: UpdateWander(); break;

            case State.Chase: UpdateChase(); break;

            case State.Investigate: UpdateInvestigate(); break;

            case State.Search: UpdateSearch(); break;

        }

    }

    void UpdateWander()

    {

        CambiarColor(Color.green);

        if (!agent.hasPath || agent.remainingDistance < 0.5f)

        {

            Vector3 randomPoint = Random.insideUnitSphere * 12f + transform.position;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 12f, NavMesh.AllAreas))

                agent.SetDestination(hit.position);

        }

        if (PuedeVermeAhora() && !isPlayerHidden) currentState = State.Chase;

    }

    void UpdateChase()

    {

        CambiarColor(Color.red);

        if (PuedeVermeAhora() && !isPlayerHidden)

        {

            // Mientras te vea, actualiza tu posición constantemente

            lastKnownPosition = player.position;

            agent.SetDestination(lastKnownPosition);

            timer = chaseRetainTime; // Reseteamos el contador de "retardo"

        }

        else

        {

            // Si te pierde de vista, sigue corriendo a la última posición conocida

            agent.SetDestination(lastKnownPosition);

            // Empezamos a descontar el tiempo de retardo

            timer -= Time.deltaTime;

            // Solo cuando el tiempo se agota O llega al sitio, pasa a buscar

            if (timer <= 0 || agent.remainingDistance < 0.6f)

            {

                timer = searchDuration; // Iniciamos el tiempo de búsqueda (mirar lados)

                currentState = State.Search;

            }

        }

        // Si te escondes en su cara

        if (isPlayerHidden && PuedeVermeAhora())

        {

            Debug.Log("¡Te vi entrar!");

        }

    }

    void UpdateSearch()

    {

        CambiarColor(Color.yellow);

        // En búsqueda ya no corre, se queda por la zona

        agent.SetDestination(lastKnownPosition);

        if (agent.remainingDistance < 0.6f)

        {

            timer -= Time.deltaTime;

            if (timer <= 0) currentState = State.Wander;

        }

        if (PuedeVermeAhora() && !isPlayerHidden) currentState = State.Chase;

    }

    void UpdateInvestigate()

    {

        CambiarColor(Color.yellow);

        agent.SetDestination(lastKnownPosition);

        if (agent.remainingDistance < 0.6f)

        {

            timer -= Time.deltaTime;

            if (timer <= 0) currentState = State.Wander;

        }

        if (PuedeVermeAhora() && !isPlayerHidden) currentState = State.Chase;

    }

    public bool PuedeVermeAhora()

    {

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > visionRange) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, directionToPlayer) < visionAngle)

        {

            if (!Physics.Linecast(transform.position + Vector3.up * 1.5f, player.position + Vector3.up, obstacleMask))

                return true;

        }

        return false;

    }

    public void HearNoise(Vector3 location)

    {

        if (currentState != State.Chase)

        {

            lastKnownPosition = location;

            timer = 3f;

            currentState = State.Investigate;

        }

    }

    void CambiarColor(Color c)

    {

        if (meshRenderer.material.color != c) meshRenderer.material.color = c;

    }

    private void OnDrawGizmosSelected()

    {

        // Visualizar el rango y ángulo de visión en el Editor

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;

        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * transform.forward;

        Gizmos.DrawRay(transform.position + Vector3.up, leftBoundary * visionRange);

        Gizmos.DrawRay(transform.position + Vector3.up, rightBoundary * visionRange);

    }

}
