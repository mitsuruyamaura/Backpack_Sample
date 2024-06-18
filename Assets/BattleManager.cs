using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UniRx;
using System;

public enum EntityType {
    Player,
    Enemy
}

public class BattleManager : MonoBehaviour {

    public static BattleManager Instance { get; private set; }

    public ReactiveProperty<int> PlayerHP = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> EnemyHP = new ReactiveProperty<int>(100);
    public float battleDuration = 5.0f; // �o�g���̐������ԁi�b�j

    private CancellationTokenSource cts;

    // �v���C���[�p�̃A�C�e�����X�g
    public List<BackPackInItem> playerBackPackItemList = new List<BackPackInItem>();

    // �G�p�̃A�C�e�����X�g
    public List<BackPackInItem> enemyBackPackItemList = new List<BackPackInItem>();

    [Header("�f�o�b�O"), Space(2)]
    [SerializeField] private List<int> playerItemNoList = new();
    [SerializeField] private List<int> enemyItemNoList = new();


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnEnable() {
        PlayerHP.Subscribe(hp => CheckEndCondition());
        EnemyHP.Subscribe(hp => CheckEndCondition());
    }

    public void StartBattle() {
        cts = new CancellationTokenSource();

        // �o�g���������Ԃ��Ǘ�����^�X�N���J�n
        ManageBattleDuration(cts.Token).Forget();

        // �f�o�b�O�p
        if (playerItemNoList.Count > 0) {
            for (int i = 0; i < playerItemNoList.Count; i++) {
                ItemData itemData = DataBaseManager.instance.GetItemData(playerItemNoList[i]);
                playerBackPackItemList[i].itemData = itemData;
            }
        }

        // �v���C���[�p�̃A�C�e��������
        playerBackPackItemList.ForEach(item => item.Hoge(item.itemData, cts.Token, EntityType.Player).Forget());

        // �G�p�̃A�C�e��������
        enemyBackPackItemList.ForEach(item => item.Hoge(item.itemData, cts.Token, EntityType.Enemy).Forget());
    }

    public void StopBattle() {
        if (cts != null) {
            cts.Cancel(); // ���ׂĂ� Hoge ���\�b�h�̎��s���L�����Z������
        }
    }

    private async UniTask ManageBattleDuration(CancellationToken token) {
        try {
            await UniTask.Delay((int)(battleDuration * 1000), cancellationToken: token);
            StopBattle();
        }
        catch (OperationCanceledException) {
            // �o�g�����蓮�ŃL�����Z�����ꂽ���������ɗ���
        }
    }

    public bool CheckEndCondition() {
        if (PlayerHP.Value <= 0 || EnemyHP.Value <= 0) {
            StopBattle();
            return true;
        }
        return false;
    }
}