using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpSlider : MonoBehaviour
{
    Slider _jumpSlider;

    private void Start() 
    {
        _jumpSlider = GetComponent<Slider>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown("r") && _jumpSlider.value < 20) _jumpSlider.value ++;

        if (Input.GetKeyDown("f") && _jumpSlider.value > 1) _jumpSlider.value--;
    }
}
