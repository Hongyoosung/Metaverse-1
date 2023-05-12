using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
//using UnityEngine.UIElements;

public class RasEvent : MonoBehaviour
{
    private const int PORT = 8555;
    private const string DEVICE_IP = "192.168.0.206"; // ��������� ����� IP �ּ�

    private Socket _clientSocket;
    private IPEndPoint _remoteEndPoint;
    private Move3D moveScript; // Move3D ��ũ��Ʈ

    public GameObject uiPanel; // UI �г�
    public Button lightButton; // ���� ��ư
    public Button temperatureButton; // �µ� ��ư
    public Button ExitButton;        // ���� ��ư
    public bool isLightOn = false; // ������ �����ִ��� ����

    void Start()
    {
        moveScript = GetComponent<Move3D>(); // Move3D ��ũ��Ʈ�� ������

        uiPanel.SetActive(false); // UI �г� ��Ȱ��ȭ

        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(DEVICE_IP), PORT);

        // ���� ��ư�� Ŭ�� ������ �߰�
        lightButton.onClick.AddListener(OnLightButtonClick);
        // �µ� ��ư�� Ŭ�� ������ �߰�
        temperatureButton.onClick.AddListener(OnTemperatureButtonClick);
        // ���� ��ư�� Ŭ�� ������ �߰�
        ExitButton.onClick.AddListener(OnExitButtonClick);

        // ReceiveData �޼��带 ������� ����
        Thread receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Switch"))
        {
            ConnectToServer();

            moveScript.enabled = false;
            uiPanel.SetActive(true);
            Cursor.visible = true; // ���콺 Ŀ���� ���̰� ��
            Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ���� �������� �ʵ��� ��
        }   
    }


    private void OnLightButtonClick()
    {
        if(!isLightOn)
        {
            string message = "LightOn";
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            _clientSocket.SendTo(messageBytes, _remoteEndPoint);

            // ��ư �ؽ�Ʈ ����
            isLightOn = true;
            lightButton.GetComponentInChildren<Text>().text = isLightOn ? "LightOff" : "LightOn";

            Debug.Log("Light " + (isLightOn ? "On" : "Off"));
        }
        else
        {
            string message = "LightOff";
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            _clientSocket.SendTo(messageBytes, _remoteEndPoint);

            // ��ư �ؽ�Ʈ ����
            isLightOn = false;
            lightButton.GetComponentInChildren<Text>().text = isLightOn ? "LightOff" : "LightOn";

            Debug.Log("Light " + (isLightOn ? "On" : "Off"));
        }
    }

    private void OnTemperatureButtonClick()
    {
        string message = "Temperature";
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        _clientSocket.SendTo(messageBytes, _remoteEndPoint);
        Debug.Log("Temperature On");
    }

    private void OnExitButtonClick()
    {
        uiPanel.SetActive(false);
        Cursor.visible = false; // ���콺 Ŀ���� ����
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ���� ������
        moveScript.enabled = true; // Move3D ��ũ��Ʈ�� Ȱ��ȭ��

        // ��������� ���� ���� ����
        Debug.Log("Connection Close");
        _clientSocket.Close();
    }


    private void ConnectToServer()
    {
        string message = "Connect";
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);

        _clientSocket.SendTo(messageBytes, _remoteEndPoint);

        Debug.Log("Connected to Raspberry Pi device");
    }

    private void ReceiveData()
    {
        // ���� ������ ������ ������ ���
        while (true)
        {
            byte[] receiveBytes = new byte[1024];
            int n = _clientSocket.Receive(receiveBytes);
            string receiveMessage = Encoding.ASCII.GetString(receiveBytes, 0, n);
            Debug.Log("Received: " + receiveMessage);
        }
        
    }
}
