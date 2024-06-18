using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Rarity {
    Common,
    Uncommon,
    Rare,
    Mystic,
    Epic,
    Legendary,
}

public enum ItemType {
    Melee,
    LongRange,
}

[System.Serializable]
public class ItemData
{
    public int id;
    public string name;
    public Rarity rarity;
    public ItemType itemType;
    public string description;
    public int price;

    public float coolTime;
    public float accuracy;
    public float attackSpeed;
    public int minDamage;
    public int maxDamage;

    public float staminaCost;
    public float staminaSpeed;  // Žg‚í‚È‚¢—\’è

    public string effect;


    public ItemData(string[] datas) {
        id = int.Parse(datas[0]);
        name = datas[1];
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[2]);
        itemType = (ItemType)Enum.Parse(typeof(ItemType), datas[3]);
        description = datas[4];
        price = int.Parse(datas[5]);

        coolTime = float.Parse(datas[6]);
        accuracy = float.Parse(datas[7]);
        attackSpeed = float.Parse(datas[8]);
        minDamage = int.Parse(datas[9]);
        maxDamage = int.Parse(datas[10]);

        staminaCost = float.Parse(datas[11]);
        staminaSpeed = float.Parse(datas[12]);

        effect = datas.Length > 13 ? datas[13] : string.Empty;


    }
}