


using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class BotAIController : MonoBehaviour
{
    private CharacterMovement characterMovement;

    [Header("Configuraci�n de IA")]
    public Transform target; // El objetivo al que el bot debe moverse
    public float proximityThreshold = 0.5f; // Distancia para considerar que el bot ha llegado al objetivo

    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        if (target != null)
        {
            // Calcula la direcci�n desde el bot hasta el objetivo
            // Este es un Vector3, pero CharacterMovement.SetMoveInput espera un Vector2.
            // Para 2.5D, solo nos interesan los componentes X y Z del objetivo.
            Vector2 directionToTarget = new Vector2(target.position.x - transform.position.x, target.position.z - transform.position.z);

            // Si el bot est� lo suficientemente lejos del objetivo, mu�vete
            if (directionToTarget.magnitude > proximityThreshold)
            {
                characterMovement.SetMoveInput(directionToTarget);
            }
            else
            {
                // El bot ha llegado al objetivo, detente o busca un nuevo objetivo
                characterMovement.SetMoveInput(Vector2.zero);
                // Aqu� podr�as a�adir l�gica para:
                // - target = GetNextWaypoint();
                // - Realizar una acci�n;
                // - Esperar un tiempo;
            }
        }
        else
        {
            // Si no hay objetivo, el bot se detiene
            characterMovement.SetMoveInput(Vector2.zero);
        }
    }
}