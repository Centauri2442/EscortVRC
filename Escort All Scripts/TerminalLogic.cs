using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class TerminalLogic : UdonSharpBehaviour
{
    public GlobalPointsSystem Points;
    
    [Header("Terminal Settings")] 
    public string Name;
    public int Price;
    public int Stock;
    public string Description;

    public GameObject[] Items;
    public Transform BuyPoint;

    public AudioSource Audio;
    public AudioClip[] BuyClip;
    public AudioClip OutOfStockClip;

    [Header("UI Settings")] 
    public Text UIName;
    public Text UIPrice;
    public Text UIStock;
    public Text UIDescription;

    public void Start()
    {
        UIName.text = Name;
        UIPrice.text = "Price | " + Convert.ToString(Price);
        UIStock.text = "Stock | " + Convert.ToString(Stock);
        UIDescription.text = Description;
    }

    public void Buy()
    {
        if (Stock <= 0 || Points.Points < Price)
        {
            Audio.PlayOneShot(OutOfStockClip);
            return;
        }
        
        Points.RemovePoints(Price);
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkBuy");
    }

    public void NetworkBuy()
    {
        Items[Stock - 1].transform.position = BuyPoint.position;
        Items[Stock - 1].SetActive(true);
        Audio.PlayOneShot(BuyClip[UnityEngine.Random.Range(0, BuyClip.Length)]);
        Stock--;
        UIStock.text = "Stock | " + Convert.ToString(Stock);
    }
}
