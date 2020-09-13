using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour
{
    Slider _speedSlider;

    public float speedValue { get { return _speedSlider.value; } }

    private void Start()
    {
        _speedSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (Input.GetKeyDown("h") && _speedSlider.value < 20) _speedSlider.value++;

        if (Input.GetKeyDown("g") && _speedSlider.value > 1) _speedSlider.value--;
    }
}
