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
            // �L�����Z������Ă��Ȃ���
            while (!token.IsCancellationRequested) {
                imgIconGauge.fillAmount = 0;

                // �o�t�A�f�o�t��K�p���Č��ݒl���Z�o

                // 
                tweener = imgIconGauge.DOFillAmount(1.0f, currentAttackSpeed).SetEase(Ease.Linear).SetLink(gameObject);

                
                // �L�����Z�����̏�����o�^(���̃X�R�[�v���ŃL�����Z�����ꂽ�ꍇ�ɁA���̏��������s�����)
                using (token.Register(() => {
                    tweener?.Kill();
                })) {
                    // �U���܂ł̑ҋ@����
                    // �����_�ȉ����܂߂����x��ۂ��߂ɁA�K�؂ȃX�P�[�����O���s��
                    int attackInterval = Mathf.CeilToInt(currentAttackSpeed * 1000); // 1000 �ŃX�P�[�����Đ؂�グ
                    await UniTask.Delay(attackInterval, cancellationToken: token);
                }

                // �ǂ��炩�� hp �� 0 �ɂȂ��Ă��Ȃ��� EntityType �Ŕ���
                // �c���Ă���ꍇ�ɂ͉��̏����ցB�c���Ă��Ȃ��ꍇ�ɂ͏I��
                // �ǂ��炩�� hp �� 0 �ɂȂ��Ă��Ȃ��� EntityType �Ŕ���
                bool isEndBattle = BattleManager.Instance.CheckEndCondition();
                if (isEndBattle) {
                    break;
                }

                // �X�^�~�i���c���Ă��邩����(�X�^�~�i�R�X�g�̂݌���)
                // �c���Ă���ꍇ�ɂ͉��̏����ցB�c���Ă��Ȃ��ꍇ�ɂ͏I��


                // �R�X�g����


                // ��������
                // currentAccuracy �� 0-100 �͈̔͂ɃX�P�[�����O
                float scaledAccuracy = currentAccuracy * 100f;

                // 0-100 �͈̔͂̃����_���Ȓl�𐶐�
                int randomValue = UnityEngine.Random.Range(0, 100);

                // ��������
                if (randomValue <= scaledAccuracy) {
                    // ���������ꍇ�̏���
                    Debug.Log("Hit!");

                    // �U���͐ݒ�
                    int damage = UnityEngine.Random.Range(currentMinDamage, currentMaxDamage);

                    // �_���[�W����

                } else {
                    // ���s�����ꍇ�̏���
                    Debug.Log("Miss!");
                }

                // �N�[���^�C���ݒ�
                int coolTimeInterval = Mathf.CeilToInt(currentAccuracy * 1000);
                await UniTask.Delay(coolTimeInterval, cancellationToken: token);

                // �ǂ��炩�� hp �� 0 �ɂȂ��Ă��Ȃ��� EntityType �Ŕ���
                isEndBattle = BattleManager.Instance.CheckEndCondition();
                if (isEndBattle) {
                    break;
                }

                // �c���Ă���ꍇ�ɂ͍ēx�������\�b�h���Ă�
                //await Hoge(itemData, token, myEntityType);
            }
        }
        catch (OperationCanceledException) {
            // while ���𔲂��ăL�����Z�����ꂽ�ꍇ�̏���
            tweener?.Kill();
            tweener = null;

            // �L�����Z�����ꂽ���Ƃ�ʒm
            onCancel.OnNext(Unit.Default);
        }
    }
}