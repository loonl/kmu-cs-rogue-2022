using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayNANOO;
public class NanooController : MonoBehaviour
{
    // Start is called before the first frame update
    public static NanooController instance;
    
    public Plugin plugin;
    public Dictionary<string, object> list;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetPlugin()
    {
        plugin = Plugin.GetInstance();
        plugin.AccountGuestSignIn((status, errorCode, jsonString, values) =>
        {
            if (status.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(values["access_token"].ToString());
                Debug.Log(values["refresh_token"].ToString());
                Debug.Log(values["uuid"].ToString());
                Debug.Log(values["openID"].ToString());
                Debug.Log(values["nickname"].ToString());
                Debug.Log(values["linkedID"].ToString());
                Debug.Log(values["linkedType"].ToString());
                Debug.Log(values["country"].ToString());
            }
            else
            {
                if (values != null)
                {
                    if (values["ErrorCode"].ToString() == "30007")
                    {
                        Debug.Log(values["WithdrawalKey"].ToString());
                    }
                    else
                    {
                        Debug.Log("Fail");
                    }
                }
                else
                {
                    Debug.Log("Fail");
                }
            }
        });

    }
}
