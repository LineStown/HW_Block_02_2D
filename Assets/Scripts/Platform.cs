using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;

    protected Transform _target;
    protected bool _inited = false;
    protected bool _busy = false;

    // update
    private void Update()
    {
        if (_inited)
            UpdatePlatform();
    }

    // get width
    public float Width()
    {
        return GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public void Init(Transform target)
    {
        _target = target;
        _inited = true; 
    }

    public bool Busy()
    {
        return _busy;
    }

    public void Busy(bool value)
    {
        _busy = value;
        gameObject.SetActive(value);

    }

    protected virtual void UpdatePlatform()
    {
        _collider.enabled = _target.transform.position.y > transform.position.y;
    }
}
