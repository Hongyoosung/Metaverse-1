using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject uiPanel;
    public Move3D moveScript;

    void Start()
    {
        moveScript = GetComponent<Move3D>(); // Move3D ��ũ��Ʈ�� ������
        Button exitButton = uiPanel.GetComponentInChildren<Button>();
        exitButton.onClick.AddListener(ExitPanel);
    }

    void ExitPanel()
    {
        
    }
}
