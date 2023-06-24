
using UnityEngine;

public interface IPlayerController
{
    Transform CharacterTransform { get; }
    Renderer CharacterRenderer { get; }
    GameController GameController { get; }
    void Init(GameController gameController);
    void DoMovement();
}
