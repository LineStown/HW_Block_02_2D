using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;

    [Space]
    [Header("Target link")]
    [SerializeField] private bool _followX;
    [SerializeField] private bool _followY;
    [SerializeField] private bool _followZ;


    // start
    void Start()
    {
        
    }

    // update
    void Update()
    {
        var target_position = new Vector3(
            _followX ? _target.position.x : transform.position.x,
            _followY ? _target.position.y : transform.position.y,
            _followZ ? _target.position.z : transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target_position, _speed * Time.deltaTime);
    }
}
