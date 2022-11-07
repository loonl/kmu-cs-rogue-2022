using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolDown : MonoBehaviour
{
    private static SkillCoolDown instance = null;
    public static SkillCoolDown Instance { get { return instance; } }
    
    public TextMeshProUGUI textCoolTime;
    public Image imageFill;
    
    private float timeCooltime = 0;
    private float timeCurrent;
    private float timeStart;
    private bool isEnded = true;

    void Start()
    {
        InitUI();
        TriggerSkill();
    }

    void Update()
    {
        if (isEnded)
            return;
        CheckCoolTime();
    }

    private void InitUI()
    {
        instance = this;
        imageFill.type = Image.Type.Filled;
        imageFill.fillMethod = Image.FillMethod.Radial360;
        imageFill.fillOrigin = (int)Image.Origin360.Top;
        imageFill.fillClockwise = false;
    }

    private void CheckCoolTime()
    {
        timeCurrent = Time.time - timeStart;
        if (timeCurrent < timeCooltime)
        {
            SetFillAmount(timeCooltime - timeCurrent);
        }
        else if (!isEnded)
        {
            EndCoolTime();
        }
    }

    private void EndCoolTime()
    {
        SetFillAmount(0);
        isEnded = true;
        textCoolTime.gameObject.SetActive(false);
    }

    public void TriggerSkill()
    {
        if(!isEnded)
        {
            return;
        }

        ResetCoolTime();
    }

    private void ResetCoolTime()
    {
        if (GameManager.Instance.Player.equipment != null)
        {
            timeCooltime = GameManager.Instance.Player.equipment[0].stat.coolTime;
        }
        textCoolTime.gameObject.SetActive(true);
        timeCurrent = timeCooltime;
        timeStart = Time.time;
        SetFillAmount(timeCooltime);
        isEnded = false;
    }
    private void SetFillAmount(float value)
    {
        imageFill.fillAmount = value/timeCooltime;
        string txt = value.ToString("0.0");
        textCoolTime.text = txt;
    }
}
