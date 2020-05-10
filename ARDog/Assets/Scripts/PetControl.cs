using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetControl : MonoBehaviour
{
    Animator animator; // 子猫のアニメーター
    Rigidbody rb; // 子猫のリジッドボディ
    float arrivalTime; // 子猫が目的の位置まで移動するのにかかる時間
    public float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        // （歩く、アイドルのアニメーションを制御するため）
        animator = gameObject.GetComponent<Animator>();
        // リジッドボディを取得
        // （位置や回転を制御するため）
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 一定時間ごとに呼び出される
    private void FixedUpdate()
    {
        // 子猫が目的の位置まで移動するのにかかる時間が0になるまで動かす
        if (arrivalTime > 0.0f)
        {
            // 子猫が目的の位置まで移動するのにかかる時間を経過時間分減らす
            arrivalTime -= Time.deltaTime;
            if (arrivalTime < Mathf.Epsilon)
            {
                // 目的の位置まで移動するのにかかる時間が0になったら移動をやめる
                animator.SetInteger("Walk", 0);
            }
        }
    }

    public void MoveTo(Vector3 pos)
    {
        Vector3 planePos = pos;
        // 水平方向は現在位置のままにする
        planePos.y = rb.transform.position.y;
        // 子猫の向きを移動先の方に向ける
        rb.transform.LookAt(planePos);
        // 現在位置から移動先までのベクトルを求める
        Vector3 distanceVec = planePos - rb.transform.position;
        // 現在位置から移動先までの距離を求める
        float distance = distanceVec.magnitude;
        // 移動スピード（歩く）を設定
        animator.SetInteger("Walk", 1);
        // 指定位置まで移動するのにかかる時間を求める
        arrivalTime = distance / speed;
    }
}
