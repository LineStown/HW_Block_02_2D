using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Platform Prefab")]
    [SerializeField] private List<Platform> _platformPrefabs;

    [Header("Bottom Tramsform")]
    [SerializeField] private Transform _bottom;

    [Header("PlayerTarget Transform")]
    [SerializeField] private Transform _playerTarget;

    [Header("Spawn Settings")]
    [SerializeField] private float _platformMinXFromPrevious;
    [SerializeField] private float _platformMaxXFromPrevious;
    [SerializeField] private float _platformMinYFromPrevious;
    [SerializeField] private float _platformMaxYFromPrevious;
    [SerializeField] private int _maxSpawnSteps;

    private float _mixX;
    private float _maxX;
    private int _maxPlatformsOnLine;

    private List<Platform> _platformPool;
    private List<Platform[]> _platformList;

    // awake
    private void Awake()
    {
        // get screen size
        _mixX = Utility.GetMinX();
        _maxX = Utility.GetMaxX();
        _maxPlatformsOnLine = Mathf.FloorToInt(_maxX * 2 / _platformPrefabs[0].Width() * 0.5f);
        Debug.Log("Max Platforms On line " + _maxPlatformsOnLine);
        _platformPool = new List<Platform>();
        _platformList = new List<Platform[]>();
    }

    // update
    private void Update()
    {
        DetectPlace();
    }

    private void DetectPlace()
    {
        // detect pos
        int pos = -1;
        for (int i = 0; i < _platformList.Count(); i++)
            if (_playerTarget.position.y > _platformList[i][0].transform.position.y)
                pos = i;

        // check up
        if (_platformList.Count() - pos < _maxSpawnSteps)
            SpawnPlatform(_maxSpawnSteps - (_platformList.Count() - pos), 1);
        if (_platformList.Count() - pos > _maxSpawnSteps)
            DropPlatforms((_platformList.Count() - pos) - _maxSpawnSteps, 1);

        // check down
        if (pos < _maxSpawnSteps)
            SpawnPlatform(_maxSpawnSteps - pos, -1);
        if (pos > _maxSpawnSteps)
            DropPlatforms(pos - _maxSpawnSteps, -1);
    }

    private void SpawnPlatform(int steps, int direction)
    {
        for (int stepId = 0; stepId < steps; stepId++)
        {
            int maxPlatforms = Random.Range(1, _maxPlatformsOnLine + 1);
            Platform[] spawnedPlatformGroup = new Platform[maxPlatforms];
            for (int platformId = 0; platformId < maxPlatforms; platformId++)
            {
                Platform platformType = _platformPrefabs[(Random.Range(0, 100) < 75) ? 0 : 1];
                float spawnedPlatformX;
                float spawnedPlatformY;
                bool badPlace;
                int badPlaceCount = 0;
                do
                {
                    // generate pos
                    if (_platformList.Count() == 0)
                    {
                        spawnedPlatformX = Random.Range(_mixX, _maxX);
                        spawnedPlatformY = _playerTarget.position.y + Random.Range(_platformMinYFromPrevious, _platformMaxYFromPrevious) / 2 * direction;
                    }
                    else
                    {
                        Platform lastPlatform = (direction == 1) ? _platformList.Last()[0] : _platformList.First()[0];
                        spawnedPlatformX = (platformId == 0) ? lastPlatform.transform.position.x + Random.Range(_platformMinXFromPrevious, _platformMaxXFromPrevious) : Random.Range(_mixX, _maxX);
                        spawnedPlatformY = lastPlatform.transform.position.y + Random.Range(_platformMinYFromPrevious, _platformMaxYFromPrevious) * direction;
                    }

                    // check platform on screen
                    if (spawnedPlatformX < _mixX + platformType.Width() / 2)
                        spawnedPlatformX = _mixX + platformType.Width() / 2;
                    if (spawnedPlatformX > _maxX - platformType.Width() / 2)
                        spawnedPlatformX = _maxX - platformType.Width() / 2;

                    // check previous platform on this line 100 tries
                    badPlace = false;
                    for (int createdPlatformId = 0; createdPlatformId < platformId; createdPlatformId++)
                        if (Mathf.Abs(spawnedPlatformGroup[createdPlatformId].transform.position.x - spawnedPlatformX) < platformType.Width())
                        {
                            badPlace = true;
                            badPlaceCount++;
                            break;
                        }
                }
                while (badPlace && badPlaceCount < 100);

                // apply
                Platform spawnedPlatform = GetPlatformFromPool(platformType);
                spawnedPlatform.transform.position = new Vector3(spawnedPlatformX, spawnedPlatformY, 0);
                spawnedPlatform.Busy(true);
                spawnedPlatformGroup[platformId] = spawnedPlatform;
            }
            if (direction == 1)
                _platformList.Add(spawnedPlatformGroup);
            else
                _platformList.Insert(0, spawnedPlatformGroup);
        }
    }

    private Platform GetPlatformFromPool(Platform platformType)
    {
        // check pool
        foreach (Platform p in _platformPool)
        {
            if (p.GetType() == platformType.GetType() && !p.Busy())
                return p;
        }

        // add new platform to pool
        Platform platform = Instantiate(platformType);
        platform.Init(_playerTarget);
        platform.Busy(false);
        _platformPool.Add(platform);
        return platform;
    }

    private void DropPlatforms(int steps, int direction)
    {
        Platform[] pg;
        for (int i = 0; i < steps; i++)
        {
            if (direction == -1)
                pg = _platformList.First();
            else
                pg = _platformList.Last();

            foreach (Platform p in pg)
                p.Busy(false);
            if (direction == -1)
                _platformList.RemoveAt(0);
            else
                _platformList.RemoveAt(_platformList.Count() - 1);
        }   
    }
}
