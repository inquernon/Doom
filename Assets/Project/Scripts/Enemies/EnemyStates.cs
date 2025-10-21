using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,       // Esperando
        Patrol,     // Patrullando
        Chase,      // Persiguiendo al jugador
        Attack,     // Atacando
        Dead        // Muerto
    }
    
    [Header("Estado Actual")]
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    
    [Header("Configuración de Patrulla")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    
    private EnemyState previousState;
    private int currentPatrolIndex;
    private float patrolWaitTimer;
    
    public EnemyState CurrentState => currentState;
    
    // Cambia el estado del enemigo
    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        
        // Salir del estado anterior
        OnStateExit(currentState);
        
        previousState = currentState;
        currentState = newState;
        
        // Entrar al nuevo estado
        OnStateEnter(currentState);
    }
    
    /// Lógica al entrar a un estado
    private void OnStateEnter(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                patrolWaitTimer = patrolWaitTime;
                break;
                
            case EnemyState.Patrol:
                SetNextPatrolPoint();
                break;
                
            case EnemyState.Chase:
                break;
                
            case EnemyState.Attack:
                break;
                
            case EnemyState.Dead:
                break;
        }
    }
    
    // Lógica al salir de un estado
    private void OnStateExit(EnemyState state)
    {
        // Lógica de limpieza si es necesaria
    }
    
    // Actualiza el temporizador de patrulla
    public bool UpdatePatrolWait()
    {
        patrolWaitTimer -= Time.deltaTime;
        return patrolWaitTimer <= 0;
    }
    
    // Obtiene el siguiente punto de patrulla
    public Vector3 GetCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return transform.position;
            
        return patrolPoints[currentPatrolIndex].position;
    }
    
    // Establece el siguiente punto de patrulla
    private void SetNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }
    
    // Avanza al siguiente punto de patrulla
    public void MoveToNextPatrolPoint()
    {
        SetNextPatrolPoint();
        patrolWaitTimer = patrolWaitTime;
    }
}