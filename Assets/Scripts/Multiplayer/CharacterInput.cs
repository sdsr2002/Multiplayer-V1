using Unity.Netcode;
using UnityEngine;

public class CharacterInput : NetworkBehaviour
{
    [SerializeField]
    private float _speed = 1f;

    [SerializeField]
    [Tooltip("If Gravity = -0.111f gravity goes to unity default gravity On Start")]
    private float _gravity = -0.111f;
    [SerializeField]
    private float _maxGravity = -10f;
    private float _currentGravity = 0;

    [Header("Detection")]
    [SerializeField]
    private Transform _feetTransform;
    [Space]
    [SerializeField]
    private Transform _groundCheck;
    [SerializeField]
    private float _groundCheckRange = 0.45f;
    [SerializeField]
    private LayerMask _groundMask = 1<<15;
    private Collider[] _groundHits = new Collider[4];

    private bool _isGrounded;

    // Input
    private Vector3 _inputs;
    private Vector3 _horizontalInput => new Vector3(_inputs.x, 0, _inputs.z);
    private float _verticalInput => _inputs.y;
    //

    private void OnConnectedToServer()
    {
        if (_gravity == 0.111f)
            _gravity = Physics.gravity.y;
    }

    public void Update()
    {
        if (IsLocalPlayer && IsOwner)
        {
            OwnerUpdate();
        }
    }

    protected void OwnerUpdate()
    {
        UpdateGravityVel();
        InputCheck();
        CheckForGround();
    }

    private void UpdateGravityVel()
    {
        if (!_isGrounded)
        {
            _currentGravity += Time.deltaTime * _gravity;
            _currentGravity = Mathf.Max(_currentGravity, _maxGravity);
        }
    }

    private void InputCheck()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _inputs.z = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _inputs.z = -1;
        }
        else
        {
            _inputs.z = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _inputs.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _inputs.x = -1;
        }
        else
        {
            _inputs.x = 0;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _inputs.y = 1;
        }
        else
        {
            _inputs.y = 0;
        }
    }

    private void CheckForGround()
    {
        if (Physics.OverlapSphereNonAlloc(_groundCheck.position,_groundCheckRange, _groundHits, _groundMask) != 0)
        {
            if (_currentGravity < 0)
            {
                _currentGravity = 0;
                
            }
            Vector3 tempClosestGroundPosition = _groundHits[0].ClosestPoint(_feetTransform.position);

            transform.position = tempClosestGroundPosition - _feetTransform.localPosition;
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    public void FixedUpdate()
    {
        if (IsLocalPlayer && IsOwner)
        {
            OwnerFixedUpdate();
        }
    }

    protected void OwnerFixedUpdate()
    {
        transform.position += transform.rotation * _horizontalInput.normalized * _speed * Time.fixedDeltaTime;
        if (!_isGrounded)
        {
            transform.position += Vector3.up * _currentGravity * Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRange);
    }
}
