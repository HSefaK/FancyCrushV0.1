using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Blobb : MonoBehaviour
{
    public List<Blobb> neighbourList = new List<Blobb>();

    public GameObject selector;
    public bool isSelected = true;
    public int blobbId;

    public bool matchFound;

    public int score = 1;

    Vector3[] checkDirs = new Vector3[4] { Vector3.left, Vector3.right, Vector3.up, Vector3.down };

    private void Start()
    {
        ToggleSelector();
        StartCoroutine(destroyFunction());
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            getAllNeighbours();
            if (!Board.instance.isSwapping)
            {
                ToggleSelector();
                Board.instance.swapBlobbs(this);
            }
        }
        //if (Board.instance.checkIfBoardIsMoving())
        //{
        //    return;
        //}
        
        
    }

    public void ToggleSelector()
    {
        isSelected = !isSelected;
        selector.SetActive(isSelected);
    }

    void getAllNeighbours()
    {
        neighbourList.Clear();
        for (int i = 0; i < checkDirs.Length; i++)
        {
            neighbourList.Add(getNeighbour(checkDirs[i]));
        }
    }

    public Blobb getNeighbour(Vector3 checkDirs)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,checkDirs,out hit))
        {
            if(hit.collider != null)
            {
                return hit.collider.GetComponent<Blobb>();
            }
        }
        return null;
    }

    public Blobb getNeighbour(Vector3 checkDirs,int id)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, checkDirs, out hit,0.8f))
        {
            Blobb b = hit.collider.GetComponent<Blobb>();
            if (b != null && b.blobbId == id)
            {
                return b;
            }
        }
        return null;
    }

    public bool checkIfNeighbour(Blobb blobb)
    {
        if (neighbourList.Contains(blobb))
        {
            return true;

        }
        return false;
    }

    List<Blobb> findMatch (Vector3 checkDir)
    {
        List<Blobb> matchList = new List<Blobb>();
        List<Blobb> checkList = new List<Blobb>();

        checkList.Add(this);

        for(int i = 0; i < checkList.Count; i++)
        {
            Blobb b = checkList[i].getNeighbour(checkDir, blobbId);
            if (b != null && blobbId == b.blobbId)
            {
                checkList.Add(b);
            }
        }
        matchList.AddRange(checkList);
        return matchList;
    }

    void clearMatch(Vector3[] dirs)
    {
        List<Blobb> matchingBlobbs = new List<Blobb>();
        List<Blobb> sortedList = new List<Blobb>();

        for (int i = 0; i < dirs.Length; i++)
        {
            matchingBlobbs.AddRange(findMatch(dirs[i]));
        }

        for (int i = 0; i < matchingBlobbs.Count; i++)
        {
            if (!sortedList.Contains(matchingBlobbs[i]))
            {
                sortedList.Add(matchingBlobbs[i]);

            }
        }

        if (sortedList.Count >= 3)
        {
            for (int i  = 0; i < sortedList.Count; i++)
            {
                sortedList[i].matchFound = true;
            }
        }
    }

    public void clearAllMatches()
    {
        clearMatch(new Vector3[2] { Vector3.left, Vector3.right });
        clearMatch(new Vector3[2] { Vector3.up, Vector3.down });

        Board.instance.reportTurnDone();
    }

    IEnumerator destroyFunction()
    {
        while (true)
        {
            if (matchFound)
            {
                GameManager.instance.updateScore(score);

                Board.instance.createNewBlobb(this, transform.position);
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.25f);

        }
    }

    public void stopAll()
    {
        StopAllCoroutines();
    }
    public bool checkNeighbours(Blobb b)
    {
        getAllNeighbours();
        if (neighbourList.Contains(b))
        {
            return true;
        }
        return false;
    }

    public bool checkForExistingMatches()
    {
        if(checkMatch(new Vector3[2] { Vector3.left, Vector3.right }) || checkMatch(new Vector3[2] { Vector3.up, Vector3.down }))
        {
            return true;
        }
        return false;
    }

    bool checkMatch(Vector3[] dirs)
    {
        List<Blobb> matchingBlobbs = new List<Blobb>();
        List<Blobb> sortedList = new List<Blobb>();

        for (int i = 0; i < dirs.Length; i++)
        {
            matchingBlobbs.AddRange(findMatch(dirs[i]));
        }

        for (int i = 0; i < matchingBlobbs.Count; i++)
        {
            if (!sortedList.Contains(matchingBlobbs[i]))
            {
                sortedList.Add(matchingBlobbs[i]);

            }
        }

        if (sortedList.Count >= 3)
        {
            return true;
            //for (int i = 0; i < sortedList.Count; i++)
            //{
            //    sortedList[i].matchFound = true;
            //}
        }
        return false;
    }
}
