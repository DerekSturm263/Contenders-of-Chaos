﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.iOS;
public class SpinWheel : MonoBehaviour
{
    public List<int> prize;
    public List<AnimationCurve> animationCurves;

    private bool spinning;
    private float anglePerItem;
    private int randomTime;
    private int itemNumber;

    void Start()
    {
        spinning = false;
        anglePerItem = 360 / prize.Count;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (Input.touchCount == 1)
            {
                touch = Input.GetTouch(1);

                if (!spinning)
                {
                    randomTime = Random.Range(1, 4);
                    itemNumber = 0;
                    float maxAngle = 360 * randomTime + (itemNumber * anglePerItem);

                    StartCoroutine(SpinTheWheel(5 * randomTime, maxAngle));
                }
                if (itemNumber == 1 || itemNumber == 5)
                {
                    
                }
                if (itemNumber == 2 || itemNumber == 6)
                {
                    
                }
                if (itemNumber == 3 || itemNumber == 7)
                {
                    
                }
                if (itemNumber == 4 || itemNumber == 8)
                {
                    
                }
            }
        }
    }
        IEnumerator SpinTheWheel(float time, float maxAngle)
    {
        spinning = true;

        float timer = 0.0f;
        float startAngle = transform.eulerAngles.z;
        maxAngle = maxAngle - startAngle;

        int animationCurveNumber = Random.Range(0, animationCurves.Count);
        Debug.Log("Animation Curve No. : " + animationCurveNumber);

        while (timer < time)
        {
            //to calculate rotation
            float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / time);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, angle + startAngle);
            timer += Time.deltaTime;
            yield return 0;
        }

        transform.eulerAngles = new Vector3(0.0f, 0.0f, maxAngle + startAngle);
        spinning = false;

        Debug.Log("Prize: " + prize[itemNumber]);//use prize[itemNumnber] as per requirement
    }
}