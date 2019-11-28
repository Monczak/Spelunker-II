using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed, acceleration, decelerationCoeff, responsiveness;
    public float groundCheckDistance = .1f;
    public ControlStyle controlStyle;

    public float mineReach;
    public float interactionReach;
    public float maxHitDeltaMagnitude;

    public GameObject digboxPrefab;
    public DigboxController digboxController;

    private PlayerControls controls;

    private Vector2 input;
    private bool interact = false, manualInteract = false;
    private Vector3 velocity, rotatedVelocity;

    private Rigidbody rb;
    private LayerMask groundMask;

    private Collider col;
    public Collider listenerCollider;

    Action<InputAction.CallbackContext> MoveAction, MoveCancelAction, ClickAction, ButtonInteractAction;

    Interactable[] nearestInteractables, oldNearestInteractables;

    public delegate void OnInteractableChangeDelegate(Interactable interactable);
    public event OnInteractableChangeDelegate OnInteractableChange;

    private void SetInput(Vector2 val)
    {
        input = val;
    }

    private void SetInteract(bool val)
    {
        interact = val;
    }

    private void SetManualInteract(bool val)
    {
        manualInteract = val;
    }

    private void RegisterInputEvents()
    {
        controls.Movement.Move.performed += MoveAction;
        controls.Movement.Move.canceled += MoveCancelAction;
        controls.Interaction.ClickInteract.performed += ClickAction;
        controls.Interaction.ButtonInteract.performed += ButtonInteractAction;
    }

    public void UnloadInputEvents()
    {
        controls.Movement.Move.performed -= MoveAction;
        controls.Movement.Move.canceled -= MoveCancelAction;
        controls.Interaction.ClickInteract.performed -= ClickAction;
        controls.Interaction.ButtonInteract.performed -= ButtonInteractAction;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        groundMask = LayerMask.GetMask("Ground");

        controls = new PlayerControls();

        MoveAction = delegate (InputAction.CallbackContext ctx)
        {
            SetInput(ctx.ReadValue<Vector2>());
        };

        MoveCancelAction = delegate (InputAction.CallbackContext ctx)
        {
            SetInput(Vector2.zero);
        };

        ClickAction = delegate (InputAction.CallbackContext ctx)
        {
            SetInteract(true);
        };

        ButtonInteractAction = delegate (InputAction.CallbackContext ctx)
        {
            SetManualInteract(true);
        };

        RegisterInputEvents();
        controls.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        digboxController = Instantiate(digboxPrefab, transform.position, Quaternion.Euler(90, 0, 0)).GetComponent<DigboxController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(input);

        RaycastHit hit = GetRaycastHitAtMouse();

        bool isInReach = IsInReach(hit.point);
        SetDigbox(hit, isInReach);

        // TODO: Measure cost of this (probably not the best expression to put in Update)
        nearestInteractables = FindNearestInteractables();
        if (nearestInteractables != null)
        {
            nearestInteractables = nearestInteractables
                .OrderBy(i => -i.priority)
                .ThenBy(i => (i.transform.position - transform.position).sqrMagnitude)
                .ToArray();
        }

        if (nearestInteractables != oldNearestInteractables)
        {
            if (OnInteractableChange != null) OnInteractableChange.Invoke(nearestInteractables?[0]);
        }
        oldNearestInteractables = nearestInteractables;

        // Temporary - will move to an Action or something
        if (interact)
        {
            DoInteraction(hit, isInReach);
            interact = false;
        }
        else if (manualInteract)
        {
            DoManualInteraction();
            manualInteract = false;
        }
    }

    private void OnDestroy()
    {
        OnInteractableChange?.Invoke(null);
        //UnloadInputEvents();
    }

    void Move(Vector2 direction)
    {
        Vector2 ivel = direction * acceleration;
        velocity = new Vector3(ivel.x, 0, ivel.y);
        rotatedVelocity = Quaternion.AngleAxis(-GameManager.Instance.cameraFollow.lookDirection, Vector3.up) * velocity;
    }

    private void FixedUpdate()
    {
        rb.AddForce(rotatedVelocity * (rotatedVelocity - new Vector3(rb.velocity.x, 0, rb.velocity.z)).magnitude * responsiveness * Time.fixedDeltaTime, ForceMode.VelocityChange);

        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), decelerationCoeff);

        bool groundNearby = Physics.Raycast(transform.position - Vector3.up * col.bounds.size.y / 2, Vector3.down, out RaycastHit hit, groundCheckDistance, LayerMask.GetMask("Ground"));

        if (groundNearby)
            rb.MovePosition(new Vector3(rb.position.x, hit.point.y + col.bounds.size.y / 2, rb.position.z));
    }

    RaycastHit GetRaycastHitAtMouse()
    {
        Ray ray = GameManager.Instance.mainCam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        Physics.Raycast(ray, out RaycastHit hit, 100f);
        return hit;
    }

    void SetDigbox(RaycastHit hit, bool inReach)
    {
        Coord coord = GameManager.Instance.mapGenerator.WorldPointToCoord(hit.point);

        digboxController.tileCoord = coord;
        digboxController.isVisible = inReach;
    }

    void DoInteraction(RaycastHit hit, bool isInReach)
    {
        InteractResult interactResult = Interact(hit, isInReach);
        if (interactResult != InteractResult.Success)
        {
            TryMineResult tryMineResult = TryMine(hit, isInReach);
            if (tryMineResult != TryMineResult.Success)
            {
                return;
            }
        }
    }

    void DoManualInteraction()
    {
        if (nearestInteractables == null || nearestInteractables.Length == 0)
            return;

        InteractResult interactResult = PerformInteraction(nearestInteractables[0]);
        if (interactResult != InteractResult.Success)
        {
            // Do something
            Debug.LogError($"Manual interaction with {nearestInteractables[0].name} failed!");
            return;
        }
    }

    InteractResult Interact(RaycastHit hit, bool inReach)
    {
        Interactable interactable = hit.collider.GetComponent<Interactable>();
        if (interactable == null) return InteractResult.NotInteractable;

        return PerformInteraction(interactable);
    }

    InteractResult PerformInteraction(Interactable interactable)
    {
        Debug.Log($"Interacting with {interactable.gameObject.name}");
        try
        {
            interactable.Interact(transform);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Error while interacting with {0}: {1}", interactable.gameObject.name, e.Message));
            return InteractResult.Error;
        }
        return InteractResult.Success;
    }

    Interactable[] FindNearestInteractables()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionReach, LayerMask.GetMask("Interactable"));

        if (colliders.Length == 0) return null;

        Interactable[] result = new Interactable[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
            if (!colliders[i].CompareTag("Auxiliary Collider"))
                result[i] = colliders[i].GetComponent<Interactable>();
        return result;
    }

    bool IsInReach(Vector3 pos)
    {
        Vector3 rayDirection = new Vector3(pos.x - transform.position.x, 0, pos.z - transform.position.z);

        if (rayDirection.magnitude > mineReach) return false;

        Coord tileCoord = GameManager.Instance.mapGenerator.WorldPointToCoord(pos);
        if (GameManager.Instance.mapGenerator.GetTileAt(new Coord(tileCoord.tileX - 1, tileCoord.tileY)).type == LevelTileType.Wall &&
            GameManager.Instance.mapGenerator.GetTileAt(new Coord(tileCoord.tileX, tileCoord.tileY - 1)).type == LevelTileType.Wall &&
            GameManager.Instance.mapGenerator.GetTileAt(new Coord(tileCoord.tileX, tileCoord.tileY + 1)).type == LevelTileType.Wall &&
            GameManager.Instance.mapGenerator.GetTileAt(new Coord(tileCoord.tileX + 1, tileCoord.tileY)).type == LevelTileType.Wall)
            return false;

        if (GameManager.Instance.mapGenerator.GetTileAt(tileCoord).isBorder) return false;

        /*
        Debug.Log(string.Format("Picked: {0} {1}", tileCoord.tileX, tileCoord.tileY));

        Ray checkRay = new Ray(transform.position, rayDirection);
        if (!Physics.Raycast(checkRay, out RaycastHit checkHit, rayDirection.magnitude, LayerMask.GetMask("Mineable"))) return false;

        // TODO: Make this work
        Coord checkCoord = GameManager.Instance.mapGenerator.WorldPointToCoord(checkHit.point);
        Debug.Log(string.Format("Hit: {0} {1}", checkCoord.tileX, checkCoord.tileY));
        if (checkCoord != tileCoord) return false;

        Debug.Log("In reach");
        */

        return true;

        /* Experimental code
        Ray checkRay = new Ray(transform.position, rayDirection);
        Physics.Raycast(checkRay, out RaycastHit checkHit, rayDirection.magnitude, LayerMask.GetMask("Mineable"));

        Vector3 posDelta = new Vector3(pos.x - checkHit.point.x, 0, pos.z - checkHit.point.z);
        if (posDelta.magnitude > maxHitDeltaMagnitude || (checkHit.point - transform.position).magnitude < (new Vector3(pos.x, transform.position.y, pos.z) + posDelta - checkHit.point).magnitude)
            return false;

        return true;
        */
    }

    TryMineResult TryMine(RaycastHit hit, bool inReach)
    {
        Vector3 pos = hit.point;

        if (!inReach)
            return TryMineResult.OutOfReach;

        LevelTile tile = GameManager.Instance.mapGenerator.GetTileAt(pos);

        if (tile.type == LevelTileType.Nothing) return TryMineResult.NotHit;
        if (tile.isBorder) return TryMineResult.NotMineable;

        try
        {
            tile.Mine();
        }
        catch
        {
            return TryMineResult.NotMineable;
        }
        GameManager.Instance.meshGenerator._updatePos1 = pos;
        GameManager.Instance.meshGenerator._updatePos2 = pos;

        GameManager.Instance.mapGenerator.UpdateMap(regen: true);
        return TryMineResult.Success;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}

public enum ControlStyle
{
    Overhead,
    Tank
}

public enum TryMineResult
{
    Success,
    NotMineable,
    NotHit,
    OutOfReach
}

public enum InteractResult
{
    Success,
    NotInteractable,
    Error
}
