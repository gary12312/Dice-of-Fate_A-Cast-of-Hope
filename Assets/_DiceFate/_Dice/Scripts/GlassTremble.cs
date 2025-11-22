using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassTremble : MonoBehaviour
{
    Vector3 start_position;
    GameObject[] dice;

    bool can_shake;
    void Start()
    {
        can_shake = true;
        start_position = transform.position;
        InvokeRepeating("ReturnToStartPosition", 0.1F, 0.1F);
        FindAllDice();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
    void OnMouseDrag()
    {
        if (can_shake)
        {
            ShakeGlass();
            ShakeDice();
            PlaySoundShake();
        }
    }

    void OnMouseUp()
    {
        if (can_shake)
        {
            can_shake = false;
            transform.rotation = Quaternion.Euler(90, 0, 0);
            DropDice();
            ReturnToStartPosition();
            CancelInvoke("ReturnToStartPosition");
            Invoke("ReturnToStartRotation", 3f);
            Invoke("ReturnDiceToStartPosition", 5f);
            Invoke("CanShakeAgain", 6f);
            Invoke("ResetDiceResult", 6f);
        }
    }

    void PlaySoundShake()
    {
        var audio = GetComponent<AudioSource>();
        if (!audio.isPlaying) { audio.Play(); }
    }
    void ShakeDice()
    {
        foreach (var dices in dice)
        {
            dices.GetComponent<Cube>().ShakeDice();
        }
    }

    void DropDice()
    {
        foreach (var dices in dice)
        {
            dices.GetComponent<Cube>().DropDice();
        }
    }
    void ShakeGlass()
    {
        float x = Random.Range(-0.005f, 0.0051f);
        float y = Random.Range(-0.005f, 0.0051f);
        float z = Random.Range(-0.005f, 0.0051f);
        transform.position += new Vector3(x, y, z) * Time.deltaTime * 500;
    }

    void CanShakeAgain()
    {
        can_shake = true;
        InvokeRepeating("ReturnToStartPosition", 0.1F, 0.1F);
    }
    void FindAllDice()
    {
        dice = GameObject.FindGameObjectsWithTag("Dice");
    }
    void ReturnToStartPosition()
    {
        transform.position = start_position;
    }

    void ReturnToStartRotation()
    {
        transform.rotation = Quaternion.Euler(30, 0, 0);
    }

    void ResetDiceResult()
    {
        CanvasManager.ResetDiceResult();
    }
    void ReturnDiceToStartPosition()
    {
        foreach (var dices in dice)
        {
            dices.GetComponent<Cube>().ReturnToStartPosition();
            dices.GetComponent<Cube>().CanDropAgain();
        }
    }
}