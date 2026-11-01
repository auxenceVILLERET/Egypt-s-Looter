using System.Globalization;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class S_CursorManager : MonoBehaviour
{
    private const float MinSensitivity = 0f;
    private const float MaxSensitivity = 10f;

    [Range(0f, 10f)]
    [SerializeField] float _sensitivityX = 1f;
    [Range(0f, 10f)]
    [SerializeField] float _sensitivityY = 1f;

    [SerializeField] CinemachineInputAxisController _axisController;

    [Header("Settings UI")]
    [SerializeField] Slider _sensitivityXSlider;
    [SerializeField] Slider _sensitivityYSlider;
    [SerializeField] TMP_InputField _sensitivityXInput;
    [SerializeField] TMP_InputField _sensitivityYInput;

    private void Start()
    {
        if (_sensitivityXSlider != null)
        {
            _sensitivityXSlider.minValue = MinSensitivity;
            _sensitivityXSlider.maxValue = MaxSensitivity;
        }

        if (_sensitivityYSlider != null)
        {
            _sensitivityYSlider.minValue = MinSensitivity;
            _sensitivityYSlider.maxValue = MaxSensitivity;
        }

        SetSensitivityX(_sensitivityX);
        SetSensitivityY(_sensitivityY);
    }

    private void OnEnable()
    {
        if (_sensitivityXSlider != null)
            _sensitivityXSlider.onValueChanged.AddListener(SetSensitivityX);
        if (_sensitivityYSlider != null)
            _sensitivityYSlider.onValueChanged.AddListener(SetSensitivityY);

        if (_sensitivityXInput != null)
            _sensitivityXInput.onEndEdit.AddListener(OnInputXEndEdit);
        if (_sensitivityYInput != null)
            _sensitivityYInput.onEndEdit.AddListener(OnInputYEndEdit);
    }

    private void OnDisable()
    {
        if (_sensitivityXSlider != null)
            _sensitivityXSlider.onValueChanged.RemoveListener(SetSensitivityX);
        if (_sensitivityYSlider != null)
            _sensitivityYSlider.onValueChanged.RemoveListener(SetSensitivityY);

        if (_sensitivityXInput != null)
            _sensitivityXInput.onEndEdit.RemoveListener(OnInputXEndEdit);
        if (_sensitivityYInput != null)
            _sensitivityYInput.onEndEdit.RemoveListener(OnInputYEndEdit);
    }

    public void LockCursor(bool locked)
    {
        if (locked)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor(bool hide)
    {
        if (hide)
            Cursor.visible = false;
        else
            Cursor.visible = true;
    }

    #region Sensitivity
    public void SetSensitivityX(float value)
    {
        _sensitivityX = Mathf.Clamp(value, MinSensitivity, MaxSensitivity);

        if (_sensitivityXSlider != null)
            _sensitivityXSlider.SetValueWithoutNotify(_sensitivityX);
        if (_sensitivityXInput != null)
            _sensitivityXInput.SetTextWithoutNotify(FormatValue(_sensitivityX));

        UpdateSensitivity();
    }

    public void SetSensitivityY(float value)
    {
        _sensitivityY = Mathf.Clamp(value, MinSensitivity, MaxSensitivity);

        if (_sensitivityYSlider != null)
            _sensitivityYSlider.SetValueWithoutNotify(_sensitivityY);
        if (_sensitivityYInput != null)
            _sensitivityYInput.SetTextWithoutNotify(FormatValue(_sensitivityY));

        UpdateSensitivity();
    }

    private void OnInputXEndEdit(string text)
    {
        float value;
        if (TryParseValue(text, out value))
        {
            SetSensitivityX(value);
        }
        else
        {
            SetSensitivityX(_sensitivityX);
        }
    }

    private void OnInputYEndEdit(string text)
    {
        float value;
        if (TryParseValue(text, out value))
        {
            SetSensitivityY(value);
        }
        else
        {
            SetSensitivityY(_sensitivityY);
        }
    }

    private bool TryParseValue(string text, out float value)
    {
        string cleaned = text.Replace(',', '.');
        return float.TryParse(cleaned, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private string FormatValue(float value)
    {
        return value.ToString("0.0", CultureInfo.InvariantCulture);
    }

    public void UpdateSensitivity()
    {
        if (_axisController == null) return;

        foreach (var controller in _axisController.Controllers)
        {
            if (controller.Name == "Look Orbit X")
            {
                controller.Input.Gain = _sensitivityX;
            }
            if (controller.Name == "Look Orbit Y")
            {
                controller.Input.Gain = _sensitivityY;
            }
        }
    }
    #endregion
}