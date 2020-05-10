using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            // 向いている方向に経過時間分だけ動かす
            rb.position += (transform.forward * animator.GetInteger("Walk")) * Time.deltaTime;
        }
    }
}
