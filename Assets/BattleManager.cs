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
    public float battleDuration = 5.0f; // バトルの制限時間（秒）

    private CancellationTokenSource cts;

    // プレイヤー用のアイテムリスト
    public List<BackPackInItem> playerBackPackItemList = new List<BackPackInItem>();

    // 敵用のアイテムリスト
    public List<BackPackInItem> enemyBackPackItemList = new List<BackPackInItem>();

    [Header("デバッグ"), Space(2)]
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

        // バトル制限時間を管理するタスクを開始
        ManageBattleDuration(cts.Token).Forget();

        // デバッグ用
        if (playerItemNoList.Count > 0) {
            for (int i = 0; i < playerItemNoList.Count; i++) {
                ItemData itemData = DataBaseManager.instance.GetItemData(playerItemNoList[i]);
                playerBackPackItemList[i].itemData = itemData;
            }
        }

        // プレイヤー用のアイテムを処理
        playerBackPackItemList.ForEach(item => item.Hoge(item.itemData, cts.Token, EntityType.Player).Forget());

        // 敵用のアイテムを処理
        enemyBackPackItemList.ForEach(item => item.Hoge(item.itemData, cts.Token, EntityType.Enemy).Forget());
    }

    public void StopBattle() {
        if (cts != null) {
            cts.Cancel(); // すべての Hoge メソッドの実行をキャンセルする
        }
    }

    private async UniTask ManageBattleDuration(CancellationToken token) {
        try {
            await UniTask.Delay((int)(battleDuration * 1000), cancellationToken: token);
            StopBattle();
        }
        catch (OperationCanceledException) {
            // バトルが手動でキャンセルされた時もここに来る
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