using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using System.Text;
using UnityEditor;

// Device Ŭ����: ����̽� ������ ��� Ŭ����
public class Device
{
    public string ipAddress;  // ����̽� IP �ּ�
    public int port;  // ����̽� ��Ʈ ��ȣ
    public Socket socket;

    public Device(string ipAddress, int port)
    {
        this.ipAddress = ipAddress;
        this.port = port;
    }
}

public class ConnectionController : MonoBehaviour
{
    public string devicesFilePath;  // ����̽� ������ ����� JSON ���� ���
    private List<Device> devices = new List<Device>();  // ����̽� ������ ������ ����Ʈ

    public GameObject uiPanel;  // UI �г�
    public GameObject uiPanel2;  // UI �г�2

    public Button deviceButtonPrefab; // ����̽� ��ư ������ ���� ������Ʈ�� �����ϴ� ����
    //public GameObject devicePanel; // ����̽� ��ư�� ������ �г� ���� ������Ʈ�� �����ϴ� ����

    public Text deviceNameText; // ���õ� ����̽��� �̸��� ǥ���ϴ� UI �ؽ�Ʈ�� �����ϴ� ����
    public Text deviceIPText; // ���õ� ����̽��� IP �ּҸ� ǥ���ϴ� UI �ؽ�Ʈ�� �����ϴ� ����
    public Text deviceStatusText; // ���õ� ����̽��� ���� ���¸� ǥ���ϴ� UI �ؽ�Ʈ�� �����ϴ� ����

    private Device selectedDevice; // ���� ���õ� ����̽��� �����ϴ� ����

    public Button lightButton;
    public Button temperatureButton;
    public Button ExitButton;

    public bool isLightOn = false; // ������ �����ִ��� ����

    void Start()
    {
        LoadDevices();  // JSON ���Ͽ��� ����̽� ������ �о�� ����Ʈ�� �߰�

        // ���� ��ư�� Ŭ�� ������ �߰�
        lightButton.onClick.AddListener(OnLightButtonClick);
        // �µ� ��ư�� Ŭ�� ������ �߰�
        temperatureButton.onClick.AddListener(OnTemperatureButtonClick);
        // ���� ��ư�� Ŭ�� ������ �߰�
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Switch"))
        {
            // ��� ����̽��� ����
            foreach (Device device in devices)
            {
                ConnectToDevice(device);
            }

            uiPanel.SetActive(true);  // UI �г� Ȱ��ȭ
            Cursor.visible = true;  // ���콺 Ŀ���� ���̰� ��
            Cursor.lockState = CursorLockMode.None;  // ���콺 Ŀ���� �������� �ʵ��� ��
        }
    }



    // JSON ���Ͽ��� ����̽� ������ �о�� ����Ʈ�� �߰�
    private void LoadDevices()
    {
        if (File.Exists(devicesFilePath))
        {
            string json = File.ReadAllText(devicesFilePath);
            devices = JsonUtility.FromJson<List<Device>>(json);
            CreateDeviceButtons();
        }
        else
        {
            Debug.LogError("Devices file not found!");
        }
    }

    // ����Ʈ�� �߰��� ����̽� ������ �������� ����̽� ��ư�� ����
    private void CreateDeviceButtons()
    {
        foreach (Device device in devices)
        {
            Button button = Instantiate(deviceButtonPrefab, uiPanel.transform);
            button.GetComponentInChildren<Text>().text = "Device " + (devices.IndexOf(device) + 1).ToString();
            button.onClick.AddListener(delegate { DeviceButtonClicked(device); });

            selectedDevice = device;
            deviceNameText.text = "Device " + (devices.IndexOf(device) + 1).ToString();
            deviceIPText.text = "IP Address: " + device.ipAddress;
            if (device.socket != null)
            {
                deviceStatusText.text = "Status: Connected";
            }
            else
            {
                deviceStatusText.text = "Status: Not Connected";
            }
        }
    }

    // ����̽� ��ư Ŭ�� �� Panel2 Ȱ��ȭ
    private void DeviceButtonClicked(Device device)
    {
        uiPanel.SetActive(false);

        uiPanel2.SetActive(true); // ����̽��� �����ϴ� ��ư�� �ִ� UI �г� Ȱ��ȭ

    }

    private void OnLightButtonClick()
    {
        if (!isLightOn)
        {
            string message = "LightOn";
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            //_clientSocket.SendTo(messageBytes, _remoteEndPoint);

            // ��ư �ؽ�Ʈ ����
            isLightOn = true;
            lightButton.GetComponentInChildren<Text>().text = isLightOn ? "LightOff" : "LightOn";

            Debug.Log("Light " + (isLightOn ? "On" : "Off"));
        }
        else
        {
            string message = "LightOff";
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            //_clientSocket.SendTo(messageBytes, _remoteEndPoint);

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
        //_clientSocket.SendTo(messageBytes, _remoteEndPoint);
        Debug.Log("Temperature On");
    }

    private void OnExitButtonClick()
    {
        uiPanel.SetActive(false);
        Cursor.visible = false; // ���콺 Ŀ���� ����
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ���� ������
        //moveScript.enabled = true; // Move3D ��ũ��Ʈ�� Ȱ��ȭ��

        // ��������� ���� ���� ����
        Debug.Log("Connection Close");
        //_clientSocket.Close();
    }

    // ����̽��� �����ϴ� �Լ�
    private void ConnectToDevice(Device device)
    {
        // ���� ���� �� ����
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse(device.ipAddress);
        IPEndPoint remoteEP = new IPEndPoint(ip, device.port);
        socket.Connect(remoteEP);
        device.socket = socket;

        // ������ �����ϸ� �ش� ����̽����� ����� ����
        if (socket.Connected)
        {
            Debug.Log("Connected to device at " + device.ipAddress + ":" + device.port);
            // send to message for divice
            string message = "Hello from Unity";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
            socket.Send(msg);
        }
        else
        {
            Debug.LogWarning("Failed to connect to device at " + device.ipAddress + ":" + device.port);
        }
    }

    // ����ġ���� ������ ������ �� ��� ����̽����� ������ ����
    private void OnDestroy()
    {
        // ��� ����̽��� ���� ����
        foreach (Device device in devices)
        {
            DisconnectFromDevice(device.ipAddress, device.port);
        }
    }

    // �ش� ����̽����� ���� ����
    private void DisconnectFromDevice(string ipAddress, int port)
    {
        // ����� ���� ��������
        Socket socket = GetConnectedSocket(ipAddress, port);

        if (socket != null)
        {
            // ���� ����
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Debug.Log("Disconnected from device at " + ipAddress + ":" + port);
        }
        else
        {
            Debug.LogWarning("No connection found for device at " + ipAddress + ":" + port);
        }
    }

    // �־��� IP �ּҿ� ��Ʈ ��ȣ�� �ش��ϴ� ���� ��ü�� ��ȯ
    private Socket GetConnectedSocket(string ipAddress, int port)
    {
        // ���� ����
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // ���� �õ�
        try
        {
            socket.Connect(ipAddress, port);
            Debug.Log("Connected to device at " + ipAddress + ":" + port);
        }
        catch (SocketException e)
        {
            Debug.LogError("Failed to connect to device at " + ipAddress + ":" + port + ": " + e.Message);
            return null;
        }

        return socket;
    }

}
