using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] GameObject _ball;
    [SerializeField] GameObject _target;
    [SerializeField] Text _ballCounter;

    LayerMask _clickMask;
    bool _readyForLaunch;

    void Start()
    {
        _clickMask = LayerMask.GetMask("Ground");
    }

    private void Update() 
    {
        _ballCounter.text = (25 - gameObject.transform.childCount).ToString(); //Update ball count on UI
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, _clickMask))
        {
            _target.transform.position = hit.point + Vector3.up * 0.01f;
            if (Input.GetMouseButtonDown(0) && gameObject.transform.childCount < 24)
            {
                if(!_readyForLaunch)
                {
                    var ball = Instantiate(_ball, hit.point, Quaternion.identity);
                    ball.transform.parent = gameObject.transform;
                }
                _readyForLaunch = !_readyForLaunch;
            }
        }
    }
}
