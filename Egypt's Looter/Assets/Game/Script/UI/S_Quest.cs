using TMPro;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class S_Quest : MonoBehaviour
{
    [BoxGroup("Quest 1")]
    [SerializeField] private TMP_Text _quest1;

    [BoxGroup("Quest 2")]
    [SerializeField] private TMP_Text _quest2;
    [BoxGroup("Quest 2")]
    [SerializeField] private Image _quest2Image;

    [BoxGroup("References")]
    [SerializeField] private Image _background;

    public void SetFirstQuestActive(bool active)
    {
        _quest1.enabled = active;
    }

    public void SetSecondQuestActive(bool active)
    {
        _quest2.enabled = active;
        _quest2Image.enabled = active;
    }

    public void SetActiveBackground(bool active)
    {
        _background.enabled = active;
    }
}
