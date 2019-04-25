﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public ShapeCreator shapeCreator;
    public GameObject prefabToSpawn;
    public Creature creature;

    [Range(0, 50)]
    public short creatureAmountToSpawn = 10;

    public void Update() {
        if(Input.GetButtonDown("Jump")) {
            StartCoroutine(SpawnCreature(creatureAmountToSpawn));
        }
    }

    public IEnumerator SpawnCreature(int amount) {
        SpawnManager.Instance.DeleteAllChilds(SpawnManager.Instance.transform);

        for(int i = 0; i < amount; i++) {
            SpawnManager.Instance.Spawn(creature);
            yield return new WaitForSeconds(.2f);
        }
    }

    public void SpawnAll(GameObject gameObject, Shape shape)
    {
        List<Vector3> possibleSpawnPoints = GetPossibleSpawnPoints(shape);

        for(int i = 0; i < possibleSpawnPoints.Count; i++) {
            GameObject.Instantiate(prefabToSpawn, possibleSpawnPoints[i], Quaternion.identity, transform);
        }
    }

    public static List<Vector3> GetPossibleSpawnPoints(Shape shape) {
        List<Vector3> result = new List<Vector3>();
        float minX = shape.points[0].x, maxX = shape.points[0].x;
        float minY = shape.points[0].z, maxY = shape.points[0].z;

        for(int i = 0; i < shape.points.Count; i++)
        {
            Vector3 point = shape.points[i];

            if(point.x < minX)
            {
                minX = point.x;
            }
            else if(point.x > maxX)
            {
                maxX = point.x;
            }
            if(point.z < minY)
            {
                minY = point.z;
            }
            else if(point.z > maxY)
            {
                maxY = point.z;
            }

        }

        for(int x = Mathf.CeilToInt(minX); x < maxX; x++)
        {
            for(int y = Mathf.CeilToInt(minY); y < maxY; y++)
            {
                Vector3 currentPosition = new Vector3(x, GetHeight(x, y), y);
                if(IsInPolygon(shape.points, currentPosition))
                {
                    result.Add(currentPosition);
                }
            }
        }

        return result;
    }

    public static float GetHeight(int x, int y) {
        if(Physics.Raycast(new Vector3(x, 0f, y), Vector3.down, out RaycastHit hit)) {
            return hit.point.y;
        }
        Physics.queriesHitBackfaces = true;
        if(Physics.Raycast(new Vector3(x, 0f, y), Vector3.up, out hit)) {
            Physics.queriesHitBackfaces = false;
            return hit.point.y;
        }
        Physics.queriesHitBackfaces = false;

        return .5f;
    }

    public static bool IsInPolygon(List<Vector3> polygon, Vector3 testPoint)
    {
        bool result = false;
        int j = polygon.Count - 1;
        for (int i = 0; i < polygon.Count; i++)
        {
            if (polygon[i].z < testPoint.z && polygon[j].z >= testPoint.z || polygon[j].z < testPoint.z && polygon[i].z >= testPoint.z)
            {
                if (polygon[i].x + (testPoint.z - polygon[i].z) / (polygon[j].z - polygon[i].z) * (polygon[j].x - polygon[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }

        return result;
    }
}
