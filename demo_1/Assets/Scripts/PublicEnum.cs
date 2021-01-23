using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicEnum : MonoBehaviour
{
    ///// <summary>
    ///// 单例模式
    ///// </summary>
    //private static PublicEnum _intance;
    //public static PublicEnum publicEnum
    //{
    //    get
    //    {
    //        return _intance;
    //    }
    //}

    //private void Awake()
    //{
    //    _intance = this;
    //}
    public enum Direction
    {
        left,
        right,
        up,
        down,
        none
    }

    public enum SolidType
    {
        stone=0
    }

    public enum LiquidType//值越大，质量越大
    {
        water=0,
        lava
    }
    public static int SolidTypeNum = 1;
    public static int LiquidTypeNum = 2;
    public GameObject[] SolidPlaneSprites;
    public GameObject[] SolidHighSprites;
    public GameObject[] LiquidPlaneSprites;
    public GameObject[] LiquidHighSprites;
    public GameObject groundSprite;

    private void Start()
    {
        //Debug.Log(SolidHighSprites.Length);
    }
}
