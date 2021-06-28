using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneToLoadName", menuName = "Scene to load")]
public class SceneToLoad : ScriptableObject
{
    [SerializeField]
    private string sceneName;
}
