using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("Variables de deteccion")]
    [SerializeField] private float detectionRange = 10f; // Rango de detección del jugador
    [SerializeField] private float loseTargetRange = 15f; // Deteccion extendida para perder al jugador
    [SerializeField] private float attackRange = 2.5f; // Rango de ataque
    [SerializeField] private float fieldOfView = 120f; // Campo de visión del enemigo en grados
    [SerializeField] private LayerMask obstacleMask; // Máscara para detectar obstáculos entre el enemigo y el jugador
    
    [Header("Referencias")]
    [SerializeField] private Transform playerTransform; // Para ver al jugador
    
    private bool playerInSight; // Jugador a la vista?
    
    public Transform PlayerTransform => playerTransform; // Referencia al transform del jugador
    public float DetectionRange => detectionRange; // Referencia al rango de detección
    public float AttackRange => attackRange; // Referencia al rango de ataque
    
    private void Start()
    {
        // Buscar al jugador si no está asignado
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }
    
    /// Verifica si el jugador está en rango de detección
    public bool IsPlayerInRange()
    {
        if (playerTransform == null) return false; // No hay jugador en rango
        
        float distance = GetDistanceToPlayer();
        return distance <= detectionRange; // Devuelve true si está en rango
    }
    
    /// Verifica si se deja de seguir al jugador
    public bool ShouldLosePlayer()
    {
        if (playerTransform == null) return true; // No hay jugador a la vista, se pierde
        
        float distance = GetDistanceToPlayer();
        return distance > loseTargetRange;
    }
    
    /// Verifica si el jugador está en rango de ataque
    public bool IsPlayerInAttackRange()
    {
        if (playerTransform == null) return false;
        
        float distance = GetDistanceToPlayer();
        return distance <= attackRange;
    }
    
    /// Verifica si el jugador está en el campo de visión
    public bool IsPlayerInFieldOfView()
    {
        if (playerTransform == null) return false;
        
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        
        return angle < fieldOfView / 2f;
    }
    
    // Verifica si hay línea de visión directa al jugador
    public bool HasLineOfSight()
    {
        if (playerTransform == null) return false;
        
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, 
            out RaycastHit hit, detectionRange, obstacleMask))
        {
            return hit.transform == playerTransform;
        }
        
        return false;
    }
    
    // Obtiene la distancia al jugador
    public float GetDistanceToPlayer()
    {
        if (playerTransform == null) return Mathf.Infinity;
        
        return Vector3.Distance(transform.position, playerTransform.position);
    }
    
    // Detecta al jugador considerando rango y campo de visión
    public bool CanSeePlayer()
    {
        return IsPlayerInRange() && IsPlayerInFieldOfView() && HasLineOfSight();
    }
    
    // Dibuja el rango de detección en el editor (solo estetico)
    private void OnDrawGizmosSelected()
    {
        // Rango de detección
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Rango de perder objetivo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseTargetRange);
        
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}