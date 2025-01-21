using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WindRandomizer : MonoBehaviour
{
    [SerializeField] private EnvironmentHandler environmentHandler;
    [SerializeField] private float duration;
    [SerializeField] private GameObject[] windTemplates;
    [SerializeField] private float minTimeDelay;
    [SerializeField] private float maxTimeDelay;
    [SerializeField] private Vector3 minPosition;
    [SerializeField] private Vector3 maxPosition;
    

    private GameObject[] windBuffer;
    
    private float spawnTime;
    private void Awake()
    {
        spawnTime = Time.time;

        windBuffer = new GameObject[windTemplates.Length];
        
        for (int i = 0; i < windTemplates.Length; i++)
        {
            windBuffer[i] = Instantiate(windTemplates[i], transform);
            windBuffer[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Time.time > spawnTime)
        {
            spawnTime = Time.time + Random.Range(minTimeDelay, maxTimeDelay);
            int nextAvailableIndex = NextAvailableBuffer();
            if (nextAvailableIndex == -1)
                return;
            SpawnWind(nextAvailableIndex);
        }

        if (!environmentHandler.Moving)
            return;
        foreach (var wind in windBuffer)
        {
            if (wind.activeSelf)
                wind.transform.position += Time.deltaTime * environmentHandler.SpeedMultiplier * Vector3.back;
        }
    }

    private int NextAvailableBuffer()
    {
        windBuffer = UtilsT.Shuffle(windBuffer);
        for (int i = 0; i < windBuffer.Length; i++)
        {
            if (!windBuffer[i].activeSelf)
                return i;
        }

        return -1;
    }

    private void SpawnWind(int id)
    {
        windBuffer[id].SetActive(true);

        windBuffer[id].transform.position = new Vector3(Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z));
        
        StartCoroutine(DeSpawnWind(id));
    }

    private IEnumerator DeSpawnWind(int id)
    {
        yield return new WaitForSeconds(duration);
        windBuffer[id].SetActive(false);
    }
}
