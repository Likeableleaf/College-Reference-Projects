using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public List<string> words = new();
    public int i;
    public int s;
    public bool flag;
    public bool flag2;

    [SerializeField] TextMeshPro tut1;
    [SerializeField] TextMeshPro tut2;
    [SerializeField] Canvas canv;

    //reference to the player for use later
    public Player player;
    //reference to the turret for use later
    public TurretFire tFire;

    // Start is called before the first frame update
    void Start()
    {
        tut1.text = "";
        tut2.text = "";
        flag = false;
        flag2 = true;
        words.Add("'E' to use");
        words.Add("'E' to SHOOT");
        words.Add("");
        words.Add("");
        words.Add("");
        i = 0;
        s = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!canv.enabled)
        {
            if (words != null)
            {
                //I added a raycast check for the turret
                //that's 1:1 how the player checks if the
                //turret is there.
                RaycastHit2D hit = Physics2D.Raycast(player.transform.position, -player.transform.up, player.rayDistance, player.rCasting);

                if (Input.GetKeyDown(KeyCode.E) && hit != false && hit.collider.gameObject.CompareTag("Turret") && i <= 1)
                {
                    i++;
                }
                tut1.text = words[i];
            }
            if (i == 2)
            {
                i++;
                StartCoroutine(showText(0.2f));
/*                //LD added this, it'll slow time down so that you can read it. 
                //I may remove this if it doesn't feel good. Because,
                //yknow you're supposed to pause time yourself not have it 
                //done for you.
                StartCoroutine(GameManager.instance.slowTime(0.7f, 0.2f));*/
                //Yeah, it didn't feel right.
            }
            if (i == 3 && Input.GetKey("space"))
            {
                flag = true;
                i++;
            }
            if (i == 4 && flag2)
            {
                StartCoroutine(saveThem());
                flag2 = false;
            }
        }
        else
        {
            tut1.text = "";
        }
    }

    public void playAnimation()
    {
        //Queue<string> queue = new Queue<string>();
        StartCoroutine(showText(/*queue,*/ 0.7f));        
    }

    public IEnumerator showText(/*Queue<string> queue, */float duration)
    {/*
        while (queue.TryPeek(out string s))
        {
            textMesh.text = queue.Dequeue();
            yield return new WaitForSecondsRealtime(duration);
        }
      */
        tut2.color = Color.red;
        while(true)
        {
            tut2.text = "PRESS SPACE";
            yield return new WaitForSecondsRealtime(duration);
            tut2.text = "";
            yield return new WaitForSecondsRealtime(duration);
            if(flag)
            {
                break;
            }
        }
    }

    public IEnumerator saveThem()
    {
        tut2.color = Color.green;
        while(s < 4)
        {
            tut2.text = "SAVE THEM";
            yield return new WaitForSecondsRealtime(0.2f);
            tut2.text = "";
            yield return new WaitForSecondsRealtime(0.2f);
            s++;
        }
    }
}
