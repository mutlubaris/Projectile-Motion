using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] GameObject _ball;
    [SerializeField] GameObject _target;
    [SerializeField] Text _ballCounter;
    [SerializeField] int _ballsInPool = 24;

    LayerMask _clickMask;
    bool _readyForLaunch;

    void Start()
    {
        _clickMask = LayerMask.GetMask("Ground");
    }

    private void Update() 
    {
        _ballCounter.text = (_ballsInPool - (gameObject.transform.childCount - 1)).ToString(); //Update ball counter on UI
        //The number of balls will increase whenever a ball is destroyed, because the childcount will decrease

        if (EventSystem.current.IsPointerOverGameObject()) return; //Return if the cursor is over the Options panel

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, _clickMask)) //If the cursor is on the Plane
        {
            _target.transform.position = hit.point + Vector3.up * 0.01f; //Update the cursor highlighter's position
            if (Input.GetMouseButtonDown(0) && gameObject.transform.childCount - 1 < _ballsInPool) //If we click and still have balls in the pool
            {
                if(!_readyForLaunch) //Instantiate a ball if there isn't one ready to go
                
                {
                    var ball = Instantiate(_ball, hit.point, Quaternion.identity);
                    ball.transform.parent = gameObject.transform; //We will keep track of the number of balls via the childcount
                }

                //If we already have a ball ready to launch, the click will launch the current ball via the BallMotion script
                //Therefore we only need to switch the bool so that we are ready to Instantiate a new ball

                _readyForLaunch = !_readyForLaunch;  
            }
        }
    }
}
