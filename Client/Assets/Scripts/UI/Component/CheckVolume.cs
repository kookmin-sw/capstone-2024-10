using UnityEngine;
using System.Collections;

public class CheckVolume : MonoBehaviour {
    public void  Start (){
        UpdateVolume();
    }

    public void UpdateVolume (){
        Managers.SoundMng.UpdateVolume();
    }
}
