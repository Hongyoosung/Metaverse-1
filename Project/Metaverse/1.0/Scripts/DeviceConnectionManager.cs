using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;

namespace myspace
{
    
    // ����̽����� ���� ���� Ŭ����
    public class DeviceConnectionManager : MonoBehaviour
    {
        public Device selectedDevice;      // ���� ���õ� ����̽��� �����ϴ� ����

        public string receivedData;

        public Text deviceNameText;         // ���õ� ����̽��� �̸��� ǥ���ϴ� UI �ؽ�Ʈ�� �����ϴ� ����
        public Text deviceIPText;           // ���õ� ����̽��� IP �ּҸ� ǥ���ϴ� UI �ؽ�Ʈ�� �����ϴ� ����
        public Text deviceStatusText;       // ���õ� ����̽��� ���� ���¸� ǥ���ϴ� UI �ؽ�Ʈ�� �����ϴ� ����
        public DeviceConnectionManager(Device device)
        {
            selectedDevice = device;
        }

        public void ConnectToDevice(Device selectedDevice)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(selectedDevice.ipAddress);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, selectedDevice.port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                // ���� ����
                Debug.Log("Connected to device: " + selectedDevice.ipAddress);
                // ����̽��κ��� �����͸� ���� ����
                ReceiveData(socket);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to connect to device: " + e.Message);
            }
        }

        private void ReceiveData(Socket socket)
        {
            Thread receiveThread = new Thread(() =>
            {
                try
                {
                    while (socket.Connected)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = socket.Receive(buffer);
                        receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log("Received data: " + receivedData);
                        // ������ �����͸� ó���ϴ� ���� �߰�
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while receiving data: " + e.Message);
                }
            });

            receiveThread.Start();
        }

        public void DisconnectFromDevice(Device device)
        {
            try
            {
                if (device.socket != null && device.socket.Connected)
                {
                    device.socket.Shutdown(SocketShutdown.Both);
                    device.socket.Close();
                }
                device.socket = null;
                deviceStatusText.text = "Status: Not Connected";
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to disconnect from device: " + e.Message);
            }
        }
    }
}
