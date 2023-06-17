using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 _3Doffset;

    public Vector3 _2Doffset;

    bool _3DCam = true;

    void Update()
    {
        if(_3DCam){
            transform.position = player.position + _3Doffset;
        }else{
            transform.position = player.position + _2Doffset;
        }
    }

    public void ChangeView(){
        if(_3DCam){
            transform.Rotate(90,0,0);
        }else{
            transform.Rotate(-90,0,0);
        }
        _3DCam = !_3DCam;
    }
}
