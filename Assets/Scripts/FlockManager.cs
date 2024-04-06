using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour {

    public static FlockManager FM;
    public GameObject ghostPrefab;
    public int ghostnum = 20;
    public GameObject[] allghost;
    public Vector3 spawnlimit = new Vector3(5.0f, 5.0f, 5.0f);
    public Vector3 goalPos = Vector3.zero;

    [Header("ghost Settings")]
    [Range(0.0f, 5.0f)] public float minSpeed;
    [Range(0.0f, 5.0f)] public float maxSpeed;
    [Range(1.0f, 10.0f)] public float neighbourDistance;
    [Range(1.0f, 5.0f)] public float rotationSpeed;

    void Start() {

        allghost = new GameObject[ghostnum];

        for (int i = 0; i < ghostnum; ++i) {

            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-spawnlimit.x, spawnlimit.x),
                Random.Range(0, spawnlimit.y),
                Random.Range(-spawnlimit.z, spawnlimit.z));

            allghost[i] = Instantiate(ghostPrefab, pos, Quaternion.identity);
        }

        FM = this;
        goalPos = this.transform.position;
    }


    void Update() {

        if (Random.Range(0, 100) < 10) {

            goalPos = this.transform.position + new Vector3(
                Random.Range(-spawnlimit.x, spawnlimit.x),
                Random.Range(0, spawnlimit.y),
                Random.Range(-spawnlimit.z, spawnlimit.z));
        }
    }
}