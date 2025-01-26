using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialFlyby : MonoBehaviour
{
    public GameObject[] asteroids;
    public GameObject[] KillVolumes;
    public GameObject SpawnVolume;
    public float velocity;
    public float minScale;
    public float maxScale;
    public float maxRotationVelocity;

    private Quaternion[] rotations;

    // Start is called before the first frame update
    void Start()
    {
        rotations = new Quaternion[asteroids.Length];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < asteroids.Length; i++)
        {
            asteroids[i].transform.position += new Vector3(0, 0, velocity * Time.deltaTime);
            asteroids[i].transform.rotation *= rotations[i];

            foreach (GameObject KillVolume in KillVolumes)
            {
                if (IsInsideCube(asteroids[i].transform.position, KillVolume.transform.position, KillVolume.transform.localScale))
                {
                    RespawnAsteroid(i);
                }
            }
        }
    }

    private void RespawnAsteroid(int i)
    {
        asteroids[i].transform.position = new Vector3(Random.Range(
                                                                            SpawnVolume.transform.position.x - SpawnVolume.transform.localScale.x / 2, 
                                                                            SpawnVolume.transform.position.x + SpawnVolume.transform.localScale.x / 2),
                                                    Random.Range(
                                                                            SpawnVolume.transform.position.y - SpawnVolume.transform.localScale.y / 2, 
                                                                            SpawnVolume.transform.position.y + SpawnVolume.transform.localScale.y / 2),
                                                    Random.Range(
                                                                            SpawnVolume.transform.position.z - SpawnVolume.transform.localScale.z / 2, 
                                                                            SpawnVolume.transform.position.z + SpawnVolume.transform.localScale.z / 2)
                                                );
        
        float scaleFactor = Random.Range(minScale, maxScale);
        asteroids[i].transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        asteroids[i].transform.rotation = 
        rotations[i] = Quaternion.Euler(GenerateRandomRotation(maxRotationVelocity));
    }

    Vector3 GenerateRandomRotation(float maxRotationalVelocity)
    {
        Vector3 randomDirection = Random.onUnitSphere;
        float randomMagnitude = Random.Range(0f, maxRotationalVelocity);
        return randomDirection * randomMagnitude;
    }
    
    bool IsInsideCube(Vector3 point, Vector3 cubeCenter, Vector3 cubeScale)
    {
        // Calculate half-extents of the cube
        Vector3 halfExtents = cubeScale / 2f;

        // Check if the point lies within the bounds on all axes
        return (point.x >= cubeCenter.x - halfExtents.x && point.x <= cubeCenter.x + halfExtents.x) &&
               (point.y >= cubeCenter.y - halfExtents.y && point.y <= cubeCenter.y + halfExtents.y) &&
               (point.z >= cubeCenter.z - halfExtents.z && point.z <= cubeCenter.z + halfExtents.z);
    }
}
