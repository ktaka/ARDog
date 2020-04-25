using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallContorl : MonoBehaviour
{
    // オブジェクトを生成する際のテンプレートとなるプレハブ
    public GameObject ballObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // タッチされていない場合は処理をぬける
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }
        // タッチされた2D座標と画面の少し奥（カメラのニアクリップ面の少し先）を奥行き方向として組み合わせる
        Vector3 pos = touchPosition;
        pos.z = Camera.main.nearClipPlane * 2.0f;

        // タップした位置から奥行き方向に伸びるレイを作成
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit = new RaycastHit();
        // 伸ばしたレイにぶつかるオブジェクトがない場合、もしくは
        // レイにぶつかるオブジェクトがあった場合でもリジッドボディを持たない場合
        if (Physics.Raycast(ray, out hit) == false || hit.rigidbody == null)
        {
            // 画面のスクリーン座標を3D空間のワールド座標に変換する
            var position = Camera.main.ScreenToWorldPoint(pos);
            // 求めた座標の位置に新しいオブジェクトを生成する
            GameObject obj = Instantiate(ballObject, position, Quaternion.identity);
        }
    }

    // タッチ位置を取得する
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        // Unityエディターで実行される場合
        if (Input.GetMouseButtonDown(0))
        {
            // マウスボタンが押された位置を取得する
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        // スマートフォンで実行される場合
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                // 画面がタッチされた位置を取得する
                touchPosition = touch.position;
                return true;
            }
        }
#endif
        touchPosition = default;
        return false;
    }
}
