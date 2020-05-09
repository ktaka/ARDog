using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// AR Foundationを使用する際は次の2つのusingを追加する
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObject : MonoBehaviour
{
    public GameObject placedPrefab; // 配置用モデルのプレハブ
    public BallControl ballControl;
    GameObject spawnedObject; // 配置モデルのプレハブから生成されたオブジェクト
    // ARRaycastManagerは画面をタッチした先に伸ばしたレイと平面の衝突を検知する
    ARRaycastManager raycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        // オブジェクトに追加されているARRaycastManagerコンポーネントを取得
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // タッチされていない場合は処理をぬける
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (HitTest(touchPosition, out Pose hitPose))
        { // タッチした先に平面がある場合
          // モデル（子猫）を配置する位置からカメラへの方向のベクトルを求めて
          // モデルをどのくらい回転させるか求める
            Quaternion rotation = Quaternion.LookRotation(GetLookVector(hitPose.position));
            if (spawnedObject == null)
            { // 配置用モデルが未生成の場合
                // プレハブから配置用モデルを生成し、レイが平面に衝突した位置に配置する
                spawnedObject = Instantiate(placedPrefab, hitPose.position, rotation);
                ballControl.petControl = spawnedObject.GetComponent<PetControl>();
            }
            else
            { // 配置するモデルが生成済みの場合
                // 配置用モデルの位置をレイが平面に衝突した位置にする
                spawnedObject.transform.position = hitPose.position;
                // 配置用モデルを回転させてカメラの方に向ける
                spawnedObject.transform.rotation = rotation;
            }
        }
    }

    // タッチ位置を取得する
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        // スマートフォンで実行される場合
        if (Input.touchCount == 2)
        {
            // 画面がタッチされた位置を取得する
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    // タッチされた先に平面があるか判定する
    // touchPosition ... タッチされた画面上の2D座標
    // hitPose ... 画面をタッチした先に伸ばしたレイと平面が衝突した位置と姿勢
    bool HitTest(Vector2 touchPosition, out Pose hitPose)
    {
        // 画面をタッチした先に伸ばしたレイと平面の衝突判定
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        { // 衝突する平面があった場合
          // 1つ目に衝突した平面と交差する位置と姿勢の情報を取得
            hitPose = hits[0].pose;
            return true;
        }
        hitPose = default;
        return false;
    }

    // 配置モデル（子猫）からカメラへの方向のベクトルを求める
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
}
