using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //[SerializeField] private Text timeText, bestTimeText;
    [SerializeField] private TextMeshProUGUI timeText, bestTimeText;
    [SerializeField] private List<GameObject> prefabs;

    private float miliseconds, seconds, minutes, lastTime, bestTime;
    private int[] sequence = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 };
    private List<GameObject> tiles;
    private int[] shuffledSeq;
    [SerializeField] private AudioSource audioKnock;
    [SerializeField] private AudioSource audioVictory;
    private bool isMuted;

    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.HasKey("SaveTime"))
        {
            bestTime = PlayerPrefs.GetFloat("SaveTime");
        }
    }
    private void Start()
    {
        SetBestTime();
        do
        {
            shuffledSeq = Shuffle(sequence);
        } while (!IsSolvable(shuffledSeq));

        tiles = CreateParts(shuffledSeq);
    }
    private void Update()
    {

        CheckHit();

    }

    private void FixedUpdate()
    {
        UpdateTime();
    }
    List<GameObject> CreateParts(int[] array)
    {
        List<GameObject> temp = new List<GameObject>();
        int index = 0;

        for (int z = 0; z >= -6; z -= 2)
        {
            for (int x = 0; x <= 6; x += 2)
            {
                Vector3 position = new Vector3(x, 0, z);

                var tile = Instantiate(prefabs[array[index]], position, Quaternion.identity);
                tile.name = array[index].ToString();
                tile.GetComponent<Tile>().index = index;
                temp.Add(tile);
                index++;
            }
        }
        return temp;
    }

    int[] Shuffle(int[] array)
    {
        return array.OrderBy(a => Guid.NewGuid()).ToArray();
    }

    bool IsSolvable(int[] array)
    {
        int k = 0; // number of blocks with lower numbers
        int e = 0; // empty cell row number

        for (int i = 0; i < 16; i++)
        {
            if (array[i] == 0)
            {
                e = i / 4 + 1;
                continue;
            }
            for (int j = i + 1; j < 16; j++)
            {
                if ((array[i] > array[j]) && (array[j] != 0))
                {
                    k++;
                }
            }
        }
        int sum = k + e;
        return sum % 2 == 0;
    }

    bool IsSolved()
    {
        return Enumerable.SequenceEqual(sequence, shuffledSeq);
    }

    void MoveTile(int index)
    {
        int tileRow = index / 4;

        if ((index - 4) >= 0 && shuffledSeq[index - 4] == 0)
        {
            var zeroTile = tiles[index - 4];
            var currTile = tiles[index];

            zeroTile.transform.Translate(-Vector3.forward * 2);
            currTile.transform.Translate(Vector3.forward * 2);
            shuffledSeq[index - 4] = shuffledSeq[index];
            shuffledSeq[index] = 0;
            SwapTiles(index, -4);

        }
        else if ((index - 1) >= 0 && shuffledSeq[index - 1] == 0 && (tileRow == (index - 1) / 4))
        {
            var zeroTile = tiles[index - 1];
            var currTile = tiles[index];

            currTile.transform.Translate(-Vector3.right * 2);
            zeroTile.transform.Translate(Vector3.right * 2);
            shuffledSeq[index - 1] = shuffledSeq[index];
            shuffledSeq[index] = 0;
            SwapTiles(index, -1);
        }
        else if ((index + 1) < 16 && shuffledSeq[index + 1] == 0 && (tileRow == (index + 1) / 4))
        {
            var zeroTile = tiles[index + 1];
            var currTile = tiles[index];

            currTile.transform.Translate(Vector3.right * 2);
            zeroTile.transform.Translate(-Vector3.right * 2);
            shuffledSeq[index + 1] = shuffledSeq[index];
            shuffledSeq[index] = 0;
            SwapTiles(index, 1);
        }
        else if ((index + 4) < 16 && shuffledSeq[index + 4] == 0)
        {
            var zeroTile = tiles[index + 4];
            var currTile = tiles[index];

            currTile.transform.Translate(-Vector3.forward * 2);
            zeroTile.transform.Translate(Vector3.forward * 2);
            shuffledSeq[index + 4] = shuffledSeq[index];
            shuffledSeq[index] = 0;
            SwapTiles(index, 4);
        }
    }

    void CheckHit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(transform.position, transform.forward * 100f, Color.green);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Tile"))
            {
                audioKnock.Play();
                MoveTile(hit.collider.gameObject.GetComponent<Tile>().index);
                if (IsSolved())
                {
                    CheckBestTime();
                    StartCoroutine(RestartGame(0.5f));
                }
            }
        }

    }

    void SwapTiles(int index, int offset)
    {
        var temp = tiles[index];
        tiles[index] = tiles[index + offset];
        tiles[index].GetComponent<Tile>().index = index;
        tiles[index + offset] = temp;
        tiles[index + offset].GetComponent<Tile>().index = index + offset;
    }
    void CheckDebug(IEnumerable arr)
    {
        string str = null;
        foreach (var item in arr)
        {
            str = str + " " + item;
        }
        Debug.Log(str);
    }

    private void UpdateTime()
    {
        miliseconds += 0.02f;

        if (miliseconds >= 1)
        {
            seconds++;
            miliseconds = 0;
        }
        if (seconds >= 60)
        {
            minutes++;
            seconds = 0;
        }

        if (minutes > 0)
        {
            if (seconds >= 10)
            {
                timeText.text = "Time: " + minutes.ToString() + ":" + seconds.ToString();
            }
            else
            {
                timeText.text = "Time: " + minutes.ToString() + ":0" + seconds.ToString();
            }
        }
        else
        {

            timeText.text = "Time: " + seconds.ToString();
        }
    }

    private float GetSeconds(float minutes, float seconds)
    {
        return minutes * 60 + seconds;
    }

    private void CheckBestTime()
    {
        lastTime = GetSeconds(minutes, seconds);
        if (bestTime < 1f)
        {
            bestTime = lastTime;
            PlayerPrefs.SetFloat("SaveTime", bestTime);
            SetBestTime();
        }
        if (lastTime < bestTime)
        {
            bestTime = lastTime;
            PlayerPrefs.SetFloat("SaveTime", bestTime);
            SetBestTime();
        }
    }

    private void SetBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("SaveTime");
        float minutes = Mathf.Floor(bestTime / 60);
        float seconds = bestTime - minutes * 60;
        if (minutes > 0)
        {
            if (seconds >= 10)
            {
                bestTimeText.text = "Best time: " + minutes.ToString() + ":" + seconds.ToString();
            }
            else
            {
                bestTimeText.text = "Best time: " + minutes.ToString() + ":0" + seconds.ToString();
            }
        }
        else
        {
            bestTimeText.text = "Best time: " + seconds.ToString();
        }
    }

    private IEnumerator RestartGame(float time)
    {
        yield return new WaitForSeconds(time);
        audioVictory.Play();
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ResetBestTime()
    {
        PlayerPrefs.DeleteKey("SaveTime");
    }




}
