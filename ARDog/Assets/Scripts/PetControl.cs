using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetControl : MonoBehaviour
{
    Animator animator; // 子猫のアニメーター
    Rigidbody rb; // 子猫のリジッドボディ
    float arrivalTime; // 子猫が目的の位置まで移動するのにかかる時間
    public float speed = 1;

    public float rotateDuration = 3.0f; // 回転にかける時間（秒）
    public float delayTime = 3.0f; // 回転を始めるまで遅らせる時間（秒）
    Quaternion rotateFrom; // 回転の開始値
    Quaternion rotateTo; // 回転の終了値
    float rotateDelta; // 回転終了までの残り時間

    bool isMoving = false; // 移動中を示すフラグ

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
        // 移動中でなければ回転（ペットの向きをカメラに向ける）の処理を行う
        if (isMoving == false)
        {
            // 回転終了までの残り時間が0以下なら回転中ではないとする
            if (rotateDelta <= 0.0f)
            {
                // 歩くアニメーションを止める
                animator.SetInteger("Walk", 0);
                // ペットとカメラの位置関係と向きを確認する
                CheckObjDirection();
            }
            // 回転中の処理を行う
            else
            {
                // カメラの方を向くように回転する
                RotateCamera();
                // 回転の残り時間から経過時間だけ減らす
                rotateDelta -= Time.deltaTime;
            }
        }
    }

    // ペットとカメラの位置関係と向きを確認してペットの回転を起動する
    void CheckObjDirection()
    {
        // ペットの正面の方向を取得
        Vector3 catDirVector = rb.transform.forward;
        // ペットからカメラへの方向のベクトルを求める
        Vector3 lookVector = GetLookVector(rb.transform.position);
        // ペットの正面方向とペットからカメラへの方向のベクトルの内積を求める
        float dot = Vector3.Dot(catDirVector, lookVector);
        // 内積の値が閾値以下なら方向がずれていると判定して回転を起動する
        if (dot <= 0.5f)
        {
            // 現在の方向を示す回転の値を回転の開始値とする
            rotateFrom = rb.transform.rotation;
            // 回転終了時にペットがカメラの方向を向くようにする
            rotateTo = Quaternion.LookRotation(lookVector);
            // 目的の方向まで回転するための時間に遅延時間を足して開始のタイミングを遅らせる
            rotateDelta = rotateDuration + delayTime;
        }
    }

    // ペットを回転させる
    void RotateCamera()
    {
        // 回転終了までの残り時間が回転にかける時間以下になるまでは回転しない
        // （回転を開始する時間を遅らせる）
        if (rotateDelta <= rotateDuration)
        {
            // 歩くアニメーションを開始する
            animator.SetInteger("Walk", 1);
            // 回転終了までの時間を0.0〜1.0の範囲の値になるよう正規化する
            float t = 1.0f - (rotateDelta / rotateDuration);
            // 球面線形補間で回転の開始から終了の間の現在の経過時間の補完値を求める
            rb.transform.rotation = Quaternion.Slerp(rotateFrom, rotateTo, t);
        }
    }

    // 配置モデルからカメラへの方向のベクトルを求める
    Vector3 GetLookVector(Vector3 position)
    {
        // 2点間の位置の差分をとって方向ベクトルを求める
        Vector3 lookVector = Camera.main.transform.position - position;
        // 床面の上（XZ平面）のみを回転の対象とするため、上下方向（Y軸）の差分は無視する
        lookVector.y = 0.0f;
        // ベクトルを正規化する
        lookVector.Normalize();
        return lookVector;
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
                // 移動中を示すフラグをOFFにする
                isMoving = false;
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
        // 移動中を示すフラグをONにする
        isMoving = true;
    }
}
