using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace myspace
{
    [System.Serializable]
    public class DeviceList
    {
        public Device[] devices;
    }

    [System.Serializable]
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

    public class ConnectionManager : MonoBehaviour
    {
        public string devicesFilePath;      // ����̽� ������ ����� JSON ���� ���
        static public List<Device> devices = new List<Device>();  // ����̽� ������ ������ ����Ʈ

        private Device selectedDevice;      // ���� ���õ� ����̽��� �����ϴ� ����

        void Start()
        {
            LoadDevices();  // JSON ���Ͽ��� ����̽� ������ �о�� ����Ʈ�� �߰�
        }

        private void LoadDevices()
        {
            if (File.Exists(devicesFilePath))
            {
                string json = File.ReadAllText(devicesFilePath);
                DeviceList deviceList = JsonUtility.FromJson<DeviceList>(json);
                devices = new List<Device>(deviceList.devices);
                Debug.Log("Devices loaded: " + devices.Count);
            }
            else
            {
                Debug.LogError("Devices file not found!");
            }
        }
    }
}
