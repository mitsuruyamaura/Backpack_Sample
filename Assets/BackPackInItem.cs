using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;
using UniRx;

public class BackPackInItem : MonoBehaviour
{
    [SerializeField]
    private Image imgIconGauge;

    public ItemData itemData;

    public float currentCoolTime;
    public float currentAccuracy;
    public float currentAttackSpeed;
    public int currentMinDamage;
    public int currentMaxDamage;
    public float currentStaminaCost;
    public float currentStaminaSpeed;

    private Tweener tweener;
    private Subject<Unit> onCancel = new Subject<Unit>();
    public IObservable<Unit> OnCancel => onCancel;



    public async UniTask Hoge(ItemData itemData, CancellationToken token, EntityType myEntityType) {
        try {
            // キャンセルされていない間
            while (!token.IsCancellationRequested) {
                imgIconGauge.fillAmount = 0;

                // バフ、デバフを適用して現在値を算出

                // 
                tweener = imgIconGauge.DOFillAmount(1.0f, currentAttackSpeed).SetEase(Ease.Linear).SetLink(gameObject);

                
                // キャンセル時の処理を登録(このスコープ内でキャンセルされた場合に、この処理が実行される)
                using (token.Register(() => {
                    tweener?.Kill();
                })) {
                    // 攻撃までの待機時間
                    // 小数点以下も含めた精度を保つために、適切なスケーリングを行う
                    int attackInterval = Mathf.CeilToInt(currentAttackSpeed * 1000); // 1000 でスケールして切り上げ
                    await UniTask.Delay(attackInterval, cancellationToken: token);
                }

                // どちらかの hp が 0 になっていないか EntityType で判定
                // 残っている場合には下の処理へ。残っていない場合には終了
                // どちらかの hp が 0 になっていないか EntityType で判定
                bool isEndBattle = BattleManager.Instance.CheckEndCondition();
                if (isEndBattle) {
                    break;
                }

                // スタミナが残っているか判定(スタミナコストのみ見る)
                // 残っている場合には下の処理へ。残っていない場合には終了


                // コスト消費


                // 命中判定
                // currentAccuracy を 0-100 の範囲にスケーリング
                float scaledAccuracy = currentAccuracy * 100f;

                // 0-100 の範囲のランダムな値を生成
                int randomValue = UnityEngine.Random.Range(0, 100);

                // 命中判定
                if (randomValue <= scaledAccuracy) {
                    // 命中した場合の処理
                    Debug.Log("Hit!");

                    // 攻撃力設定
                    int damage = UnityEngine.Random.Range(currentMinDamage, currentMaxDamage);

                    // ダメージ処理

                } else {
                    // 失敗した場合の処理
                    Debug.Log("Miss!");
                }

                // クールタイム設定
                int coolTimeInterval = Mathf.CeilToInt(currentAccuracy * 1000);
                await UniTask.Delay(coolTimeInterval, cancellationToken: token);

                // どちらかの hp が 0 になっていないか EntityType で判定
                isEndBattle = BattleManager.Instance.CheckEndCondition();
                if (isEndBattle) {
                    break;
                }

                // 残っている場合には再度同じメソッドを呼ぶ
                //await Hoge(itemData, token, myEntityType);
            }
        }
        catch (OperationCanceledException) {
            // while 文を抜けてキャンセルされた場合の処理
            tweener?.Kill();
            tweener = null;

            // キャンセルされたことを通知
            onCancel.OnNext(Unit.Default);
        }
    }
}