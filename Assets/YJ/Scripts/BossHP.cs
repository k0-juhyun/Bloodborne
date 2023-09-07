using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHP : MonoBehaviour
{
    // 태어날 때, 체력을 최대체력으로 설정한다, UI도 같이 한다
    // 플레이어의 무기와 충돌하면 체력을 1 줄어들게 한다
    // 체력이 0이 되면 나(Boss)의 상태를 죽음 상태로 한다

    
    int hp;                         // HP
    [Header("최대 체력")]
    public int maxHP = 10;          // 보스 최대 체력

    public Slider sliderHP;         // 보스 체력 슬라이더(UI)

    public int HP //property 프로퍼티(속성)
    {
        get // 호출하는 입장
        {
            return hp;
        }
        set
        {
            hp = value;
            sliderHP.value = hp; //= value 도 됨
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // 태어날때 체력을 최대체력으로 - 프로퍼티로 만들자
        // 최대체력을 최댓값으로 한다
        sliderHP.maxValue = maxHP;
        HP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        // 개발자 모드? p 누르면 게르만 hp = 1로 만드는?
        // 근데 플레이어 죽음이 UI도 없고 다시 시작하는 상태가 없어서 필요 없으려나..
        // 물약이 무한이라 괜찮겠다
    }
}
