using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
    public float speed = 1.0f;
    public float radius = 5.0f;
    public float angle = 0.0f;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        MoveInCircle();
    }

    private void MoveInCircle()
    {
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, 0.25f, z);
        angle += Time.deltaTime * speed;
    }
}
