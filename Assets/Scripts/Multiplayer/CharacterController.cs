using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class CharacterController : NetworkBehaviour
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

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            CameraManager.SetTarget(transform);
        }
    }

    public override void OnGainedOwnership()
    {
        //CameraManager.SetTarget(transform);
        //_rigidbody = GetComponent<Rigidbody>();
    }


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
        transform.position += transform.rotation * _horizontalInput.normalized * _speed * Time.deltaTime;
        Gravity();
        //CheckForGround();
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

            Vector3 newPos = tempClosestGroundPosition;
            var f = CheckGroundRay(newPos + Vector3.up * 4, newPos - Vector3.up * 1f);
            if (f.collider != null)
            {
                newPos = f.point;
            }
            transform.position = newPos - _feetTransform.localPosition;
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns>RaycastHit</returns>
    private RaycastHit CheckGroundRay(Vector3 startPos, Vector3 endPos)
    {
        RaycastHit[] hit = new RaycastHit[1];
        Physics.RaycastNonAlloc(startPos, endPos - startPos, hit, (endPos - startPos).magnitude, _groundMask, QueryTriggerInteraction.Ignore);
        
        return hit[0];
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
        CheckForGround();
    }

    protected void Gravity()
    {
        if (!_isGrounded)
        {
            transform.position += Vector3.up * _currentGravity * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRange);
            if (Physics.OverlapSphereNonAlloc(_groundCheck.position, _groundCheckRange, _groundHits, _groundMask) != 0)
            {
                if (_currentGravity < 0)
                {
                    _currentGravity = 0;

                }
                Vector3 tempClosestGroundPosition = _groundHits[0].ClosestPoint(_feetTransform.position);
                Gizmos.DrawCube(tempClosestGroundPosition, new Vector3(0.1f,0.1f,0.1f));
                var startPos = tempClosestGroundPosition + Vector3.up * 4;
                var endPos = tempClosestGroundPosition;
                Gizmos.DrawLine(startPos, endPos);

            }
        }
    }
}
