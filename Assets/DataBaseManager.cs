using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public ItemDataSO itemDataSO;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public ItemData GetItemData(int searchId) {
        return itemDataSO.itemDataList.FirstOrDefault(data => data.id == searchId);
    }
}
