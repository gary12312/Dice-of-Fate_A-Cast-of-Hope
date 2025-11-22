using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    bool stoped;
    bool drop;
    bool show_result;
    public int result;
    float x, y, z;

    Vector3 start_position;
    void Start()
    {
        RotateDice();
        start_position = transform.position;
    }
    void Update()
    {
       if (CheckOnStoped() && drop) { stoped = true;  }
       if (stoped && !show_result) { ShowResult(); show_result = true; }
    }

    public void DropDice()
    {
        drop = true;
        RotateDice();
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);
    }

    void RotateDice()
    {
        x = Random.Range(0, 270);
        y = Random.Range(0, 270);
        z = Random.Range(0, 270);
        transform.Rotate(x * Time.deltaTime * 500, y * Time.deltaTime * 500, z * Time.deltaTime * 500);
    }

    public void ShakeDice()
    {
        float x = Random.Range(-2, 2f);
        float y = Random.Range(-2f, 2f);
        float z = Random.Range(-2f, 2f);
        transform.Rotate(x * Time.deltaTime * 500, y * Time.deltaTime * 500, z * Time.deltaTime * 500);
    }
    bool CheckOnStoped()
    {
        if (GetComponent<Rigidbody>().linearVelocity.magnitude == 0.0f) { return true; } 
        return false;
    }
    void ShowResult()
    {
        Global.total += result;
        CanvasManager.ShowTotal(Global.total);

        switch (gameObject.name)
        {
            case "DiceGreen": CanvasManager.ShowResultGreenDice(result); break;
            case "DiceRed":   CanvasManager.ShowResultRedDice(result); break;
            case "DiceBlue":  CanvasManager.ShowResultBlueDice(result); break;
        }
    }

    public void ReturnToStartPosition()
    {
        transform.position = start_position;
        RotateDice();
    }

    public void CanDropAgain()
    {
        drop = false;
        stoped = false;
        show_result = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Table")
        {
            var audio = GetComponent<AudioSource>();
            if (!audio.isPlaying) { audio.Play(); }
        }
    }
}
