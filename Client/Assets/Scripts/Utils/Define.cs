using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public const int PlayerCount = 3;

    public enum WorldObject
    {
        Unknown,
        Player,
        Fish,
        AquariumFish,
    }

    public enum State
    {
        Moving,
        Idle,
    }

    public enum Layer
    {
        Ground = 9,
        Block = 10,
    }

    public enum Scene
    {
        Game,
        Lobby,
        Unknown,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
        Down,
        Up,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }
}
