using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {

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
    }

    public void UpdateEpidemicCounter()
    {
        outbreakCounter.text = (++numOutbreaks).ToString();
    }
}
