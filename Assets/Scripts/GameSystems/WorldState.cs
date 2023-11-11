using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.Character;

public class WorldState{
    public static WorldState instance; // Just so the world state can be accessed anywhere in the world

    /// <summary>
    /// Folder which holds all the tears
    /// </summary>
    Transform tearsHolder; 

    /// <summary>
    /// The number of tears remaining in the game
    /// </summary>
    /// <value></value>
    public int numberOfTears{
        get{ return tearsHolder.childCount; }
    }

    /// <summary>
    /// Set in constructor, this is the number of tears that were created at the start of the level
    /// </summary>
    public readonly int numberOfTearsCreated; 

    Utilities.MinMax playerSearchRadius;
    public float PLAYER_SEARCH_RADIUS{
        get{ return Utilities.Remap(1 - GetGameProgress(1), 0, 1, playerSearchRadius.min, playerSearchRadius.max);}
    }

    public bool isBeingChased = false;


    /// <summary>
    /// Read by the AI, this is used so that the AI is always within a radius of the player (Which should get smaller as more tears are collected)
    /// </summary>
    public CharacterBase player;
    public CharacterBase wendeergo;

    /// <summary>
    /// Constructs a new world state instance
    /// </summary>
    /// <param name="player"></param>
    /// <param name="tearsHolder"></param>
    public WorldState(CharacterBase player, CharacterBase wendeergo, Utilities.MinMax playerSearchRadius, Transform tearsHolder = null){
        this.player = player;
        this.wendeergo = wendeergo;
        this.playerSearchRadius = playerSearchRadius;
        this.tearsHolder = tearsHolder;
        this.numberOfTearsCreated = tearsHolder.childCount;
        instance = this; // Resets the instance -- Needed as statics don't generally reset on their own cross scene
    }

    public float GetGameProgress(int subtractDelta = 0) => 1-((float)(WorldState.instance.numberOfTears - subtractDelta) / (float)(WorldState.instance.numberOfTearsCreated - subtractDelta));
}
