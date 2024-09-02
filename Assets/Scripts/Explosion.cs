using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void Destroy()
    {
        if ((transform.parent != null) && (transform.parent.tag == "ExpParent"))
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
