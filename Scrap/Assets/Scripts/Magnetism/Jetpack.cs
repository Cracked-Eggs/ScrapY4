using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour
{
    public float maxFuel = 4f;
    public float thrustForce = 0.5f;
    public Rigidbody rb;
    public Transform groundedTransform;

    private float curFuel;
    void Start()
    {
        curFuel = maxFuel;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space) && curFuel > 0f)
        {
            Debug.Log("pooooop");
            curFuel -= Time.deltaTime;
            rb.AddForce(rb.transform.up *  thrustForce,ForceMode.Impulse);
        }
        else if(Physics.Raycast(groundedTransform.position, Vector3.down, 0.05f, LayerMask.GetMask("Ground"))&&curFuel > 0f)
        {
            curFuel += Time.deltaTime;  
        }
        
    }
}
