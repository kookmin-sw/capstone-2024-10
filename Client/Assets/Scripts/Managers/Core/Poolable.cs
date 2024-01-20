using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링을 적용하고 싶은 물체에 해당 스크립트를 붙이고 ResourceManager를 통해 사용하면 된다.
/// </summary>
public class Poolable : MonoBehaviour
{
    public bool IsUsing;
}
