using UnityEngine;

/// <summary>
/// Controlador principal de la IA del enemigo tipo Doom
/// Orquesta los componentes de movimiento, detección y animación
/// Principio de Responsabilidad Única: Solo coordina los componentes
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(EnemyStates))]
public class EnemyAIController : MonoBehaviour
{
    // Referencias a componentes (Inyección de Dependencias)
    private EnemyMovement movement;
    private EnemyDetection detection;
    private EnemyAnimation animationController;
    private EnemyStates stateMachine;
    
    [Header("Estado")]
    [SerializeField] private bool isDead = false;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    public bool IsDead => isDead;
    
    private void Awake()
    {
        // Obtener referencias a componentes
        movement = GetComponent<EnemyMovement>();
        detection = GetComponent<EnemyDetection>();
        animationController = GetComponent<EnemyAnimation>();
        stateMachine = GetComponent<EnemyStates>();
    }
    
    private void Update()
    {
        // Si está muerto, no actualizar comportamiento
        if (isDead)
        {
            if (stateMachine.CurrentState != EnemyStates.EnemyState.Dead)
            {
                OnDeath();
            }
            return;
        }
        
        // Actualizar el comportamiento según el estado actual
        UpdateCurrentState();
        
        // Actualizar animaciones
        UpdateAnimations();
    }
    
    /// <summary>
    /// Actualiza la lógica según el estado actual
    /// </summary>
    private void UpdateCurrentState()
    {
        switch (stateMachine.CurrentState)
        {
            case EnemyStates.EnemyState.Idle:
                UpdateIdleState();
                break;
                
            case EnemyStates.EnemyState.Patrol:
                UpdatePatrolState();
                break;
                
            case EnemyStates.EnemyState.Chase:
                UpdateChaseState();
                break;
                
            case EnemyStates.EnemyState.Attack:
                UpdateAttackState();
                break;
                
            case EnemyStates.EnemyState.Dead:
                UpdateDeadState();
                break;
        }
    }
    
    /// <summary>
    /// Estado Idle: Enemigo esperando
    /// </summary>
    private void UpdateIdleState()
    {
        movement.Stop();
        
        // Detectar jugador
        if (detection.CanSeePlayer())
        {
            if (showDebugInfo)
                Debug.Log($"{gameObject.name}: ¡Jugador detectado! Cambiando a Chase");
                
            stateMachine.ChangeState(EnemyStates.EnemyState.Chase);
            return;
        }
        
        // Cambiar a patrulla después del tiempo de espera
        if (stateMachine.UpdatePatrolWait())
        {
            if (showDebugInfo)
                Debug.Log($"{gameObject.name}: Iniciando patrulla");
                
            stateMachine.ChangeState(EnemyStates.EnemyState.Patrol);
        }
    }
    
    /// <summary>
    /// Estado Patrol: Enemigo patrullando
    /// </summary>
    private void UpdatePatrolState()
    {
        // Detectar jugador primero
        if (detection.CanSeePlayer())
        {
            if (showDebugInfo)
                Debug.Log($"{gameObject.name}: ¡Jugador detectado durante patrulla! Cambiando a Chase");
                
            stateMachine.ChangeState(EnemyStates.EnemyState.Chase);
            return;
        }
        
        // Moverse al punto de patrulla
        Vector3 patrolPoint = stateMachine.GetCurrentPatrolPoint();
        movement.Patrol(patrolPoint);
        
        // Si llegó al punto, cambiar a Idle
        if (movement.HasReachedDestination)
        {
            if (showDebugInfo)
                Debug.Log($"{gameObject.name}: Punto de patrulla alcanzado");
                
            stateMachine.MoveToNextPatrolPoint();
            stateMachine.ChangeState(EnemyStates.EnemyState.Idle);
        }
    }
    
