using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;

    [SerializeField] private Slider hpbar;

    [SerializeField] private TextMeshProUGUI playerstat;

    private Player player;
    private Stat boss;

    [SerializeField] private GameObject bossUI;

    private Slider bossHpbar;
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private GameObject backIMG;
    [SerializeField] private GameObject statusFrame;

    private bool openStatus = false;
    void Start()
    {
        bossHpbar = bossUI.GetComponent<Slider>();
        hpbar = playerUI.GetComponent<Slider>();

        backIMG.SetActive(false);
        statusFrame.SetActive(false);
        bossUI.SetActive(false);
    }

    public void init(Player player)
    {
        this.player = player;
    }

    [SerializeField] private Image playerHelmetImg;

    [SerializeField] private Image playerArmorImg;

    [SerializeField] private Image playerLeftFantsImg;

    [SerializeField] private Image playerRightFantsImg;

    [SerializeField] private Image playerLeftImg;

    [SerializeField] private Image playerRightImg;

    // Update is called once per frame
    void Update()
    {
        if (playerUI.activeSelf)
        {
            hpbar.value = player.stat.hp / player.stat.maxHp;
        }
        if (bossUI.activeSelf)
        {
            bossHpbar.value = boss.hp / boss.maxHp;
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            loadStatus();
            loadItem();
            playerstat.text = "HP : " + player.stat.hp + "\n\n" + "SPEED : " + player.stat.speed + "\n\n" + "ATK : " + player.stat.damage + "\n\n" + "SKILL DAMAGE : " + player.stat.skillDamage;
        }
        
        //goldTxt.text = player.Inventory.Gold.ToString();
    }

    public void loadBossUI(Stat boss)
    {
        this.boss = boss;
        bossUI.SetActive(true);
    }

    public void loadItem()
    {
        if (player.equipment[0].image)
        {
            playerLeftImg.enabled = true;
            playerLeftImg.sprite = player.equipment[0].image;
            playerLeftImg.SetNativeSize();
        }
        else
            playerLeftImg.enabled = false;

        if (player.equipment[1].image)
        {
            playerHelmetImg.enabled = true;
            playerHelmetImg.sprite = player.equipment[1].image;
            playerHelmetImg.SetNativeSize();
        }
        else
            playerHelmetImg.enabled = false;

        if (player.equipment[2].image)
        {
            playerArmorImg.enabled = true;
            playerArmorImg.sprite = player.equipment[2].image;
            playerArmorImg.SetNativeSize();
        }
        else
            playerArmorImg.enabled = false;

        if (player.equipment[3].image)
        {
            playerRightFantsImg.enabled = playerLeftFantsImg.enabled = true;
            playerRightFantsImg.sprite = playerLeftFantsImg.sprite = player.equipment[3].image;
            playerLeftFantsImg.SetNativeSize();
            playerRightFantsImg.SetNativeSize();
        }
        else
            playerRightFantsImg.enabled = playerLeftFantsImg.enabled = false;
        
        if (player.equipment[4].image)
        {
            playerRightImg.enabled = true;
            playerRightImg.sprite = player.equipment[4].image;
            playerRightImg.SetNativeSize();
        }
        else
            playerRightImg.enabled = false;
    }

    public void loadStatus()
    {
        openStatus = !openStatus;
        backIMG.SetActive(openStatus);
        statusFrame.SetActive(openStatus);
        if (openStatus)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
