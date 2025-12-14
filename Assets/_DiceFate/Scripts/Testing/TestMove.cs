using UnityEngine;

public class TestMove : MonoBehaviour
{
    Vector2 position;
    private float speed = 5f;
    Vector3 moving;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            moving += new Vector3(1,0,0);
        }
        

       // moving  =  new Vector3(position.x, moving.y, position.y ) *speed* Time.deltaTime;

    }
}
