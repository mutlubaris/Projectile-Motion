using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpSlider : MonoBehaviour
{
    Slider _jumpSlider;

    public float jumpValue { get { return _jumpSlider.value; } }

    private void Start() 
    {
        _jumpSlider = GetComponent<Slider>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown("y") && _jumpSlider.value < 20) _jumpSlider.value ++;

        if (Input.GetKeyDown("t") && _jumpSlider.value > 1) _jumpSlider.value--;
    }
}
