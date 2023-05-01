using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPrompt
{
    void ShowPrompt(Transform target, Vector3 offset);
    void HidePrompt();
}
