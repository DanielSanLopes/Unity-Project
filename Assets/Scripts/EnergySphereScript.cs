using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySphereScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
    }

    void EndOfSuperShoot() {
        DanielScript.EndOfSuperFire();
    }
}
