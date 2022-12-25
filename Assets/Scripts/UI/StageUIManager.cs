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
    private GameObject scoreUI;

    [SerializeField] private GameObject bossUI;

    private Slider bossHpbar;
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private GameObject backIMG;
    [SerializeField] private GameObject statusFrame;

    private bool openStatus = false;

    void Start()
    {
        // GM Player로 받아오려면 null 레퍼런스 에러 발생
        player = GameObject.Find("Player").GetComponent<Player>();
        bossHpbar = bossUI.GetComponent<Slider>();
        hpbar = playerUI.GetComponent<Slider>();
        backIMG.SetActive(false);
        statusFrame.SetActive(false);
        bossUI.SetActive(false);
        scoreUI = Resources.Load<GameObject>("Prefabs/UI/ScoreTxt");
        scoreUI = Instantiate(scoreUI, GameObject.FindWithTag("MainCanvas").transform);
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
            playerstat.text = "HP : " + player.stat.hp + "\n\n" + "SPEED : " + player.stat.speed + "\n\n" + "ATK : " +
                              player.stat.damage + "\n\n" + "SKILL DAMAGE : " + player.stat.skillDamage;
        }

        goldTxt.text = player.Inventory.Gold.ToString();
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
            CenterAlign(0);
        }
        else
            playerLeftImg.enabled = false;

        if (player.equipment[1].image)
        {
            playerHelmetImg.enabled = true;
            playerHelmetImg.sprite = player.equipment[1].image;
            playerHelmetImg.SetNativeSize();
            CenterAlign(1);
        }
        else
            playerHelmetImg.enabled = false;

        if (player.equipment[2].image)
        {
            playerArmorImg.enabled = true;
            playerArmorImg.sprite = player.equipment[2].image;
            playerArmorImg.SetNativeSize();
            CenterAlign(2);
        }
        else
            playerArmorImg.enabled = false;

        if (player.equipment[3].image)
        {
            playerRightFantsImg.enabled = playerLeftFantsImg.enabled = true;
            playerRightFantsImg.sprite = playerLeftFantsImg.sprite = player.equipment[3].image;
            playerLeftFantsImg.SetNativeSize();
            playerRightFantsImg.SetNativeSize();
            CenterAlign(3);
        }
        else
            playerRightFantsImg.enabled = playerLeftFantsImg.enabled = false;

        if (player.equipment[4].image)
        {
            playerRightImg.enabled = true;
            playerRightImg.sprite = player.equipment[4].image;
            playerRightImg.SetNativeSize();
            CenterAlign(4);
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
    
    // Sprite 중앙정렬 함수
    private void CenterAlign(int index)
    {
        // LocalPosition 기준 오차 : x좌표 = -50, y좌표 : +50

        // pants는 두 개를 보여줘야 하므로 따로 핸들링
        if (index == 3)
        {
            Image right = playerRightFantsImg, left = playerLeftFantsImg;
            
            // 정렬하기 위해 필요한 정보 가져오기
            float sizeX = right.gameObject.GetComponent<RectTransform>().rect.width;
            float sizeY = right.gameObject.GetComponent<RectTransform>().rect.height;

            // 각자 정렬 - 바지처럼 보이기 하기 위해 중앙에서 좀더 붙여줌.
            left.transform.GetComponent<RectTransform>().localPosition = new Vector2(-16 - sizeX / 2, sizeY / 2);
            right.transform.GetComponent<RectTransform>().localPosition = new Vector2(16 - sizeX / 2, sizeY / 2);
            return;
        }

        Image temp = playerLeftImg;

        switch (index)
        {
            case 0:
                temp = playerLeftImg;
                break;
            case 1:
                temp = playerHelmetImg;
                break;
            case 2:
                temp = playerArmorImg;
                break;
            case 4:
                temp = playerRightImg;
                break;
        }

        // 중앙 정렬하기 위해 필요한 정보 가져오기
        float xSize = temp.gameObject.GetComponent<RectTransform>().rect.width;
        float ySize = temp.gameObject.GetComponent<RectTransform>().rect.height;

        // 중앙 정렬
        temp.transform.GetComponent<RectTransform>().localPosition = new Vector2(-xSize / 2, ySize / 2);
    }
}
