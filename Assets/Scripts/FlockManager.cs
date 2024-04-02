using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
public static FlockManager FM;
public GameObject GhostPrefab;
public int numGhost=10;
public GameObject[] allGhost;
public Vector3 ghostcube=new Vector3(5,5,3);
    // Start is called before the first frame update
    [Header ("GhostFlocking Setting")]
    [Range(0.0f,5.0f)]
    public float minspead;
    [Header ("GhostFlocking Setting")]
    [Range(0.0f,5.0f)]
    public float maxspeed;
    void Start()
    {
        allGhost=new GameObject[numGhost];
        for(int i=0; i<numGhost; i++)
        {
            Vector3 pos=this.transform.position+ new Vector3(Random.Range(-ghostcube.x,ghostcube.x),
                                                             Random.Range(-ghostcube.y,ghostcube.y),
                                                             Random.Range(-ghostcube.z,ghostcube.z));
                allGhost[i]=Instantiate(GhostPrefab,pos, Quaternion.identity);
        }
        FM=this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
