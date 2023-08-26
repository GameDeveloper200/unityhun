using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Nyilt kodok (amiket meg kell adni értékeket)
    public float walkSpeed = 7f;
    public float runSpeed = 12f;
    public float crouchSpeed = 2f;
    public float crouchHeight = 0.5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 12f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Camera playerCamera;
    public GameObject menuPanel;

    // privát változok (hagyd így ha lehet)
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrouching;
    private CharacterController characterController;
    private bool isMenuOpen = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Ellenõrzés, hogy a menü nyitva van-e
        bool isMenuOpen = menuPanel.activeSelf;

        // Csak akkor hajtódik végre amikor nincs meg nyitva a menu
        if (!isMenuOpen)
        {
            // Futtatás ellenõrzése
            isRunning = Input.GetKey(KeyCode.LeftShift) && isGrounded;

            // Sebesség beállítása a futás vagy séta alapján
            float moveSpeed = isRunning ? runSpeed : walkSpeed;

            // Mozgás irányok
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
            moveDirection *= moveSpeed;

            // Elenõrzi hogy a földön vagy
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Földön az y sebességet nullázzuk vagy kis értékre állítjuk
            }

            // Mozgatás és gravitáció
            characterController.Move(moveDirection * Time.deltaTime);
            characterController.Move(Vector3.down * Time.deltaTime * 9.81f);

            // Ugrás
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            // Forgás
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            // Gravitáció
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            // Guggolás ellenõrzése
            if (isGrounded && Input.GetKeyDown(KeyCode.C))
            {
                ToggleCrouch();
            }
        }

        // Menü megnyitása és bezárása
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    // Elenörzi hogy érinted e a földet
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isGrounded = hit.normal.y > 0.7f;
    }


    // Ha le nyomtad a gugolás gombot
    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        characterController.height = isCrouching ? crouchHeight : 2f;
    }

    // Ha meg nyomtad az ESC gombot
    public void ToggleMenu()
    {
        // Elenörzi hogy aktivnak vagy inaktivnak kell lenie a menunek
        isMenuOpen = !isMenuOpen;
        menuPanel.SetActive(isMenuOpen);

        // Menu Cursor
        Cursor.lockState = isMenuOpen ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = isMenuOpen;
    }
}