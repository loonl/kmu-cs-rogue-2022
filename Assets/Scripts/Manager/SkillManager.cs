using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Player player;
    private static SkillManager _instance = null;
    public static SkillManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void InstantiateSkill(string skillname)
    {
        Debug.Log(skillname);
        GameObject skill = Instantiate(Resources.Load($"Prefabs/Skill/{skillname}") as GameObject);
        if (player.transform.localScale.x < 0)
            skill.transform.position = player.transform.position + new Vector3(skill.transform.localScale.x / 2, 0.5f, -0.1f);
        else
            skill.transform.position = player.transform.position + new Vector3(-skill.transform.localScale.x / 2, 0.5f, -0.1f);
    }

}
