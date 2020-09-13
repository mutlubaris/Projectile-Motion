using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour
{
    Slider _speedSlider;

    private void Start()
    {
        _speedSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (Input.GetKeyDown("t") && _speedSlider.value < 20) _speedSlider.value++;

        if (Input.GetKeyDown("g") && _speedSlider.value > 1) _speedSlider.value--;
    }
}
