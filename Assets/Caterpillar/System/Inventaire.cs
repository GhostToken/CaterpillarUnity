using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventaire : MonoBehaviour
{

    #region Identifiers

    public static string NumberOfItemsIdentifier
    {
        get
        {
            return "NumberOfItems";
        }
    }

    public static string GhostTokensIdentifier
    {
        get
        {
            return ECurrency.GhostTokens.ToIdentifier();
        }
    }

    public static string EnergieIdentifier
    {
        get
        {
            return ECurrency.Energy.ToIdentifier();
        }
    }

    public static string ItemIdentifier
    {
        get
        {
            return "Item_";
        }
    }

    public static int GameCost
    {
        get
        {
            return 50;
        }
    }

    #endregion

    #region Events

    public static event Action OnInventoryUpdate;

    #endregion

    #region Properties

    static Dictionary<string, int> VirtualCurrency = new Dictionary<string, int>();
    static List<ItemInstance> Inventory = new List<ItemInstance>();

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        Load_LocalStorage();
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        Save_LocalStorage();
    }

    #endregion

    #region Static Public Methods

    public static int GetCurrency(ECurrency Currency)
    {
        string identifier = Currency.ToIdentifier();
        if (VirtualCurrency.ContainsKey(identifier) == true )
        {
            return VirtualCurrency[identifier];
        }
        return 0;
    }

    public static bool CanStartGame()
    {
        return (GetCurrency(ECurrency.Energy) > GameCost);
    }

    public static void ConsumeGameCost()
    {
        ConsumeCurrency(ECurrency.Energy, GameCost);
    }

    #endregion

    #region Static Private Methods

    private static void ConsumeCurrency(ECurrency _Currency, int _Amount)
    {
        SubtractUserVirtualCurrencyRequest request = new SubtractUserVirtualCurrencyRequest()
        {
            Amount = _Amount,
            VirtualCurrency = _Currency.ToIdentifier()
        };
        PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnTransactionResult,
        (error) =>
        {
            Debug.Log("Got error substractin user currency :");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private static void OnTransactionResult(ModifyUserVirtualCurrencyResult Result)
    {
        VirtualCurrency[Result.VirtualCurrency] = Result.Balance;
        OnInventoryUpdate();
    }

    #endregion

    #region Private Methods

    private void Load_LocalStorage()
    {
        VirtualCurrency[GhostTokensIdentifier] = PlayerPrefsHelpers.TryGet(GhostTokensIdentifier, 0);
        VirtualCurrency[EnergieIdentifier] = PlayerPrefsHelpers.TryGet(EnergieIdentifier, 0);

        int numberOfItemsToLoad = PlayerPrefsHelpers.TryGet(NumberOfItemsIdentifier, 0);

        ISerializerPlugin serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
        for (int index = 0; index < Inventory.Count; index++)
        {
            string ItemJson = PlayerPrefsHelpers.TryGet(ItemIdentifier + index.ToString("000"), "");
            if(string.IsNullOrWhiteSpace(ItemJson) == false)
            {
                ItemInstance Item = serializer.DeserializeObject<ItemInstance>(ItemJson);
            }
        }
    }

    private static void Save_LocalStorage()
    {
        if (VirtualCurrency.ContainsKey(GhostTokensIdentifier))
        {
            PlayerPrefs.SetInt(GhostTokensIdentifier, VirtualCurrency[GhostTokensIdentifier]);
        }
        if (VirtualCurrency.ContainsKey(EnergieIdentifier))
        {
            PlayerPrefs.SetInt(EnergieIdentifier, VirtualCurrency[EnergieIdentifier]);
        }

        PlayerPrefs.SetInt(NumberOfItemsIdentifier, Inventory.Count);
        for( int index = 0; index < Inventory.Count; index++)
        {
            PlayerPrefs.SetString(ItemIdentifier + index.ToString("000"), Inventory[index].ToJson());
        }
    }

    #endregion

    #region Playfab Pull Call

    public static void OnGetLocalInventory(GetUserInventoryResult Result)
    {
        VirtualCurrency = Result.VirtualCurrency;
        Inventory = Result.Inventory;
        OnInventoryUpdate();
    }

    #endregion
}
