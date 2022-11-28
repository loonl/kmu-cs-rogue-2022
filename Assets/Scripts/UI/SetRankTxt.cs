using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetRankTxt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private TextMeshProUGUI rankNick;
    [SerializeField] private TextMeshProUGUI rankScore;
    // Start is called before the first frame update
    public void SetRank(string rank, string rankNick, string rankScore)
    {
        this.rank.text = rank;
        this.rankNick.text = rankNick;
        this.rankScore.text = rankScore;
    }
}
