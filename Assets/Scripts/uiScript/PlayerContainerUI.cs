using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContainerUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Image healthBarFill;
    public Image chargeBarFill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initialize(Color color)
    {
        scoreText.color = color;
        healthBarFill.color = color;


        scoreText.text = "0";
        healthBarFill.fillAmount = 1;
        chargeBarFill.fillAmount = 0;
    }

    public void updateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

    public void updateHealthBar(int curHp, int maxHp)
    {
        healthBarFill.fillAmount = ((float)curHp / (float)maxHp);
    }

    public void updateChargeBar(float chargedmg, float maxchargedmg)
    {
        chargeBarFill.fillAmount = chargedmg / maxchargedmg;
    } 

}
