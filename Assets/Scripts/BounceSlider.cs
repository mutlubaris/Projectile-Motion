using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BounceSlider : MonoBehaviour
{
    Slider _bounceSlider;

    public float bounceValue { get { return _bounceSlider.value; } }

    private void Start()
    {
        _bounceSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (Input.GetKeyDown("n") && _bounceSlider.value < 9) _bounceSlider.value++;

        if (Input.GetKeyDown("b") && _bounceSlider.value > 1) _bounceSlider.value--;
    }
}
