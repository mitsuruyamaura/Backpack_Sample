using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

/// <summary>
/// �X�v���b�h�V�[�g����擾�����f�[�^���V�[�g�P�ʂŔC�ӂ̃X�N���v�^�u���E�I�u�W�F�N�g�ɒl�Ƃ��Ď�荞��
/// </summary>
[RequireComponent(typeof(GSSReader))]
public class GSSReceiver : MonoBehaviour {

    public bool IsLoading { get; set; }


    private void Awake() {
        // GSS �̃f�[�^�擾����
        PrepareGSSLoadStartAsync().Forget();
    }

    /// <summary>
    /// GSS �̃f�[�^�擾����
    /// </summary>
    /// <returns></returns>
    private async UniTask PrepareGSSLoadStartAsync() {

        IsLoading = true;

        await GetComponent<GSSReader>().GetFromWebAsync();

        IsLoading = false;
        OnGSSLoadEnd();

        Debug.Log("GSS �f�[�^�� SO �Ɏ擾");
    }

    /// <summary>
    /// �C���X�y�N�^�[���� GSSReader �� OnLoadEnd �ɂ��̃��\�b�h��ǉ����邱�Ƃ� GSS �̓ǂݍ��݊������ɃR�[���o�b�N�����
    /// </summary>
    public void OnGSSLoadEnd() {

        GSSReader reader = GetComponent<GSSReader>();

        // �X�v���b�h�V�[�g����擾�����e�V�[�g�̔z��� List �ɕϊ�
        List<SheetData> sheetDataslist = reader.sheetDatas.ToList();

        // ��񂪎擾�ł����ꍇ
        if (sheetDataslist != null) {

            // �X�N���v�^�u���E�I�u�W�F�N�g�ɑ��
            DataBaseManager.instance.itemDataSO.itemDataList =
                new List<ItemData>(sheetDataslist.Find(x => x.SheetName == SheetName.ItemData).DatasList.Select(x => new ItemData(x)).ToList());

            // TODO ���� SO ��ǉ�����
        }
    }
}