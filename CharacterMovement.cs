using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Nyilt kodok (amiket meg kell adni �rt�keket)
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

    // priv�t v�ltozok (hagyd �gy ha lehet)
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
        // Ellen�rz�s, hogy a men� nyitva van-e
        bool isMenuOpen = menuPanel.activeSelf;

        // Csak akkor hajt�dik v�gre amikor nincs meg nyitva a menu
        if (!isMenuOpen)
        {
            // Futtat�s ellen�rz�se
            isRunning = Input.GetKey(KeyCode.LeftShift) && isGrounded;

            // Sebess�g be�ll�t�sa a fut�s vagy s�ta alapj�n
            float moveSpeed = isRunning ? runSpeed : walkSpeed;

            // Mozg�s ir�nyok
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
            moveDirection *= moveSpeed;

            // Elen�rzi hogy a f�ld�n vagy
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // F�ld�n az y sebess�get null�zzuk vagy kis �rt�kre �ll�tjuk
            }

            // Mozgat�s �s gravit�ci�
            characterController.Move(moveDirection * Time.deltaTime);
            characterController.Move(Vector3.down * Time.deltaTime * 9.81f);

            // Ugr�s
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            // Forg�s
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            // Gravit�ci�
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            // Guggol�s ellen�rz�se
            if (isGrounded && Input.GetKeyDown(KeyCode.C))
            {
                ToggleCrouch();
            }
        }

        // Men� megnyit�sa �s bez�r�sa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    // Elen�rzi hogy �rinted e a f�ldet
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isGrounded = hit.normal.y > 0.7f;
    }


    // Ha le nyomtad a gugol�s gombot
    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        characterController.height = isCrouching ? crouchHeight : 2f;
    }

    // Ha meg nyomtad az ESC gombot
    public void ToggleMenu()
    {
        // Elen�rzi hogy aktivnak vagy inaktivnak kell lenie a menunek
        isMenuOpen = !isMenuOpen;
        menuPanel.SetActive(isMenuOpen);

        // Menu Cursor
        Cursor.lockState = isMenuOpen ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = isMenuOpen;
    }
}