    /// <summary>
    /// Estado Chase: Persiguiendo al jugador
    /// </summary>
    private void UpdateChaseState()
    {
        if (detection.PlayerTransform == null)
        {
            if (showDebugInfo)
                Debug.Log($"{gameObject.name}: Jugador no encontrado, volviendo a Idle");
                
            stateMachine.ChangeState(EnemyStates.EnemyState.Idle);
            return;
        }
        
        // Verificar si debe perder al jugador
        if (detection.ShouldLosePlayer())
        {
            if (showDebugInfo)
                Debug.Log($"{gameObject.name}: Jugador fuera de rango, perdiendo objetivo");
                
            stateMachine.ChangeState(EnemyStates.EnemyState.Idle);
            return;
        }
        
        // Verificar si está en rango de ataque
        if (detection.IsPlayerInAttackRange())
        {
            if (showDebugInfo)
                Debug.Log($"{gameObject.name}: Jugador en rango de ataque");
                
            stateMachine.ChangeState(EnemyStates.EnemyState.Attack);
            return;
        }
        
        // Perseguir al jugador
        movement.MoveToTarget(detection.PlayerTransform.position);
    }
    
    /// <summary>
    /// Estado Attack: En rango de ataque del jugador
    /// </summary>
    private void UpdateAttackState()
    {
        if (detection.PlayerTransform == null)
        {
            stateMachine.ChangeState(EnemyStates.EnemyState.Idle);
            return;
        }
        
        // Detener movimiento y rotar hacia el jugador
        movement.Stop();
        movement.LookAtTarget(detection.PlayerTransform.position);
        
        // Verificar si el jugador salió del rango de ataque
        if (!detection.IsPlayerInAttackRange())
        {
            if (detection.IsPlayerInRange())
            {
                if (showDebugInfo)
                    Debug.Log($"{gameObject.name}: Jugador fuera de rango de ataque, continuando persecución");
                    
                stateMachine.ChangeState(EnemyStates.EnemyState.Chase);
            }
            else
            {
                if (showDebugInfo)
                    Debug.Log($"{gameObject.name}: Jugador muy lejos, volviendo a Idle");
                    
                stateMachine.ChangeState(EnemyStates.EnemyState.Idle);
            }
            return;
        }
        
        // Aquí se implementará la lógica de ataque cuando añadas el sistema de combate
        // Por ahora solo reproduce la animación
        animationController.PlayAttack();
    }
    
    /// Actualiza las animaciones según el movimiento
    private void UpdateAnimations()
    {
        if (isDead) return;
        
        float speed = movement.CurrentSpeed;
        animationController.SetMovementSpeed(speed);
    }
    
    /// Estado Dead: Enemigo muerto
    private void UpdateDeadState()
    {
        // El enemigo está muerto, no hace nada más
        // Este método existe para mantener consistencia en la máquina de estados
    }
    
    /// Maneja la muerte del enemigo
    /// Este método será llamado por el sistema de combate cuando lo implementes
    private void OnDeath()
    {
        if (stateMachine.CurrentState == EnemyStates.EnemyState.Dead) return;
        
        if (showDebugInfo)
            Debug.Log($"{gameObject.name}: ¡Enemigo eliminado!");
        
        stateMachine.ChangeState(EnemyStates.EnemyState.Dead);
        movement.Stop();
        animationController.PlayDeath();
        
        // Desactivar después de la animación de muerte (ajusta el tiempo según tu animación)
        Destroy(gameObject, 3f);
    }
    
    /// Marca al enemigo como muerto
    /// Llama este método desde tu sistema de combate cuando la salud llegue a 0
    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
    }
    
    /// Método público para que otros sistemas puedan "matar" al enemigo
    /// Útil para testing o eventos especiales
    public void KillEnemy()
    {
        Die();
    }
    
    /// Obtiene el estado actual para debugging o UI
    public EnemyStates.EnemyState GetCurrentState()
    {
        return stateMachine.CurrentState;
    }
    
    /// Fuerza un cambio de estado (útil para debugging o eventos externos)
    public void ForceStateChange(EnemyStates.EnemyState newState)
    {
        stateMachine.ChangeState(newState);
    }
    
    // Visualización de información de debug en el editor
    private void OnDrawGizmos()
    {
        if (!showDebugInfo || !Application.isPlaying) return;
        
        // Mostrar estado actual sobre el enemigo
        Vector3 textPosition = transform.position + Vector3.up * 3f;
        
#if UNITY_EDITOR
        UnityEditor.Handles.Label(textPosition, 
            $"Estado: {(stateMachine != null ? stateMachine.CurrentState.ToString() : "N/A")}\n" +
            $"Velocidad: {(movement != null ? movement.CurrentSpeed.ToString("F1") : "N/A")}");
#endif
    }
}