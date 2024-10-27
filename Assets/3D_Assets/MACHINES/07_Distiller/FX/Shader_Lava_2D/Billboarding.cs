using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboarding : MonoBehaviour
{
    Vector3 cameraDir;

  

    // Update is called once per frame
    void Update()
    {
        cameraDir = Camera.main.transform.forward;
        cameraDir.y = 0;

        transform.rotation = Quaternion.LookRotation(cameraDir);
    }
}
