using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMotion : MonoBehaviour
{
    [SerializeField] GameObject _target;
    [SerializeField] Slider _jumpSlider;
    [SerializeField] Slider _speedSlider;

    [SerializeField] [Range(10, 50)] int _lineSegment = 10;

    [SerializeField] [Range(1, 20)] float _jumpMultiplier = 2;
    [SerializeField] [Range(1, 20)] float _speedMultiplier = 10;
    [SerializeField] [Range(1, 9)] float _bounciness = 5;
    [SerializeField] float _minimumJump = 0.1f;

    LineRenderer _lineRenderer;
    LayerMask _clickMask;

    Vector3 _startPosition;
    Vector3 _targetPosition;
    Vector3 _cursorPosition;
    Vector3[] _checkpoints;
    int _checkpointIndex = 1;

    bool _ballSpawned;
    bool _movementStarted;

    void Start()
    {
        _clickMask = LayerMask.GetMask("Ground");
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _target.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
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
                if (_ballSpawned)
                {
                    _checkpoints = new Vector3[_lineRenderer.positionCount];
                    _lineRenderer.GetPositions(_checkpoints);
                    _lineRenderer.enabled = false;
                    _movementStarted = true;
                }

                else
                {
                     transform.position = _cursorPosition;
                    _startPosition = transform.position;

                    transform.GetChild(0).gameObject.SetActive(true);
                    _lineRenderer.enabled = true;
                    _target.SetActive(true);
                    _ballSpawned = true;
                }
            }

            if (_ballSpawned)
            {
                _jumpMultiplier = (int)_jumpSlider.value;
                _speedMultiplier = (int)_speedSlider.value;

                Vector3 vo = CalculateVelocity(_cursorPosition, _startPosition, 1f);
                Visualize(vo);

                _target.transform.position = hit.point + Vector3.up * 0.01f;
            }
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

            if (transform.position == _checkpoints[_checkpointIndex]) _checkpointIndex++;
        }
        else
        {
            for (int i = 0; i < _lineSegment + 1; i++)
            {
                _checkpoints[i] = _checkpoints[i] + (_cursorPosition - _startPosition);
                _checkpoints[i].y *= _bounciness / 10;
            }
            _jumpMultiplier *= _bounciness / 10;

            if (_jumpMultiplier > _minimumJump) _checkpointIndex = 1;
        }
    }
}
