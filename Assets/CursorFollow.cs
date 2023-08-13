using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorFollow : MonoBehaviour
{
    public bool follow = true;
    public Vector3 offset = new Vector3(16,-16,0);
    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        HideCursorOverlay();
    }

    public void HideCursorOverlay()
    {
        image.color = new Color(255, 255, 255, 0.0f);
    }

    // Update is called once per frame
    bool moving = false;
    Vector3 startPos = new Vector3();
    Vector3 destPos = new Vector3();
    float startTime;
    float totalTime;
    void Update()
    {
        if (follow)
            transform.position = Input.mousePosition + offset;
        else if (moving)
        {
            float tFrac = (Time.time - startTime) / totalTime;
            transform.position = Vector3.Lerp(startPos, destPos, tFrac);
            if (tFrac > 1)
            {
                moving = false;
            }
        }
    }

    public void RemoveControl()
    {
        follow = false;
    }

    public void RegainControl()
    {
        follow = true;
    }

    public void ShowCursorOverlay()
    {
        image.color = Color.white;
    }

    public void MoveTo(Vector3 dest,float time)
    {
        //Setup
        follow = false;
        moving = true;
        ShowCursorOverlay();

        //Time
        startTime = Time.time;
        totalTime = time;

        //Pos
        startPos = transform.position;
        destPos = dest;
    }
}
