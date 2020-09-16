using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonkey : MonoBehaviour
{
    public Vector2 TestStart;
    public Vector2 TestTarget;

    public IntVector3 StartPos;
    public IntVector3 EndPos;

    public GameObject _markerPrefab;
    public int DefaultMarkerCount;
    private List<GameObject> _markers = new List<GameObject>();
    public List<IntVector3> Path;

    public string _testEnemyType;

    private void Start() {
        for(int i = 0; i < DefaultMarkerCount; i++) {
            GenerateMarkerPrefab();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestEnemySpawn() {
        EnemyManager.Instance.SpawnEnemy(TestStart, _testEnemyType, "");
    }

    private void GenerateMarkerPrefab() {
        GameObject marker = Instantiate(_markerPrefab);
        marker.SetActive(false);
        _markers.Add(marker);
    }

    private void TestPathfinder() {
        StopCoroutine(DisplayMarkers());
        Path.Clear();
        DisableAllMarkers();
        PathStatus status = MapService.GetPathToDestination(StartPos, EndPos, Path);
        CustomLogger.Log(nameof(TestMonkey), $"Pathfinding ended with status {status}!");
        StartCoroutine(DisplayMarkers());
    }

    private IEnumerator DisplayMarkers() {
        for(int i = 0; i < Path.Count; i++) {
            if (i >= _markers.Count) {
                GenerateMarkerPrefab();
            }
            _markers[i].SetActive(true);
            Vector2 position = LevelDataManager.Instance.ArrayToWorldSpace(Path[i].x, Path[i].y);
            _markers[i].transform.position = position;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DisableAllMarkers() {
        for(int i = 0; i < _markers.Count; i++) {
            _markers[i].SetActive(false);
        }
    }
}
