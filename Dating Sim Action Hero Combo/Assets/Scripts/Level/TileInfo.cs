
public interface ITileInfo {
    int X { get; }
    int Y { get; }
    TileData Data { get; }

    void UpdateTile(TileData newData);
}

public class TileInfo : ITileInfo {

    public int X { get; private set; }
    public int Y { get; private set; }
    public TileData Data { get; private set; }

    public TileInfo(int x, int y, TileData type) {
        X = x;
        Y = y;
        Data = type;
    }

    public void UpdateTile(TileData newData) {
        Data = newData;
    }
}
