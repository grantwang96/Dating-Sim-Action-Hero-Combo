using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour {
    [SerializeField] private int _widthBuffer;
    [SerializeField] private int _heightBuffer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetPosition();
    }

    // sets the position of the object tracking the player
    private void SetPosition() {
        // ignore if player does not exist yet
        if(PlayerUnit.Instance == null) {
            return;
        }
        // get the player's current position(s)
        IntVector3 mapPosition = PlayerUnit.Instance.MoveController.MapPosition;
        Vector3 position = PlayerUnit.Instance.MoveController.Body.position;
        Vector3 newPosition = transform.position;
        // update the x position only if the x coordinate is within the buffered space
        if (mapPosition.x + _widthBuffer <= LevelDataManager.Instance.MapBoundsX &&
           mapPosition.x - _widthBuffer >= 0) {
            newPosition.x = position.x;
        }
        // update the y position only if the y coordinate is within the buffered space
        if (mapPosition.y + _heightBuffer <= LevelDataManager.Instance.MapBoundsY &&
           mapPosition.y - _heightBuffer >= 0) {
            newPosition.y = position.y;
        }
        transform.position = newPosition;
    }

    private static bool IsWithinMapBound(int mapBound, int position) {
        return position >= 0 && position <= mapBound;
    }

    private bool IsWithinMapBuffer(IntVector3 mapPosition) {
        bool isWithinMapBuffer = LevelDataManager.Instance.IsWithinMap(mapPosition + new IntVector3(_widthBuffer, _heightBuffer));
        isWithinMapBuffer &= LevelDataManager.Instance.IsWithinMap(mapPosition + new IntVector3(-_widthBuffer, _heightBuffer));
        isWithinMapBuffer &= LevelDataManager.Instance.IsWithinMap(mapPosition + new IntVector3(_widthBuffer, -_heightBuffer));
        isWithinMapBuffer &= LevelDataManager.Instance.IsWithinMap(mapPosition + new IntVector3(-_widthBuffer, -_heightBuffer));
        return isWithinMapBuffer;
    }
}
