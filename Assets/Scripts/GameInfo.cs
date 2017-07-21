using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {
    public const int UPDATE_COUNTERS = 100;

    public Text redCubeCounter;
    public Text blueCubeCounter;
    public Text blackCubeCounter;
    public Text yellowCubeCounter;
    public Text outbreakCounter;
    public Text infectionRateCounter;
    public Text researchCenterCounter;

    private int numRedCubes;
    private int numBlueCubes;
    private int numBlackCubes;
    private int numYellowCubes;
    private int numOutbreaks;
    private int numEpidemics;
    private int numResearchCenters;

    private void Start()
    {
        numRedCubes = 24;
        numBlueCubes = 24;
        numBlackCubes = 24;
        numYellowCubes = 24;
        numOutbreaks = 0;
        numEpidemics = 0;
        numResearchCenters = 6;
    }

    private void Awake()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    private void OnEvent(byte eventCode, object content, int senderId)
    {
        switch (eventCode)
        {
            case UPDATE_COUNTERS:
                UpdateCounters((int[])content);
                break;
        }
    }

    public void UpdateCounters(int[] values)
    {
        numBlackCubes = values[0];
        numBlueCubes = values[1];
        numRedCubes = values[2];
        numYellowCubes = values[3];
        numResearchCenters = values[4];
        numOutbreaks = values[5];
        numEpidemics = values[6];

        blackCubeCounter.text = numBlackCubes.ToString();
        blueCubeCounter.text = numBlueCubes.ToString();
        redCubeCounter.text = numRedCubes.ToString();
        yellowCubeCounter.text = numYellowCubes.ToString();
        researchCenterCounter.text = numResearchCenters.ToString();
        outbreakCounter.text = numOutbreaks.ToString();

        if (numEpidemics > 4)
        {
            infectionRateCounter.text = "4";
        }
        else if (numEpidemics > 2)
        {
            infectionRateCounter.text = "3";
        }
        else
        {
            infectionRateCounter.text = "2";
        }
    }

    public void UpdateCubeCounter(CubeDraggable.CubeColor cubeColor, bool shouldDecrement)
    {
        switch (cubeColor)
        {
            case CubeDraggable.CubeColor.BLACK:
                numBlackCubes = shouldDecrement ? numBlackCubes - 1 : numBlackCubes + 1;
                blackCubeCounter.text = numBlackCubes.ToString();
                break;
            case CubeDraggable.CubeColor.BLUE:
                numBlueCubes = shouldDecrement ? numBlueCubes - 1 : numBlueCubes + 1;
                blueCubeCounter.text = numBlueCubes.ToString();
                break;
            case CubeDraggable.CubeColor.RED:
                numRedCubes = shouldDecrement ? numRedCubes - 1 : numRedCubes + 1;
                redCubeCounter.text = numRedCubes.ToString();
                break;
            case CubeDraggable.CubeColor.YELLOW:
                numYellowCubes = shouldDecrement ? numYellowCubes - 1 : numYellowCubes + 1;
                yellowCubeCounter.text = numYellowCubes.ToString();
                break;
            case CubeDraggable.CubeColor.RESEARCH:
                numResearchCenters = shouldDecrement ? numResearchCenters - 1 : numResearchCenters + 1;
                researchCenterCounter.text = numResearchCenters.ToString();
                break;
        }

        SendUpdateEvent();
    }

    public void UpdateInfectionRate()
    {
        numEpidemics++;

        if (numEpidemics > 4)
        {
            infectionRateCounter.text = "4";
        }
        else if (numEpidemics > 2)
        {
            infectionRateCounter.text = "3";
        }
        else
        {
            infectionRateCounter.text = "2";
        }

        SendUpdateEvent();
    }

    public void UpdateEpidemicCounter()
    {
        outbreakCounter.text = (++numOutbreaks).ToString();

        SendUpdateEvent();
    }

    private void SendUpdateEvent()
    {
        int[] values = new int[]
        {
            numBlackCubes,
            numBlueCubes,
            numRedCubes,
            numYellowCubes,
            numResearchCenters,
            numOutbreaks,
            numEpidemics
        };

        PhotonNetwork.RaiseEvent(UPDATE_COUNTERS, values, true, null);
    }
}
