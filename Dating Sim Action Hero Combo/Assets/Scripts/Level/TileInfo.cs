using System.Collections.Generic;

public interface ITileInfo {
    int X { get; }
    int Y { get; }
    TileData Data { get; }
    IReadOnlyList<ITileOccupant> Occupants { get; }

    void UpdateTile(TileData newData);
    void AddOccupant(ITileOccupant occupant);
    void RemoveOccupant(ITileOccupant occupant);
    bool ContainsOccupant(ITileOccupant occupant);
}

public class TileInfo : ITileInfo {

    public int X { get; private set; }
    public int Y { get; private set; }
    public TileData Data { get; private set; }
    public IReadOnlyList<ITileOccupant> Occupants => _occupants;

    private readonly List<ITileOccupant> _occupants = new List<ITileOccupant>();

    public TileInfo(int x, int y, TileData type) {
        X = x;
        Y = y;
        Data = type;
    }

    public void UpdateTile(TileData newData) {
        Data = newData;
    }

    public void AddOccupant(ITileOccupant occupant) {
        if (_occupants.Contains(occupant)) {
            return;
        }
        _occupants.Add(occupant);
    }

    public void RemoveOccupant(ITileOccupant occupant) {
        _occupants.Remove(occupant);
    }

    public bool ContainsOccupant(ITileOccupant occupant) {
        return _occupants.Contains(occupant);
    }
}
