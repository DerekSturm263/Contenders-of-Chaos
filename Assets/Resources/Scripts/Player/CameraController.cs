using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTrans;
    [SerializeField] private float speed;

    private Vector3 offset;

    private void Awake()
    {
        offset = transform.position - followTrans.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, followTrans.position + offset, Time.deltaTime * speed);
    }
}
