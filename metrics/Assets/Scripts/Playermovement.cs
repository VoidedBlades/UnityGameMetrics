using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Playermovement : MonoBehaviour
{
    [SerializeField, Range(0, 100)] float speed;
    [SerializeField, Range(1, 100)] private int ID;
    [SerializeField] private Metrics Data;

    private float tick = 1f;


    private void Start()
    {
       //ID = Random.Range(1, 10000);
    }

    // Update is called once per frame
    void Update()
    {
        tick += Time.deltaTime;
        Vector3 dir = new Vector3();
        Vector3 newLookat = Camera.main.transform.position + Camera.main.transform.forward * 5;
        transform.LookAt(new Vector3(newLookat.x, transform.position.y, newLookat.z));

        if (Input.GetKey(KeyCode.W))
        {
            dir += transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dir += -transform.forward;
        }

        if(Input.GetKey(KeyCode.A))
        {
            dir += -transform.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir += transform.right;
        }

        transform.position = Vector3.Lerp(transform.position, transform.position + dir, speed * Time.deltaTime);

        if (tick >= .2f)
        {
            tick = 0;
            Data.Store(new Location(transform.position, transform.position + Camera.main.transform.forward * 1f), ID);
            Data.Save(ID);
        }
    }
}
