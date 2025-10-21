using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [Header("Componente de Animación")]
    [SerializeField] private Animator animator; // Componente Animator para manejar las animaciones
    
    [Header("Nombres de Animaciones")]
    [SerializeField] private string idleAnimName = "Idle"; // Quieto
    [SerializeField] private string walkAnimName = "Walk"; // Caminar
    [SerializeField] private string attackAnimName = "Attack"; // Atacar
    [SerializeField] private string deathAnimName = "Death"; // Muerte
    
    // Parámetros del Animator, usando hashes para optimizar
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int deathHash = Animator.StringToHash("Death");

    private bool isInitialized; // Indica si el agente ha sido inicializado correctamente

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Establece la velocidad de movimiento para la animación
    public void SetMovementSpeed(float speed)
    {
        if (animator != null)
            animator.SetFloat(speedHash, speed);
    }
    
    // Reproduce la animación de ataque
    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger(attackHash);
    }
    
    /// Reproduce la animación de muerte
    public void PlayDeath()
    {
        if (animator != null)
            animator.SetTrigger(deathHash);
    }
    
    /// Verifica si está reproduciendo la animación de ataque
    public bool IsAttacking()
    {
        if (animator == null) return false;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(attackAnimName);
    }
}
