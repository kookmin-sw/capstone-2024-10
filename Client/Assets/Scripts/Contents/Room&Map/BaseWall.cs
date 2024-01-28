using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//에디터에서 벽이 소속되어있는 부모오브젝트는 오직 가시성만을 위한 것이다.
[SelectionBase]
public class BaseWall : MonoBehaviour
{
    //부서질 수 있는 벽은 바리케이트로 다시 막을 수도 있음
    public bool isBreakable;
    public bool isOpenable;
    
    // Start is called before the first frame update
    void Start()
    {
        if(isBreakable)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.AddComponent<BreakWall>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
