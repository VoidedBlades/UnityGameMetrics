using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField, Range(0, 100)] private float speed;


    private float RAXA = 0f;
    private float RAYA = 0f;

    float X = 0f;
    float Y = 0f;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        transform.position = player.transform.position;

        X = speed * Input.GetAxis("Mouse X") * 0.02f;
        Y = speed * Input.GetAxis("Mouse Y") * 0.02f;

        RAXA -= Y;
        RAYA += X;
        RAXA = ClampAngle(RAXA, -90f, 90f);
        Quaternion fromRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
        Quaternion ToRot = Quaternion.Euler(RAXA, RAYA, 0f);
        Quaternion rot = ToRot;

        transform.rotation = rot;

        X = Mathf.Lerp(X, 0, Time.deltaTime * 2f);
        Y = Mathf.Lerp(Y, 0, Time.deltaTime * 2f);
    }


    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
