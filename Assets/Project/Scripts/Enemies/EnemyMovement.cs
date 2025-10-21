using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent; // Componente de Nav Mesh para el movimiento del enemigo
    
    [Header("Configuración de Movimiento")]
    [SerializeField] private float walkSpeed = 3.5f; // Caminar velocidad
    [SerializeField] private float chaseSpeed = 5f; // Perseguir velocidad
    [SerializeField] private float stoppingDistance = 2f; // Distancia para detenerse cerca del objetivo
    
    private Vector3 startPosition; // Posición inicial del enemigo
    private bool isInitialized; // Indica si el agente ha sido inicializado correctamente
    
    public float CurrentSpeed => agent != null ? agent.velocity.magnitude : 0f; // Velocidad actual del enemigo
    public bool HasReachedDestination => agent != null && !agent.pathPending && agent.remainingDistance <= stoppingDistance; // Verifica si ha llegado al destino
    
    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // Obtener el componente NavMeshAgent si no está asignado
            
        startPosition = transform.position;
    }
    
    private void Start()
    {
        if (agent != null)
        {
            // Inicializar el agente con configuracion basica (agent.speed y stoppingDistance son requeridos para el navmesh)
            agent.speed = walkSpeed;
            agent.stoppingDistance = stoppingDistance;
            isInitialized = true;
        }
    }

    // Mueve el enemigo hacia un objetivo
    public void MoveToTarget(Vector3 targetPosition)
    {
        if (!isInitialized || agent == null) return;
        
        agent.isStopped = false; // Variable del navmesh para reanudar el movimiento
        agent.speed = chaseSpeed;
        agent.SetDestination(targetPosition);
    }
    
    // Detiene el movimiento del enemigo
    public void Stop()
    {
        if (!isInitialized || agent == null) return;
        
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // Velocidad = 0,0,0
    }
    
    // Patrulla hacia una posición
    public void Patrol(Vector3 patrolPosition)
    {
        if (!isInitialized || agent == null) return;
        
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(patrolPosition);
    }
    
    // Regresa a la posición inicial (tras perder al jugador)
    public void ReturnToStart()
    {
        if (!isInitialized || agent == null) return;
        
        Patrol(startPosition);
    }
    
    // Obtiene la distancia al objetivo
    public float GetDistanceToTarget(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition);
    }
    
    // Gira hacia el objetivo para verlo
    public void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Mantener rotación solo en el plano horizontal
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
