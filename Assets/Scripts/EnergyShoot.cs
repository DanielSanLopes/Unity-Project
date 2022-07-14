using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShoot : MonoBehaviour
{

    private GameObject Player;
    private Transform Mira;
    private Vector3 dirDisparo;

    // Start is called before the first frame update
    void Start(){

        Player = GameObject.FindGameObjectWithTag("Player");
        


        if(Player.GetComponent<Animator>().GetBool("grabWall") == true) {

            Mira = GameObject.FindGameObjectWithTag("looker").GetComponent<Transform>();

            print("Rotation Inspector " + UnityEditor.TransformUtils.GetInspectorRotation(Mira).z);
            print("Rotation " + Mira.rotation.z);
            print("Local Rotation " + Mira.localRotation.z);
            print("Euler Angles " + Mira.eulerAngles.z);
            print("Local Euler Angles " + Mira.localEulerAngles.z);


            print("Degrees " + deltaDegrees1());
            print("Rads " + deltaDegrees1() * Mathf.Deg2Rad);



            float radDegrees = deltaDegrees1() * Mathf.Deg2Rad;
            float radDegrees2 = deltaDegrees2() * Mathf.Deg2Rad;
            float radDegrees3 = deltaDegrees3() * Mathf.Deg2Rad;
            float radDegrees4 = deltaDegrees4() * Mathf.Deg2Rad;

            if (Player.transform.localScale.x > 0) {
                if (deltaDegrees1() <= 45)
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, -(Mathf.Tan(radDegrees) * 470)));
                if (deltaDegrees1() > 45)
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2((Mathf.Tan(radDegrees2) * 500), -500));
            } else {
                if (deltaDegrees3() <= 45)
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, -(Mathf.Tan(radDegrees3) * 450)));

                if (deltaDegrees3() > 45)
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(-(Mathf.Tan(radDegrees4) * 500), -500));
            }
        } else {
            if (Player.transform.localScale.x > 0)
                this.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(500, 0));
            else
                this.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(-500, 0));
        }   
        Destroy(this.gameObject, 10);
    }



    float deltaDegrees1() {
        float degrees = 270 - Mira.localEulerAngles.z;
        return degrees;
    }


    float deltaDegrees2() {
        float degrees = Mira.localEulerAngles.z - 180;
        return degrees;
    }

    float deltaDegrees3() {
        float degrees = Mira.localEulerAngles.z - 90;
        return degrees;
    }

    float deltaDegrees4() {
        float degrees = 180 - Mira.localEulerAngles.z;
        return degrees;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
