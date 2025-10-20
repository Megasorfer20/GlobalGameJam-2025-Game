using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerInputController : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }
    public PlayerType playerType;

    private CharacterMovement characterMovement;
    private PlayerControls playerControls;
    private InputActionMap currentActionMap;

    private Vector2 currentMoveInput;

    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        if (characterMovement == null)
        {
            Debug.LogError("ERROR: CharacterMovement no encontrado en " + gameObject.name + ". Asegúrate de que el script CharacterMovement esté adjunto.", this);
            enabled = false; // Desactiva este script si no encuentra CharacterMovement
            return;
        }

        playerControls = new PlayerControls();
        if (playerControls == null)
        {
            Debug.LogError("ERROR: PlayerControls es nulo después de instanciarse para " + gameObject.name, this);
            enabled = false;
            return;
        }
        else
        {
            Debug.Log("PlayerControls instanciado correctamente para " + gameObject.name, this);
        }

        if (playerType == PlayerType.Player1)
        {
            Debug.Log($"Intentando asignar Action Map Player1 para {gameObject.name}");
            currentActionMap = playerControls.Player1.Get(); // Usamos .Get() para asegurarnos de obtener la instancia de InputActionMap
            if (currentActionMap == null) Debug.LogError("ERROR: playerControls.Player1.Get() devolvió null para " + gameObject.name, this);
            else Debug.Log($"Action Map Player1 ({currentActionMap.name}) asignado correctamente para {gameObject.name}", this);
        }
        else // PlayerType.Player2
        {
            Debug.Log($"Intentando asignar Action Map Player2 para {gameObject.name}");
            currentActionMap = playerControls.Player2.Get(); // Usamos .Get() para asegurarnos de obtener la instancia de InputActionMap
            if (currentActionMap == null) Debug.LogError("ERROR: playerControls.Player2.Get() devolvió null para " + gameObject.name, this);
            else Debug.Log($"Action Map Player2 ({currentActionMap.name}) asignado correctamente para {gameObject.name}", this);
        }

        if (currentActionMap == null)
        {
            Debug.LogError("ERROR: currentActionMap es nulo ANTES de suscribir acciones para " + gameObject.name + ". El script se desactivará.", this);
            enabled = false; // Desactiva este script si currentActionMap sigue siendo null
            return; // Sal del Awake para evitar más errores
        }

        // Suscribimos la acción de movimiento
        InputAction moveAction = currentActionMap.FindAction("Move");
        if (moveAction == null)
        {
            Debug.LogError($"ERROR: La acción 'Move' no se encontró en el Action Map '{currentActionMap.name}' para {gameObject.name}. Revisa tu asset PlayerControls.inputactions.", this);
            enabled = false;
            return;
        }
        moveAction.performed += ctx => currentMoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => currentMoveInput = Vector2.zero;

        // Suscribimos la acción de Dash
        InputAction dashAction = currentActionMap.FindAction("Dash");
        if (dashAction == null)
        {
            Debug.LogError($"ERROR: La acción 'Dash' no se encontró en el Action Map '{currentActionMap.name}' para {gameObject.name}. Revisa tu asset PlayerControls.inputactions.", this);
            enabled = false;
            return;
        }
        dashAction.performed += ctx => characterMovement.Dash();

        // Suscribimos las acciones adicionales (V/B o K/L)
        InputAction obtainAction = currentActionMap.FindAction("Obtain");
        if (obtainAction == null) Debug.LogWarning($"Advertencia: La acción 'Obtain' no se encontró en el Action Map '{currentActionMap.name}' para {gameObject.name}.");
        else obtainAction.performed += ctx => Debug.Log($"{gameObject.name}: Action 1 (Obtain) pressed!");

        InputAction useAction = currentActionMap.FindAction("Use");
        if (useAction == null) Debug.LogWarning($"Advertencia: La acción 'Use' no se encontró en el Action Map '{currentActionMap.name}' para {gameObject.name}.");
        else useAction.performed += ctx => Debug.Log($"{gameObject.name}: Action 2 (Use) pressed!");

        Debug.Log($"InputController para {gameObject.name} inicializado correctamente con Action Map {currentActionMap.name}", this);
    }

    void OnEnable()
    {
        if (currentActionMap != null)
        {
            currentActionMap.Enable();
            Debug.Log($"Action Map {currentActionMap.name} habilitado para {gameObject.name}", this);
        }
    }

    void OnDisable()
    {
        if (currentActionMap != null)
        {
            currentActionMap.Disable();
            Debug.Log($"Action Map {currentActionMap.name} deshabilitado para {gameObject.name}", this);
        }
    }

    void Update()
    {
        characterMovement.SetMoveInput(currentMoveInput);
    }
}