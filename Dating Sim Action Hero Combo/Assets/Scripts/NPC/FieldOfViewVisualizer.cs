using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewVisualizer : MonoBehaviour
{
    [SerializeField] private NPCTargetManager _targetManager;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _fovResolution;
    [SerializeField] private int _edgeResolveIterations;
    [SerializeField] private float _edgeDistanceThreshold;
    [SerializeField] private PlayerOutfitState _visualizeOutfitState;

    private Mesh _visionMesh;
    private bool _isMatchingPlayerOutfitState;
    private bool _active;

    private void Start() {
        PlayerOutfitController.Instance.OnOutfitChangeStarted += OnPlayerOutfitChanged;
        _visionMesh = new Mesh();
        _visionMesh.name = "View Mesh";
        _meshFilter.mesh = _visionMesh;
        OnPlayerOutfitChanged(PlayerOutfitController.Instance.OutfitState);
    }

    private void OnDestroy() {
        PlayerOutfitController.Instance.OnOutfitChangeStarted -= OnPlayerOutfitChanged;
    }

    private void OnPlayerOutfitChanged(PlayerOutfitState newOutfitState) {
        _isMatchingPlayerOutfitState = newOutfitState == _visualizeOutfitState;
        UpdateShouldVisualize();
    }

    public void SetActive(bool active) {
        _active = active;
        UpdateShouldVisualize();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (ShouldVisualize()) {
            DrawFieldOfVision();
        }
    }

    private void DrawFieldOfVision() {
        // get vision increments
        int stepCount = Mathf.RoundToInt(_targetManager.VisionAngle * _fovResolution);
        float stepAngleSize = _targetManager.VisionAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        // generate list of viewpoints based on angle and resolution
        for(int i = 0; i <= stepCount * 2; i++) {
            float angle = (_targetManager.transform.eulerAngles.z - _targetManager.VisionAngle + stepAngleSize * i) - 180;
            ViewCastInfo viewCastInfo = ViewCast(angle);
            if(i > 0) {
                if(ShouldPerformEdgeCast(oldViewCast, viewCastInfo)) {
                    EdgeInfo edge = FindEdge(oldViewCast, viewCastInfo);
                    if(edge.PointA != Vector3.zero) {
                        viewPoints.Add(edge.PointA);
                    }
                    if(edge.PointB != Vector3.zero) {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }
            viewPoints.Add(viewCastInfo.Point);
            oldViewCast = viewCastInfo;
        }

        // build list of vertices and triangles
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        // this mesh is built on a child object. So the first position is the local origin
        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount - 1; i++) {
            vertices[i + 1] = _targetManager.transform.InverseTransformPoint(viewPoints[i]);

            // set the vertices of the triangle
            if (i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }
        }
        // apply vertices and triangles to the mesh
        _visionMesh.Clear();
        _visionMesh.vertices = vertices;
        _visionMesh.triangles = triangles;
        _visionMesh.RecalculateNormals();
    }

    private ViewCastInfo ViewCast(float angle) {
        // create and initialize a ViewCastInfo
        ViewCastInfo info = new ViewCastInfo();
        Vector2 direction = ExtraMath.AngleToVector2(angle);
        info.Point = _targetManager.transform.position + (Vector3)direction * _targetManager.VisionRange;
        info.Distance = _targetManager.VisionRange;
        info.Angle = angle;
        // check if something was hit
        RaycastHit2D hit = Physics2D.Raycast(_targetManager.transform.position, direction, _targetManager.VisionRange, _targetManager.VisionLayers);
        // if something was hit, modify the ViewCastInfo
        if (hit.transform != null) {
            info.Hit = true;
            info.Point = hit.point;
            info.Distance = hit.distance;
        }
        return info;
    }

    private bool ShouldPerformEdgeCast(ViewCastInfo oldViewCast, ViewCastInfo newViewCast) {
        bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.Distance - newViewCast.Distance) > _edgeDistanceThreshold;
        return oldViewCast.Hit != newViewCast.Hit || (oldViewCast.Hit && newViewCast.Hit && edgeDistanceThresholdExceeded);
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i = 0; i < _edgeResolveIterations; i++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.Distance - maxViewCast.Distance) > _edgeDistanceThreshold;
            if (newViewCast.Hit == minViewCast.Hit && !edgeDistanceThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.Point;
            } else {
                maxAngle = angle;
                maxPoint = maxViewCast.Point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    private void UpdateShouldVisualize() {
        _meshRenderer.enabled = ShouldVisualize();
    }

    private bool ShouldVisualize() {
        return _isMatchingPlayerOutfitState && _active;
    }
}

public struct ViewCastInfo {
    public bool Hit;
    public Vector3 Point;
    public float Distance;
    public float Angle;

    public ViewCastInfo(bool hit, Vector3 point, float distance, float angle) {
        Hit = hit;
        Point = point;
        Distance = distance;
        Angle = angle;
    }
}

public struct EdgeInfo {
    public Vector3 PointA;
    public Vector3 PointB;

    public EdgeInfo(Vector3 pointA, Vector3 pointB) {
        PointA = pointA;
        PointB = pointB;
    }
}
