using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using DG.Tweening;

public class S_BloodScreen : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SerializeField] private S_GeneratorSRP _generator;
    [BoxGroup("Dependencies")]
    [SerializeField] private Image _bloodScreenImage;

    private S_Health _health;
    private float _bloodScreenDuration = 0.5f;
    private float time = 0f;

    void OnEnable()
    {
        _generator.OnPlayerSpawned += Set;
        _bloodScreenImage.enabled = false;
    }

    void Set()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { return; }

        _health = player.GetComponentInChildren<S_Health>();
        if (_health == null) _health = player.GetComponentInParent<S_Health>();
        if (_health == null) { return; }

        _health.OnHealthChanged += UpdateBloodScreen;
    }

    void OnDestroy()
    {
        _health.OnHealthChanged -= UpdateBloodScreen;
    }

    void UpdateBloodScreen(int currentHp)
    {
        _bloodScreenImage.enabled = true;
        time = 0f;
    }

    void FadeOut()
    {
        _bloodScreenImage.DOFade(0f, _bloodScreenDuration).OnComplete(() =>
        {
            _bloodScreenImage.enabled = false;
            _bloodScreenImage.color = new Color(_bloodScreenImage.color.r, _bloodScreenImage.color.g, _bloodScreenImage.color.b, 1f);
        });
    }

    void Update()
    {
        if (_bloodScreenImage.enabled)
        {
            time += Time.deltaTime;
            if (time >= _bloodScreenDuration)
            {
                FadeOut();
                time = 0f;
            }
        }
    }
}
