using UnityEngine;

public class PlatformV : Platform
{
    [SerializeField] private float _minTime;
    [SerializeField] private float _MaxTime;

    private void Start()
    {
        float time = Random.Range(_minTime, _MaxTime);
        InvokeRepeating(nameof(Tick), time, time);
    }

    private void Tick()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
