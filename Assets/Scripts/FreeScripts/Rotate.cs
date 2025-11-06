using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject Target;
    public float RotationPerSec = 0.1f;
    public Vector3 Axis = Vector3.up;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Target.transform.Rotate(Axis, this.RotationPerSec * 360f * Time.deltaTime);
    }
}
