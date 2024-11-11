using ProjectTank.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI teamAScore;
    [SerializeField] private TextMeshProUGUI teamBScore;

    private int teamA;
    private int teamB;
    [Space]
    [SerializeField] private TextMeshProUGUI teamWin;
    [SerializeField] private GameObject UImodal;
    [SerializeField] private Button mainMenuButton;

    private void OnEnable()
    {
        mainMenuButton.onClick.AddListener(() => Loader.LoadNetwork(Loader.Scene.MainMenu));
    }

    private void Start()
    {
        teamAScore.text = "TEAM A: " + teamA.ToString();
        teamBScore.text = "TEAM B: " + teamB.ToString();
    }

    public void UpdateTeamAScore()
    {
        teamA += 1;
        teamAScore.text = "TEAM A: " + teamA.ToString();


        if (teamA >= 10)
        {
            teamWin.text = "TEAM A WIN";
            UImodal.SetActive(true);
        }
    }

    public void UpdateTeamBScore()
    {
        teamB += 1;
        teamBScore.text = "TEAM B: " + teamB.ToString();

        if (teamB >= 10)
        {
            teamWin.text = "TEAM B WIN";
            UImodal.SetActive(true);
        }
    }





}
