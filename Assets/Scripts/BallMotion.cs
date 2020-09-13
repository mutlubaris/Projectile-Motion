using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMotion : MonoBehaviour
{
    [SerializeField] [Range(10, 50)] int _lineSegment = 10;
    
    [SerializeField] float _minimumJump = 0.1f;

    LineRenderer _lineRenderer;
    LayerMask _clickMask;
    JumpSlider _jumpSlider;
    SpeedSlider _speedSlider;
    BounceSlider _bounceSlider;

    Vector3 _startPosition;
    Vector3 _targetPosition;
    Vector3 _cursorPosition;
    Vector3[] _checkpoints;

    int _checkpointIndex = 1;
    float _jumpMultiplier;
    float _speedMultiplier;
    float _bounceMultiplier;

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

        if (Physics.Raycast(ray, out hit, 1000f, _clickMask) && !_movementStarted)
        {
            _cursorPosition = hit.point;

            if (Input.GetMouseButtonDown(0))
            {
                _checkpoints = new Vector3[_lineRenderer.positionCount];
                _lineRenderer.GetPositions(_checkpoints);
                _lineRenderer.enabled = false;
                _movementStarted = true;
            }

            _jumpMultiplier = _jumpSlider.jumpValue;
            _speedMultiplier = _speedSlider.speedValue;
            _bounceMultiplier = _bounceSlider.bounceValue; 

            Vector3 vo = CalculateVelocity(_cursorPosition, _startPosition, 1f);
            Visualize(vo);
        }

        if (_movementStarted) MoveBall();
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
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

    Vector3 CalculatePosition(Vector3 vo, float time)
    {
        Vector3 Vxz = vo;
        Vxz.y = 0f;

        Vector3 result = _startPosition + vo * time;
        float sY = ((-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + _startPosition.y);

        result.y = sY * _jumpMultiplier;
        
        return result;
    }

    void Visualize(Vector3 vo)
    {
        _lineRenderer.positionCount = _lineSegment + 1;
        
        for (int i = 0; i < _lineSegment; i++)
        {
            Vector3 pos = CalculatePosition(vo, i / (float)_lineSegment);
            _lineRenderer.SetPosition(i, pos); 
        }

        _lineRenderer.SetPosition(_lineSegment, _cursorPosition);
    }

    void MoveBall()
    {
        if (_checkpointIndex < _checkpoints.Length)
        {
            transform.position = Vector3.MoveTowards(transform.position, _checkpoints[_checkpointIndex], _speedMultiplier * Time.deltaTime * 5);

            if (transform.position == _checkpoints[_checkpointIndex]) 
            {
                _checkpointIndex++;
            }
        }
        else
        {
            for (int i = 0; i < _checkpoints.Length; i++)
            {
                _checkpoints[i] += (_cursorPosition - _startPosition);
                _checkpoints[i] -= ((_checkpoints[i] - transform.position) * (1 - (_bounceMultiplier / 10)));
            }
            _cursorPosition = _checkpoints[(_checkpoints.Length - 1)];
            _startPosition = transform.position;


            _jumpMultiplier *= (_bounceMultiplier / 10);

            if (_jumpMultiplier > _minimumJump) _checkpointIndex = 1;
            else Destroy(gameObject);
        }
    }
}
