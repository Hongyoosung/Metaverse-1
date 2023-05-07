using UnityEngine;

public class Move3D : MonoBehaviour
{
    public float speed = 5.0f; // �̵� �ӵ�
    public float sensitivity = 2.0f; // ���콺 ����

    private Rigidbody rb;
    private Camera cam;
    private float moveFB, moveLR;
    private float rotX, rotY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // �̵� �Է� �ޱ�
        moveFB = Input.GetAxis("Vertical") * speed;
        moveLR = Input.GetAxis("Horizontal") * speed;

        // ȸ�� �Է� �ޱ�
        rotX += Input.GetAxis("Mouse X") * sensitivity;
        rotY += Input.GetAxis("Mouse Y") * sensitivity;
        rotY = Mathf.Clamp(rotY, -90f, 90f);

        // ȸ�� ����
        transform.localRotation = Quaternion.Euler(0f, rotX, 0f);
        cam.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);

        // �̵� ����
        Vector3 movement = transform.forward * moveFB + transform.right * moveLR;
        rb.MovePosition(transform.position + movement * Time.deltaTime);
    }
}
