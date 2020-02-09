using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;
using System;

public class PenguinArea : Area
{
    public PenguinAgent penguinAgent;
    public GameObject penguinBaby;
    public Baby savePrefab;
    public Baby babyPrefab;
    public TextMeshPro cumulativeRewardText;

    [HideInInspector]
    public float fishSpeed = 0f;
    [HideInInspector]
    public float feedRadius = 1f;

    private List<GameObject> saveList;
    private List<GameObject> savedList;

    public AudioSource babySound;
    public AudioSource penguinSound;


    public override void ResetArea()
    {
        RemoveAllBabies();
        PlacePenguin();
        PlaceBaby();
        SpawnBabies(4, fishSpeed);
    }

    public void RemoveSpecificBaby(GameObject fishObject)
    {
        saveList.Remove(fishObject);
        Destroy(fishObject);
    }

    public void AddBaby(Vector3 pos)
    {
        GameObject fishObject = Instantiate<GameObject>(babyPrefab.gameObject);
        fishObject.transform.position = pos;
        fishObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        savedList.Add(fishObject);
    }

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float radius = minRadius;

        if (maxRadius > minRadius)
        {
            radius = UnityEngine.Random.Range(minRadius, maxRadius);
        }

        return center + Quaternion.Euler(0f, UnityEngine.Random.Range(minAngle, maxAngle), 0f) * Vector3.forward * radius;
    }

    private void RemoveAllBabies()
    {
        if (saveList != null)
        {
            for (int i = 0; i < saveList.Count; i++)
            {
                if (saveList[i] != null)
                {
                    Destroy(saveList[i]);
                }
            }
        }

        if (savedList != null)
        {
            for (int i = 0; i < savedList.Count; i++)
            {
                if (savedList[i] != null)
                {
                    Destroy(savedList[i]);
                }
            }
        }

        saveList = new List<GameObject>();
        savedList = new List<GameObject>();
    }

    private void PlacePenguin()
    {
        penguinAgent.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, 0f, 9f) + Vector3.up * .5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    private void PlaceBaby()
    {
        penguinBaby.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up * .5f;
        penguinBaby.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void SpawnBabies(int count, float fishSpeed)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject saveObject = Instantiate<GameObject>(savePrefab.gameObject);
            saveObject.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up * .5f;
            saveObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            saveObject.transform.parent = transform;
            saveList.Add(saveObject);
        }
    }

    private void Update()
    {
        cumulativeRewardText.text = penguinAgent.GetCumulativeReward().ToString("0.00");
    }
}
