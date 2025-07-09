using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;

    [Header("Grid‑Snap")]
    [SerializeField] bool useGrid = true;
    [SerializeField] float cellSize = 1f;
    [SerializeField] float snapEps = 0.01f;

    enum Axis { None, Horizontal, Vertical }

    PlayerControls controls;
    Vector2 moveInput;
    Vector2 moveInputFiltered;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    Vector2 lastDir = Vector2.down;
    bool interactThisFrame;
    bool pauseThisFrame;
    public bool InteractPressed { get; private set; }
    public bool EscapePressed { get; private set; }

    Vector3 targetPos;
    bool busy;
    Axis lockAxis = Axis.None;

    bool frozen;
    public void PlaySleepPose(bool on) { anim.SetBool("Sleep", on); }


    public void SetFrozen(bool v)
    {
        frozen = v;
        moveInput = Vector2.zero;
        moveInputFiltered = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        busy = false;
        anim.SetBool("isMoving", false);
    }

    void Awake()
    {
        controls = new PlayerControls();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Interect.performed += _ => interactThisFrame = true;
        controls.Player.Menu.performed += _ => pauseThisFrame = true;

        targetPos = transform.position;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (frozen) { InteractPressed = false; return; }

        InteractPressed = interactThisFrame;
        interactThisFrame = false;
        EscapePressed = pauseThisFrame;
        pauseThisFrame = false;

        Vector2 raw = moveInput;

        if (lockAxis == Axis.None)
        {
            if (raw != Vector2.zero)
                lockAxis = Mathf.Abs(raw.x) >= Mathf.Abs(raw.y) ? Axis.Horizontal : Axis.Vertical;
        }

        if (lockAxis == Axis.Horizontal)
        {
            if (raw.x == 0) lockAxis = Axis.None;
            raw.y = 0;
        }
        else if (lockAxis == Axis.Vertical)
        {
            if (raw.y == 0) lockAxis = Axis.None;
            raw.x = 0;
        }

        moveInputFiltered = raw;

        if (useGrid && !busy && raw != Vector2.zero)
        {
            busy = true;
            targetPos = transform.position + (Vector3)(raw.normalized * cellSize);
        }

        bool moving = useGrid ? busy : raw.sqrMagnitude > 0.01f;
        anim.SetBool("isMoving", moving);

        if (moving)
        {
            Vector2 animDir = useGrid ? (targetPos - transform.position).normalized : raw.normalized;

            anim.SetFloat("moveX", animDir.x);
            anim.SetFloat("moveY", animDir.y);
            lastDir = animDir;

            if (Mathf.Abs(animDir.x) > Mathf.Abs(animDir.y))
                sr.flipX = animDir.x < 0;
        }
        else
        {
            anim.SetFloat("moveX", lastDir.x);
            anim.SetFloat("moveY", lastDir.y);
        }
    }

    void FixedUpdate()
    {
        if (frozen) { rb.linearVelocity = Vector2.zero; return; }

        if (useGrid)
        {
            if (busy)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.fixedDeltaTime);

                if (Vector3.Distance(transform.position, targetPos) < snapEps)
                    busy = false;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            rb.linearVelocity = moveInputFiltered * moveSpeed;
        }
    }
}
