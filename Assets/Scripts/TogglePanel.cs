using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [SerializeField] KeyCode _toggleKey = KeyCode.Tab;
    [SerializeField] GameObject _panel = null;

    void Update()
    {
        if (Input.GetKeyDown(_toggleKey)) _panel.SetActive(!_panel.activeSelf);
    }
}
