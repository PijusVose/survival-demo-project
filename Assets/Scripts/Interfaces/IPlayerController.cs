
using UnityEngine;

public interface IPlayerController
{
    Transform CharacterTransform { get; }
    Renderer CharacterRenderer { get; }
    void Init();
    void DoMovement();
}
