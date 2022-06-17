using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int id;

    public int firstGoal;
    public int secondGoal;
    public int finalGoal;

    public Color roadColor;

    public readonly int width = 10;
    public readonly int length = 20;

    public List<LevelTile> firstChapterTiles = new List<LevelTile>();
    public List<LevelTile> secondChapterTiles = new List<LevelTile>();
    public List<LevelTile> finalChapterTiles = new List<LevelTile>();
}
