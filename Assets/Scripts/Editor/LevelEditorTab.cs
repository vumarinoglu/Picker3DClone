﻿using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class LevelEditorTab : EditorTab
{
    private int prevWidth = -1;
    private int prevLenght = -1;

    private enum BrushType
    {
        COLLECTIBLE,
        OBSTACLE,
        BOOSTER,
        ERASER
    }

    private BrushType currentBrushType;
    private ObstacleType currentObstacleType;
    private BoosterType currentBoosterType;
    private CollectibleType currentCollectibleType;

    private enum BrushMode
    {
        Tile,
        Row,
        Column,
        Fill
    }

    private BrushMode currentBrushMode = BrushMode.Tile;

    private readonly Dictionary<string, Texture> tileTextures = new Dictionary<string, Texture>();

    private Level currentLevel;

    private ReorderableList goalList;
    //private int currentGoal;

    private ReorderableList availableColorBlocksList;
    //private ColorBlockType currentColorBlock;

    private Vector2 scrollPos;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="editor">The parent editor.</param>
    public LevelEditorTab(Picker3DEditor editor) : base(editor)
    {
        var editorImagesPath = new DirectoryInfo(Application.dataPath + "/Resources/Game");
        var fileInfo = editorImagesPath.GetFiles("*.png", SearchOption.TopDirectoryOnly);
        foreach (var file in fileInfo)
        {
            var filename = Path.GetFileNameWithoutExtension(file.Name);
            tileTextures[filename] = Resources.Load("Game/" + filename) as Texture;
        }
    }

    /// <summary>
    /// Called when this tab is drawn.
    /// </summary>
    public override void Draw()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        var oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 90;

        GUILayout.Space(15);

        DrawMenu();

        if (currentLevel != null)
        {
            var level = currentLevel;
            prevWidth = level.width;

            GUILayout.Space(15);

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();
            DrawGeneralSettings();
            GUILayout.Space(15);
            //DrawInGameBoosterSettings();
            GUILayout.EndVertical();

            GUILayout.Space(300);

            /*GUILayout.BeginVertical();
            DrawGoalSettings();
            GUILayout.Space(15);
            DrawAvailableColorBlockSettings();
            GUILayout.EndVertical();*/

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(15);

            DrawLevelEditor();
        }

        EditorGUIUtility.labelWidth = oldLabelWidth;
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws the menu.
    /// </summary>
    private void DrawMenu()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("New", GUILayout.Width(100), GUILayout.Height(50)))
        {
            currentLevel = new Level();
            //currentGoal = null;
            InitializeNewLevel();
            //CreateGoalsList();
            //CreateAvailableColorBlocksList();
        }

        if (GUILayout.Button("Open", GUILayout.Width(100), GUILayout.Height(50)))
        {
            var path = EditorUtility.OpenFilePanel("Open level", Application.dataPath + "/PuzzleMatchKit/Resources/Levels",
                "json");
            if (!string.IsNullOrEmpty(path))
            {
                currentLevel = LoadJsonFile<Level>(path);
                //CreateGoalsList();
                //CreateAvailableColorBlocksList();
            }
        }

        if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(50)))
        {
            SaveLevel(Application.dataPath + "/Resources");
        }

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the general settings.
    /// </summary>
    private void DrawGeneralSettings()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        EditorGUILayout.HelpBox(
            "The general settings of this level.",
            MessageType.Info);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Level number", "The number of this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.id = EditorGUILayout.IntField(currentLevel.id, GUILayout.Width(30));
        GUILayout.EndHorizontal();

        /*GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Limit type", "The limit type of this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.limitType = (LimitType)EditorGUILayout.EnumPopup(currentLevel.limitType, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (currentLevel.limitType == LimitType.Moves)
        {
            EditorGUILayout.LabelField(new GUIContent("Moves", "The maximum number of moves of this level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
        }
        else if (currentLevel.limitType == LimitType.Time)
        {
            EditorGUILayout.LabelField(new GUIContent("Time", "The maximum number of seconds of this level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
        }
        currentLevel.limit = EditorGUILayout.IntField(currentLevel.limit, GUILayout.Width(30));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Penalty", "The penalty when missing a match."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.penalty = EditorGUILayout.IntField(currentLevel.penalty, GUILayout.Width(30));
        GUILayout.EndHorizonal();*/

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("First Goal", "The collectibles needed to bring end of first chapter of the level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.firstGoal = EditorGUILayout.IntField(currentLevel.firstGoal, GUILayout.Width(70));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Second Goal", "The collectibles needed to bring end of second chapter of the level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.secondGoal = EditorGUILayout.IntField(currentLevel.secondGoal, GUILayout.Width(70));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Final Goal", "The collectibles needed to bring end of final chapter of the level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.finalGoal = EditorGUILayout.IntField(currentLevel.finalGoal, GUILayout.Width(70));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Level color", "The color of road in this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.roadColor = EditorGUILayout.ColorField(currentLevel.roadColor, GUILayout.Width(70));
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the in-game booster settings.
    /// </summary>
    private void DrawInGameBoosterSettings()
    {
        var oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 110;

        GUILayout.BeginVertical();

        EditorGUILayout.LabelField("In-game boosters", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        EditorGUILayout.HelpBox(
            "The in-game booster settings of this level.",
            MessageType.Info);
        GUILayout.EndHorizontal();

        foreach (var booster in Enum.GetValues(typeof(BoosterType)))
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(/*StringUtils.DisplayCamelCaseString(*/booster.ToString()/*)*/);
            /*var availableBoosters = currentLevel.availableBoosters;
            availableBoosters[(BoosterType)booster] =
                EditorGUILayout.Toggle(availableBoosters[(BoosterType)booster], GUILayout.Width(30));*/
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        EditorGUIUtility.labelWidth = oldLabelWidth;
    }

    /// <summary>
    /// Draws the goal settings.
    /// </summary>
    private void DrawGoalSettings()
    {
        EditorGUILayout.LabelField("Goals", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        EditorGUILayout.HelpBox(
            "This list defines the goals needed to be achieved by the player in order to complete this level.",
            MessageType.Info);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(250));
        if (goalList != null)
        {
            goalList.DoLayoutList();
        }
        GUILayout.EndVertical();

        /*if (currentGoal != null)
        {
            DrawGoal(currentGoal);
        }*/

        GUILayout.EndHorizontal();

        /*if (currentLevel.limitType == LimitType.Moves)
        {
            EditorGUIUtility.labelWidth = 100;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Award boosters",
                    "Enable this if you want boosters equal to the number of remaining moves to be awarded to the player at the end of the game."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevel.awardBoostersWithRemainingMoves =
                EditorGUILayout.Toggle(currentLevel.awardBoostersWithRemainingMoves);
            GUILayout.EndHorizontal();

            if (currentLevel.awardBoostersWithRemainingMoves)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Booster", "The type of booster to award."),
                    GUILayout.Width(EditorGUIUtility.labelWidth));
                currentLevel.awardedBoosterType =
                    (BoosterType)EditorGUILayout.EnumPopup(currentLevel.awardedBoosterType, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
            EditorGUIUtility.labelWidth = 90;
        }*/
    }

    /// <summary>
    /// Draws the available color block settings.
    /// </summary>
    private void DrawAvailableColorBlockSettings()
    {
        GUILayout.BeginVertical();

        EditorGUILayout.LabelField("Available color blocks", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        EditorGUILayout.HelpBox(
            "This list defines the available color blocks when a new random color block is created.",
            MessageType.Info);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(250));
        if (availableColorBlocksList != null)
        {
            availableColorBlocksList.DoLayoutList();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 120;
        GUILayout.BeginHorizontal();
        /*EditorGUILayout.LabelField(new GUIContent("Collectable chance",
                "The random chance of a collectable block to be created."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.collectableChance = EditorGUILayout.IntField(currentLevel.collectableChance, GUILayout.Width(30));*/
        GUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 90;

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the level editor.
    /// </summary>
    private void DrawLevelEditor()
    {
        EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        EditorGUILayout.HelpBox(
            "The layout settings of this level.",
            MessageType.Info);
        GUILayout.EndHorizontal();

        /*GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Width", "The width of this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.width = EditorGUILayout.IntField(currentLevel.width, GUILayout.Width(30));
        GUILayout.EndHorizontal();*/

        //prevLenght = currentLevel.length;

        /*GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Height", "The height of this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        //currentLevel.length = EditorGUILayout.IntField(currentLevel.length, GUILayout.Width(30));
        GUILayout.EndHorizontal();*/

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Brush type", "The current type of brush."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentBrushType = (BrushType)EditorGUILayout.EnumPopup(currentBrushType, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        if (currentBrushType == BrushType.COLLECTIBLE)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Collectible", "The current type of collectible."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentCollectibleType = (CollectibleType)EditorGUILayout.EnumPopup(currentCollectibleType, GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }
        else if (currentBrushType == BrushType.OBSTACLE)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Obstacle", "The current type of obstacle."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentObstacleType =
                (ObstacleType)EditorGUILayout.EnumPopup(currentObstacleType, GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }
        else if (currentBrushType == BrushType.BOOSTER)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Booster", "The current type of booster."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentBoosterType =
                (BoosterType)EditorGUILayout.EnumPopup(currentBoosterType, GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Brush mode", "The current brush mode."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentBrushMode = (BrushMode)EditorGUILayout.EnumPopup(currentBrushMode, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (prevWidth != currentLevel.width || prevLenght != currentLevel.length)
        {

        }

        if(currentLevel.width > 0)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent("First chapter", "First chapter of current level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));

            GUILayout.Space(currentLevel.width * 30);

            EditorGUILayout.LabelField(new GUIContent("Second chapter", "Second chapter of current level."), 
                GUILayout.Width(EditorGUIUtility.labelWidth));

            GUILayout.Space(currentLevel.width * 30);

            EditorGUILayout.LabelField(new GUIContent("Final chapter", "Final chapter of current level."), 
                GUILayout.Width(EditorGUIUtility.labelWidth));

            GUILayout.EndHorizontal();
        }

        for (var i = 0; i < currentLevel.length; i++)
        {
            GUILayout.BeginHorizontal();
            for (var j = 0; j < currentLevel.width; j++)
            {
                var tileIndex = (currentLevel.width * i) + j;
                CreateButton(currentLevel.firstChapterTiles, tileIndex);
            }

            GUILayout.Space(50);

            for (var j = 0; j < currentLevel.width; j++)
            {
                var tileIndex = (currentLevel.width * i) + j;
                CreateButton(currentLevel.secondChapterTiles, tileIndex);
            }

            GUILayout.Space(50);

            for (var j = 0; j < currentLevel.width; j++)
            {
                var tileIndex = (currentLevel.width * i) + j;
                CreateButton(currentLevel.finalChapterTiles, tileIndex);
            }

            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Initializes a newly-created level.
    /// </summary>
    private void InitializeNewLevel()
    {
        /*foreach (var type in Enum.GetValues(typeof(ColorBlockType)))
        {
            currentLevel.availableColors.Add((ColorBlockType)type);
        }

        foreach (var type in Enum.GetValues(typeof(BoosterType)))
        {
            currentLevel.availableBoosters.Add((BoosterType)type, true);
        }*/

        for (var i = 0; i < currentLevel.width; i++)
        {
            for (var j = 0; j < currentLevel.length; j++)
            {
                currentLevel.firstChapterTiles.Add(null);
                currentLevel.secondChapterTiles.Add(null);
                currentLevel.finalChapterTiles.Add(null);
            }
        }
    }

    /// <summary>
    /// Creates the list of goals of this level.
    /// </summary>
    private void CreateGoalsList()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("First Goal", "The first goal of this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.firstGoal = EditorGUILayout.IntField(currentLevel.firstGoal, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Second Goal", "The second goal of this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.secondGoal = EditorGUILayout.IntField(currentLevel.secondGoal, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Final Goal", "The final goal of this level."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevel.finalGoal = EditorGUILayout.IntField(currentLevel.finalGoal, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        /*goalList = SetupReorderableList("Goals", currentLevel.goals, ref currentGoal, (rect, x) =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                x.ToString());
        },
            (x) =>
            {
                currentGoal = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var goalTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Goal));
                foreach (var type in goalTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false,
                        CreateGoalCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentGoal = null;
            });*/
    }

    /// <summary>
    /// Callback to call when a new goal is created.
    /// </summary>
    /// <param name="obj">The type of object to create.</param>
    private void CreateGoalCallback(object obj)
    {
        /*var goal = Activator.CreateInstance((Type)obj) as Goal;
        currentLevel.goals.Add(goal);*/
    }

    /// <summary>
    /// Creates the list of available color blocks of this level.
    /// </summary>
    private void CreateAvailableColorBlocksList()
    {
        /*availableColorBlocksList = SetupReorderableList("Color blocks", currentLevel.availableColors, ref currentColorBlock, (rect, x) =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                x.ToString());
        },
            (x) =>
            {
                currentColorBlock = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                foreach (var type in Enum.GetValues(typeof(ColorBlockType)))
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.ToString())), false,
                        CreateColorBlockTypeCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentColorBlock = ColorBlockType.ColorBlock1;
            });
        availableColorBlocksList.onRemoveCallback = l =>
        {
            if (currentLevel.availableColors.Count == 1)
            {
                EditorUtility.DisplayDialog("Warning", "You need at least one color block type.", "Ok");
            }
            else
            {
                if (!EditorUtility.DisplayDialog("Warning!",
                    "Are you sure you want to delete this item?", "Yes", "No"))
                {
                    return;
                }
                currentColorBlock = ColorBlockType.ColorBlock1;
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            }
        };*/
    }

    /// <summary>
    /// Callback to call when a new color block type is created.
    /// </summary>
    /// <param name="obj">The type of object to create.</param>
    private void CreateColorBlockTypeCallback(object obj)
    {
        /*var color = (ColorBlockType)obj;
        if (currentLevel.availableColors.Contains(color))
        {
            EditorUtility.DisplayDialog("Warning", "This color block type is already present in the list.", "Ok");
        }
        else
        {
            currentLevel.availableColors.Add(color);
        }*/
    }

    /// <summary>
    /// Creates a new tile button.
    /// </summary>
    /// <param name="tileIndex">The tile index.</param>
    private void CreateButton(List<LevelTile> tiles, int tileIndex)
    {
        var tileTypeName = string.Empty;

        if (tiles[tileIndex] is CollectibleTile)
        {
            var blockTile = (CollectibleTile)tiles[tileIndex];
            tileTypeName = blockTile.type.ToString();
        }
        else if (tiles[tileIndex] is BoosterTile)
        {
            var boosterTile = (BoosterTile)tiles[tileIndex];
            tileTypeName = boosterTile.type.ToString();
        }
        else if (tiles[tileIndex] is ObstacleTile)
        {
            var boosterTile = (ObstacleTile)tiles[tileIndex];
            tileTypeName = boosterTile.type.ToString();
        }

        if (tileTextures.ContainsKey(tileTypeName))
        {
            if (GUILayout.Button(tileTextures[tileTypeName], GUILayout.Width(30), GUILayout.Height(30)))
            {
                DrawTile(tiles, tileIndex);
            }
        }
        else
        {
            if (GUILayout.Button("", GUILayout.Width(30), GUILayout.Height(30)))
            {
                DrawTile(tiles, tileIndex);
            }
        }
    }

    /// <summary>
    /// Draws the tile at the specified index.
    /// </summary>
    /// <param name="tileIndex">The tile index.</param>
    private void DrawTile(List<LevelTile> tiles, int tileIndex)
    {
        var x = tileIndex % currentLevel.width;
        var y = tileIndex / currentLevel.width;
        if (currentBrushType == BrushType.COLLECTIBLE)
        {
            switch (currentBrushMode)
            {
                case BrushMode.Tile:
                    tiles[tileIndex] = new CollectibleTile { type = currentCollectibleType };
                    break;

                case BrushMode.Row:
                    for (var i = 0; i < currentLevel.width; i++)
                    {
                        var idx = i + (y * currentLevel.width);
                        tiles[idx] = new CollectibleTile { type = currentCollectibleType };
                    }
                    break;

                case BrushMode.Column:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        var idx = x + (j * currentLevel.width);
                        tiles[idx] = new CollectibleTile { type = currentCollectibleType };
                    }
                    break;

                case BrushMode.Fill:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        for (var i = 0; i < currentLevel.width; i++)
                        {
                            var idx = i + (j * currentLevel.width);
                            tiles[idx] = new CollectibleTile { type = currentCollectibleType };
                        }
                    }
                    break;
            }
        }
        else if (currentBrushType == BrushType.BOOSTER)
        {
            switch (currentBrushMode)
            {
                case BrushMode.Tile:
                    tiles[tileIndex] = new BoosterTile { type = currentBoosterType };
                    break;

                case BrushMode.Row:
                    for (var i = 0; i < currentLevel.width; i++)
                    {
                        var idx = i + (y * currentLevel.width);
                        tiles[idx] = new BoosterTile { type = currentBoosterType };
                    }
                    break;

                case BrushMode.Column:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        var idx = x + (j * currentLevel.width);
                        tiles[idx] = new BoosterTile { type = currentBoosterType };
                    }
                    break;

                case BrushMode.Fill:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        for (var i = 0; i < currentLevel.width; i++)
                        {
                            var idx = i + (j * currentLevel.width);
                            tiles[idx] = new BoosterTile { type = currentBoosterType };
                        }
                    }
                    break;
            }
        }
        else if (currentBrushType == BrushType.OBSTACLE)
        {
            switch (currentBrushMode)
            {
                case BrushMode.Tile:
                    tiles[tileIndex] = new ObstacleTile { type = currentObstacleType };
                    break;

                case BrushMode.Row:
                    for (var i = 0; i < currentLevel.width; i++)
                    {
                        var idx = i + (y * currentLevel.width);
                        tiles[idx] = new ObstacleTile { type = currentObstacleType };
                    }
                    break;

                case BrushMode.Column:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        var idx = x + (j * currentLevel.width);
                        tiles[idx] = new ObstacleTile { type = currentObstacleType };
                    }
                    break;

                case BrushMode.Fill:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        for (var i = 0; i < currentLevel.width; i++)
                        {
                            var idx = i + (j * currentLevel.width);
                            tiles[idx] = new ObstacleTile { type = currentObstacleType };
                        }
                    }
                    break;
            }
        }
        else if (currentBrushType == BrushType.ERASER)
        {
            switch (currentBrushMode)
            {
                case BrushMode.Tile:
                    tiles[tileIndex] = null;
                    break;

                case BrushMode.Row:
                    for (var i = 0; i < currentLevel.width; i++)
                    {
                        var idx = i + (y * currentLevel.width);
                        tiles[idx] = null;
                    }
                    break;

                case BrushMode.Column:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        var idx = x + (j * currentLevel.width);
                        tiles[idx] = null;
                    }
                    break;

                case BrushMode.Fill:
                    for (var j = 0; j < currentLevel.length; j++)
                    {
                        for (var i = 0; i < currentLevel.width; i++)
                        {
                            var idx = i + (j * currentLevel.width);
                            tiles[idx] = null;
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Draws the specified goal item.
    /// </summary>
    /// <param name="goal">The goal item to draw.</param>
    private void DrawGoal(/*Goal goal*/)
    {
        var oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60;

        /*goal.Draw();*/

        EditorGUIUtility.labelWidth = oldLabelWidth;
    }

    /// <summary>
    /// Saves the current level to the specified path.
    /// </summary>
    /// <param name="path">The path to which to save the current level.</param>
    public void SaveLevel(string path)
    {
#if UNITY_EDITOR
        SaveJsonFile(path + "/Levels/" + currentLevel.id + ".json", currentLevel);
        AssetDatabase.Refresh();
#endif
    }
}

