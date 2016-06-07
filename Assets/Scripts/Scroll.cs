using UnityEngine;
using System.Collections;

public class Scroll : MonoBehaviour {

    public float x_max_speed = 0.01f;
    public float y_max_speed = 0.01f;
    public float x_acc = 0.006f;
    public float y_acc = 0.006f;

    private float x_speed = 0;
    private float y_speed = 0;
    private float x_dir = 1;
    private float y_dir = 1;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        x_speed += Time.deltaTime * x_acc * x_dir;
        y_speed += Time.deltaTime * y_acc * y_dir;
        if ((x_speed > x_max_speed && x_dir > 0) || (x_speed < -x_max_speed && x_dir < 0)) {
            x_dir *= -1;
        }
        if ((y_speed > y_max_speed && y_dir > 0) || (y_speed < -y_max_speed && y_dir < 0)) {
            y_dir *= -1;
        }

        Vector2 offset = GetComponent<Renderer>().material.mainTextureOffset;
        offset.x += Time.deltaTime * x_speed;
        offset.y += Time.deltaTime * y_speed;
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}
