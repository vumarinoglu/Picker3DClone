/// <summary>
/// The base class used for the tiles in the visual editor.
/// </summary>
public class LevelTile
{
   
}

/// <summary>
/// The class used for block tiles.
/// </summary>
public class CollectibleTile : LevelTile
{
    public CollectibleType type;
}

/// <summary>
/// The class used for booster tiles.
/// </summary>
public class BoosterTile : LevelTile
{
    public BoosterType type;
}

/// <summary>
/// The class used for obstacle tiles.
/// </summary>
public class ObstacleTile : LevelTile
{
    public ObstacleType type;
}

