using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMotion : MonoBehaviour
{
    [SerializeField] [Range(10, 50)] int _lineSegment = 10; //Number of vertices for the arc visualization

    [SerializeField] float _minimumJump = 0.05f; //The min jump height of the ball in order to keep bouncing

    LineRenderer _lineRenderer;
    LayerMask _clickMask; //The layer in which the Plane resides

    JumpSlider _jumpSlider;
    SpeedSlider _speedSlider;
    BounceSlider _bounceSlider;
    float _jumpMultiplier;
    float _speedMultiplier;
    float _bounceMultiplier;

    Vector3 _startPosition;
    Vector3 _targetPosition;

    Vector3[] _checkpoints;
    int _checkpointIndex = 1;

    bool _movementStarted;

    void Start()
    {
        _clickMask = LayerMask.GetMask("Ground");
        _lineRenderer = GetComponent<LineRenderer>();

        _jumpSlider = FindObjectOfType<JumpSlider>();
        _speedSlider = FindObjectOfType<SpeedSlider>();
        _bounceSlider = FindObjectOfType<BounceSlider>();

        _startPosition = transform.position;
    }
    
    void Update() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //If the ball is ready to launch and the cursor is on the Plane, we can try to calculate the path
        if (Physics.Raycast(ray, out hit, 1000f, _clickMask) && !_movementStarted)
        {
            _targetPosition = hit.point;

            if (Input.GetMouseButtonDown(0)) //Start the movement after calculating the path points and disable the visualization
            {
                _checkpoints = new Vector3[_lineRenderer.positionCount];
                _lineRenderer.GetPositions(_checkpoints);
                _lineRenderer.enabled = false;
                _movementStarted = true;

                _speedMultiplier = _speedSlider.speedValue; //We need to check these values only when the ball is launched
                _bounceMultiplier = _bounceSlider.bounceValue; //We need to check these values only when the ball is launched
            }

            _jumpMultiplier = _jumpSlider.jumpValue; //We need to check this value every frame so that the arc is drawn correctly
            
            Vector3 vo = CalculateVelocity(_targetPosition, _startPosition, 1f); //Determine the Velocity of the ball
            Visualize(vo); //Draw the predicted path according to the jump multiplier and the Velocity
        }

        if (_movementStarted) MoveBall();
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time) //Determines the velocity of the ball
    {
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;
        
        float Vxz = Sxz * time;
        float Vy = (Sy / time) +(0.5f * Mathf.Abs(Physics.gravity.y) * time);

        Vector3 result = distance.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    Vector3 CalculatePosition(Vector3 vo, float time) //Determines the position of the ball at any given point in time
    {
        Vector3 Vxz = vo;
        Vxz.y = 0f;

        Vector3 result = _startPosition + vo * time;
        float sY = ((-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + _startPosition.y);

        result.y = sY * _jumpMultiplier;
        
        return result;
    }

    void Visualize(Vector3 vo) //Updates the linerenderer according to the calculated positions
    {
        _lineRenderer.positionCount = _lineSegment + 1;
        
        for (int i = 0; i < _lineSegment; i++)
        {
            Vector3 pos = CalculatePosition(vo, i / (float)_lineSegment);
            _lineRenderer.SetPosition(i, pos); 
        }

        _lineRenderer.SetPosition(_lineSegment, _targetPosition);
    }

    void MoveBall()
    {
        if (_checkpointIndex < _checkpoints.Length) //If we haven't reached the ground yet
        {
            transform.position = Vector3.MoveTowards(transform.position, _checkpoints[_checkpointIndex], _speedMultiplier * Time.deltaTime * 5);

            if (transform.position == _checkpoints[_checkpointIndex]) //Start moving to the next path point
            {
                _checkpointIndex++;
            }
        }
        else //When we reach the ground, we need to calculate the new path points according to the bounciness
        {
            for (int i = 0; i < _checkpoints.Length; i++) //Select every path point
            {
                _checkpoints[i] += (_targetPosition - _startPosition); //Carry the path points to the current spot
                _checkpoints[i] -= ((_checkpoints[i] - transform.position) * (1 - (_bounceMultiplier / 10))); //Subtract the bounce loss
            }
            _targetPosition = _checkpoints[(_checkpoints.Length - 1)]; //Update the target point for the next time we are going to carry the path points
            _startPosition = transform.position; //Update the start point for the next time we are going to carry the path points


            _jumpMultiplier *= (_bounceMultiplier / 10);

            if (_jumpMultiplier > _minimumJump) _checkpointIndex = 1; //Check if the next bounce is going to be high enough
            else Destroy(gameObject); //Otherwise destroy the gameobject
        }
    }
}
