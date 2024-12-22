using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {  
        if(gameObject.CompareTag("Coin"))
            transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
        else if (gameObject.CompareTag("Health"))
            transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
        else
            transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);

    }

}