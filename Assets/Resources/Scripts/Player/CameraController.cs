using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform followTrans;
    [SerializeField] private float speed;

    private Vector3 offset = new Vector3(0f, 0f, 0f);

    private void Start()
    {
        offset.z = transform.position.z - followTrans.position.z;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, followTrans.position + offset, Time.deltaTime * speed);
    }
}
