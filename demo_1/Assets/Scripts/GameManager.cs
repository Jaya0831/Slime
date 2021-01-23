using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 单例模式
    /// </summary>
    private static GameManager _intance;
    public static GameManager gameManager
    {
        get
        {
            return _intance;
        }
    }
    private void Awake()
    {
        _intance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
