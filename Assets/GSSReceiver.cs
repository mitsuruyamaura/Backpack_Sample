using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

/// <summary>
/// スプレッドシートから取得したデータをシート単位で任意のスクリプタブル・オブジェクトに値として取り込む
/// </summary>
[RequireComponent(typeof(GSSReader))]
public class GSSReceiver : MonoBehaviour {

    public bool IsLoading { get; set; }


    private void Awake() {
        // GSS のデータ取得準備
        PrepareGSSLoadStartAsync().Forget();
    }

    /// <summary>
    /// GSS のデータ取得準備
    /// </summary>
    /// <returns></returns>
    private async UniTask PrepareGSSLoadStartAsync() {

        IsLoading = true;

        await GetComponent<GSSReader>().GetFromWebAsync();

        IsLoading = false;
        OnGSSLoadEnd();

        Debug.Log("GSS データを SO に取得");
    }

    /// <summary>
    /// インスペクターから GSSReader の OnLoadEnd にこのメソッドを追加することで GSS の読み込み完了時にコールバックされる
    /// </summary>
    public void OnGSSLoadEnd() {

        GSSReader reader = GetComponent<GSSReader>();

        // スプレッドシートから取得した各シートの配列を List に変換
        List<SheetData> sheetDataslist = reader.sheetDatas.ToList();

        // 情報が取得できた場合
        if (sheetDataslist != null) {

            // スクリプタブル・オブジェクトに代入
            DataBaseManager.instance.itemDataSO.itemDataList =
                new List<ItemData>(sheetDataslist.Find(x => x.SheetName == SheetName.ItemData).DatasList.Select(x => new ItemData(x)).ToList());

            // TODO 他の SO を追加する
        }
    }
}