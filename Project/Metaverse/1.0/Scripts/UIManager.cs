using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using myspace;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;

namespace myspace
{
    // UI ���� Ŭ����
    public class UIManager : MonoBehaviour
    {

        private List<Device> devices;          // ����̽� ������ ������ ����Ʈ

        public GameObject uiPanel;          // UI �г�
        public GameObject uiPanel2;         // UI �г�2
        public Button deviceButtonPrefab;   // ����̽� ��ư ������ ���� ������Ʈ�� �����ϴ� ����

        public Button lightButton;
        public Button exitButton;
        public Button temButton;
        public Button disButton;

        public bool isLightOn = false; // ������ �����ִ��� ����

        DeviceConnectionManager deviceConnectionManager;

        private Device selectedDevice;      // ���� ���õ� ����̽��� �����ϴ� ����

        void Start()
        {
            deviceConnectionManager = FindObjectOfType<DeviceConnectionManager>();
            devices = ConnectionManager.devices;
            selectedDevice = deviceConnectionManager.selectedDevice;
           
            CreateDeviceButtons();

            // ���� ��ư�� Ŭ�� ������ �߰�
            lightButton.onClick.AddListener(OnLightButtonClick);
            // exit ��ư�� Ŭ�� ������ �߰�
            exitButton.onClick.AddListener(OnExitButtonClick);
            // �µ� ��ư�� Ŭ�� ������ �߰�
            temButton.onClick.AddListener(OnTemperatureButtonClick);
            // �Ÿ� ��ư�� Ŭ�� ������ �߰�
            disButton.onClick.AddListener(OnDistanceButtonClick);

            uiPanel.SetActive(false);
            uiPanel2.SetActive(false);
        }

        private void CreateDeviceButtons()
        {
            foreach (Device device in devices)
            {
                Button button = Instantiate(deviceButtonPrefab, uiPanel.transform);
                button.GetComponentInChildren<Text>().text = "Device " + (devices.IndexOf(device) + 1).ToString();
                button.onClick.AddListener(() => DeviceButtonClicked(device));
            }
        }

        // ����̽� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
        private void DeviceButtonClicked(Device device)
        {
            uiPanel.SetActive(false);
            uiPanel2.SetActive(true);

            // ���õ� ����̽� ����
            selectedDevice = device;

            // ���õ� ����̽��� ����
            deviceConnectionManager.ConnectToDevice(selectedDevice);
            
        }

        // ���� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
        private void OnLightButtonClick()
        {
            if (!isLightOn)
            {
                string message = "LightOn";
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                selectedDevice.socket.Send(messageBytes);

                // ��ư �ؽ�Ʈ ����
                isLightOn = true;
                lightButton.GetComponentInChildren<Text>().text = isLightOn ? "LightOff" : "LightOn";

                Debug.Log("Light " + (isLightOn ? "On" : "Off"));
            }
            else
            {
                string message = "LightOff";
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                selectedDevice.socket.Send(messageBytes);

                // ��ư �ؽ�Ʈ ����
                isLightOn = false;
                lightButton.GetComponentInChildren<Text>().text = isLightOn ? "LightOff" : "LightOn";

                Debug.Log("Light " + (isLightOn ? "On" : "Off"));
            }
        }

        // �µ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
        private void OnTemperatureButtonClick()
        {
            if (selectedDevice != null)
            {
                string message = "Temperature";
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                

                try
                {
                    selectedDevice.socket.Send(messageBytes);
                    Debug.Log("Temperature On");
                    message = deviceConnectionManager.receivedData;
                    StartCoroutine(UpdateButtonText(temButton, message, "Temperature", 3.0f));
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to send data to the device: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("No device selected!");
            }
        }

        // �Ÿ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
        private void OnDistanceButtonClick()
        {
            if (selectedDevice != null)
            {
                string message = "Distance";
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                try
                {
                    selectedDevice.socket.Send(messageBytes);
                    Debug.Log("Distance On");
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to send data to the device: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("No device selected!");
            }
        }

        // exit ��ư Ŭ�� �� ȣ��Ǵ� �޼���
        private void OnExitButtonClick()
        {
            deviceConnectionManager.DisconnectFromDevice(selectedDevice);

            uiPanel2.SetActive(false);
            uiPanel.SetActive(true);
        }

        // ��ư �ؽ�Ʈ�� �ӽ÷� �����ϴ� �ڷ�ƾ
        private IEnumerator UpdateButtonText(Button clickbutton, string newText, string originalText, float duration)
        {
            // ��ư �ؽ�Ʈ ����
            Button button = clickbutton; // �ش� ��ư�� �°� ����
            Text buttonText = button.GetComponentInChildren<Text>();
            string originalButtonText = buttonText.text;
            buttonText.text = newText;

            yield return new WaitForSeconds(duration);

            buttonText.text = originalText;

        }
    }
}
