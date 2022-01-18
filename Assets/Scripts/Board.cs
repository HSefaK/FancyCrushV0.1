using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;


    public int width;
    public int height;

    public GameObject[] blobblist;
    List<Blobb> allBlobbList = new List<Blobb>();

    public bool isSwapping;
    bool turnChecked;

    Blobb lastBloob;
    Blobb blobb1, blobb2;
    Vector3 blobb1StartPos, blobb1EndPos, blobb2StartPos, blobb2EndPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        fillBoard();
        StartCoroutine(permaBoardCheck());
    }

    void fillBoard()
    {
        int [] previousLeft = new int[height];
        int previousBelow = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                

                List<GameObject> possibleBlobbs = new List<GameObject>();
                possibleBlobbs.AddRange(blobblist);
                for (int k = 0; k >= 0; k--)
                {
                    int idB = possibleBlobbs[k].GetComponent<Blobb>().blobbId;
                    if(idB == previousLeft[j] || idB == previousBelow)
                    {
                        possibleBlobbs.RemoveAt(k);
                    }
                }
                int randomizer = Random.Range(0, blobblist.Length);
                GameObject newBlobb = Instantiate(blobblist[randomizer], new Vector3(transform.position.x + i, transform.position.y + j, 0), Quaternion.identity) as GameObject;

                Blobb nB = newBlobb.GetComponent<Blobb>();
                allBlobbList.Add(nB);

                newBlobb.transform.parent = this.transform;
                previousBelow = nB.blobbId;
                previousLeft[j] = nB.blobbId;
            }
        }
    }

    void togglePhysics(bool on)
    {
        for (int i = 0; i< allBlobbList.Count; i++)
        {
            allBlobbList[i].GetComponent<Rigidbody>().isKinematic = on;

        }
    }
    bool moveToSwapLocation(Blobb b, Vector3 swapGoal)
    {
        return b.transform.position != (b.transform.position = Vector3.MoveTowards(b.transform.position, swapGoal, 4 * Time.deltaTime));

    }

    public void createNewBlobb(Blobb b, Vector3 pos)
    {
        allBlobbList.Remove(b);

        int randBlobb = Random.Range(0, blobblist.Length);
        GameObject newBlobb = Instantiate(blobblist[randBlobb],new Vector3(pos.x,pos.y + 9f,pos.z),Quaternion.identity);

        allBlobbList.Add(newBlobb.GetComponent<Blobb>());
        newBlobb.transform.parent = transform;
    }

    public void swapBlobbs(Blobb currentBloob)
    {
        if (lastBloob == null)
        {
            lastBloob = currentBloob;
        }
        else if (lastBloob == currentBloob)
        {
            lastBloob = null;
        }
        else
        {
            if (lastBloob.checkIfNeighbour(currentBloob))
            {
                blobb1 = lastBloob;
                blobb2 = currentBloob;

                blobb1StartPos = lastBloob.transform.position;
                blobb1EndPos = currentBloob.transform.position;

                blobb2StartPos = currentBloob.transform.position;
                blobb2EndPos = lastBloob.transform.position;

                StartCoroutine(swapBlobbs());
            }

            else
            {
                lastBloob.ToggleSelector();
                lastBloob = currentBloob;
            }
        }
    }


    IEnumerator swapBlobbs()
    {
        if (isSwapping)
        {
            yield break;
        }

        isSwapping = true;
        togglePhysics(true);
        while (moveToSwapLocation(blobb1, blobb1EndPos) && moveToSwapLocation(blobb2, blobb2EndPos)) { yield return null; }

        blobb1.clearAllMatches();
        blobb2.clearAllMatches();

        while (!turnChecked) { yield return null; }
        bool blobb1MatchFound = blobb1.matchFound;
        bool blobb2MatchFound = blobb2.matchFound;
        if (!blobb1MatchFound && !blobb2MatchFound)
        {
            while (moveToSwapLocation(blobb1, blobb1StartPos) && moveToSwapLocation(blobb2, blobb2StartPos)) { yield return null; }
        }

        if(blobb1MatchFound || blobb2MatchFound)
        {
            GameManager.instance.updateMatach();
        }

        turnChecked = false;
        isSwapping = false;
        togglePhysics(false);
        lastBloob = null;
        blobb1.ToggleSelector();
        blobb2.ToggleSelector();

    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Gizmos.DrawWireCube(new Vector3(transform.position.x + i, transform.position.y + j, 0),new Vector3(1,1,1));
            }
        }
    }

#endif

   public void reportTurnDone()
    {
        turnChecked = true;
    }

    public bool checkIfBoardIsMoving()
    {
        for (int i= 0; i< allBlobbList.Count; i++)
        {
            if(allBlobbList[i].transform.localPosition.y >7.2f)
            {
                return true;
            }
            //if(allBlobbList[i].GetComponent<Rigidbody>().velocity.y > 0.001f)
            if(!allBlobbList[i].GetComponent<Rigidbody>().IsSleeping())
            {
                return true;
            }

        }
        return false;
    }
 
    IEnumerator permaBoardCheck()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            if(!isSwapping && !checkIfBoardIsMoving())
            {
                for (int i= 0; i < allBlobbList.Count; i++)
                {
                    allBlobbList[i].clearAllMatches();
                }
                //if (!checkForPossibleMatches())
                //{
                //    Debug.Log("no match");
                //    for (int j = 0; j < 1; j++)
                //    {
                //        for (int i = 0; i < allBlobbList.Count; i++)
                //        {
                //            allBlobbList[i].matchFound = true;
                //        }
                //    }

                //}
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void stopGame()
    {
        StopAllCoroutines();
        foreach (Blobb b in allBlobbList)
        {
            b.stopAll();
        }
    }

    bool checkForPossibleMatches()
    {
        togglePhysics(true);
        for(int i= 0; i < allBlobbList.Count; i++)
        {
            for(int j = 0; j<allBlobbList.Count;j++)
            {
                if (allBlobbList[i].checkNeighbours(allBlobbList[j])) 
                {

                    Blobb b1 = allBlobbList[i];
                    Blobb b2 = allBlobbList[j];

                    List<Blobb> tempNeighbours = new List<Blobb>(b1.neighbourList);

                    Vector3 b1TempPos = b1.transform.position;
                    Vector3 b2TempPos = b2.transform.position;
                    b1.transform.position = b2TempPos;
                    b1.transform.position = b1TempPos;

                    b1.neighbourList = b2.neighbourList;
                    b2.neighbourList = tempNeighbours;

                    if (b1.checkForExistingMatches())
                    {
                        b1.transform.position = b1TempPos;
                        b2.transform.position = b2TempPos;

                        b2.neighbourList = b1.neighbourList;
                        b1.neighbourList = tempNeighbours;
                        togglePhysics(false);
                        return true;
                    }
                    if (b2.checkForExistingMatches())
                    {
                        b1.transform.position = b1TempPos;
                        b2.transform.position = b2TempPos;

                        b2.neighbourList = b1.neighbourList;
                        b1.neighbourList = tempNeighbours;
                        togglePhysics(false);
                        return true;
                    }
                    b1.transform.position = b1TempPos;
                    b2.transform.position = b2TempPos;

                    b2.neighbourList = b1.neighbourList;
                    b1.neighbourList = tempNeighbours;
                    togglePhysics(false);
                }
            }
        }
        return false;
    }
}
