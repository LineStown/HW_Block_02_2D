using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Rigidbody2D _playerRigitbody;
    [SerializeField] private CircleCollider2D _playerCollider;
    [SerializeField] private AudioSource _playerAudioSource;
    [SerializeField] private AudioClip _playerJumpSound;
    [SerializeField] private float _playerSpeed = 3.0f;
    [SerializeField] private float _playerJumpForce = 8.0f;

    [Header("Platform")]
    [SerializeField] private LayerMask _platformLayerMask;
    private float _platformDistance;
    private bool _onPlatform = false;
    private bool _onJump = false;

    private Vector3 _playerRightDirection;
    private Vector3 _playerLeftDirection;
    private float _playerDirection;

    // awake
    private void Awake()
    {
        _playerRightDirection = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _playerLeftDirection = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _playerDirection = 1;
        _platformDistance = _playerCollider.radius * _playerRigitbody.transform.localScale.y + 0.05f;
    }

    // start
    private void Start()
    {
        
    }

    // update
    private void Update()
    {
        Control();
    }

    // fixed update
    private void FixedUpdate()
    {
        // detect platform
        _onPlatform = Physics2D.Raycast(_playerRigitbody.position, Vector2.down, _platformDistance, _platformLayerMask);
        Debug.DrawRay(_playerRigitbody.position, Vector2.down * _platformDistance, Color.red);

        // run
        _playerRigitbody.linearVelocityX = _playerSpeed * _playerDirection;

        // jump
        if (_onJump)
        {
            _playerRigitbody.AddForce(Vector2.up * _playerJumpForce, ForceMode2D.Impulse);
            _playerAudioSource.PlayOneShot(_playerJumpSound);
            _onJump = false;
        }

        // left <> right
        if (_playerRigitbody.position.x > 9 || _playerRigitbody.position.x < -9)
        {
            _playerRigitbody.position = new Vector2(_playerRigitbody.position.x * -1, _playerRigitbody.position.y);
        }
    }

    // control
    private void Control()
    {
        // run
        _playerDirection = Input.GetAxisRaw("Horizontal");
        if (_playerDirection < 0)
            transform.localScale = _playerLeftDirection;
        if (_playerDirection > 0)
            transform.localScale = _playerRightDirection;

        _playerAnimator.SetBool("Run", _playerDirection != 0);

        // jump
        if (_onPlatform && !_onJump && Input.GetKeyDown(KeyCode.Space))
            _onJump = true;
        _playerAnimator.SetBool("Jump", !_onPlatform);
    }
}
