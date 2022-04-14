using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace IoTDashboard
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasLayer1;
        [SerializeField]
        public InputField addressInputField;
        [SerializeField]
        public InputField usrnameInputField;
        [SerializeField]
        public InputField pwdInputField;
        [SerializeField]
        public Button connButton;
        [SerializeField]
        public Text connStatus;
 
        [SerializeField]
        private CanvasGroup _canvasLayer2;
        [SerializeField]
        public Text temperatureStatusText;
        [SerializeField]
        public Text humidityStatusText;
        [SerializeField]
        public ToggleSwitch controlLedToggle;
        [SerializeField]
        public ToggleSwitch controlPumpToggle;


        [SerializeField]
        private GameObject ExitButton;

        private Tween twenFade;

        public void InitLoginInfo() {
            addressInputField.text = "mqttserver.tk";
            usrnameInputField.text = "bkiot";
            pwdInputField.text = "12345678";
        }

        void Start() {
            InitLoginInfo();
        }

        public void displayConnStatus(string status) {
            connStatus.text = status;
        }

        public void displayStatus(SensorData sensor) {
            temperatureStatusText.text = sensor.temperature + " °C";
            humidityStatusText.text = sensor.humidity + " %";
        }

        public ControlData GetControlLed() {
            ControlData controlLed = new ControlData();;
            controlLed.device = "LED";
            if (controlLedToggle.toggle.isOn) {
                controlLed.status = "OFF";
            }

            else {
                controlLed.status = "ON";
            }
            controlLedToggle.toggle.interactable = false;
            return controlLed;
        }

        public ControlData GetControlPump() {
            ControlData controlPump = new ControlData();
            controlPump.device = "PUMP";
            if (controlPumpToggle.toggle.isOn) {
                controlPump.status = "OFF";
            }

            else {
                controlPump.status = "ON";
            }
            controlPumpToggle.toggle.interactable = false;
            return controlPump;
        }

        public void UpdateLedToggle(ControlData controlData) {
            controlLedToggle.toggle.interactable = true;
            if (controlData.status == "ON")
                controlLedToggle.toggle.isOn = true;
            else
                controlLedToggle.toggle.isOn = false;
        }

        public void UpdatePumpToggle(ControlData controlData) {
            controlPumpToggle.toggle.interactable = true;
            if (controlData.status == "ON")
                controlPumpToggle.toggle.isOn = true;
            else
                controlPumpToggle.toggle.isOn = false;
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish) {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration) {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration) {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }

        IEnumerator _IESwitchLayer() {
            if (_canvasLayer1.interactable == true)
            {
                FadeOut(_canvasLayer1, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer2, 0.25f);
            }
            else
            {
                FadeOut(_canvasLayer2, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
            }
        }

        public void SwitchLayer() {
            StartCoroutine(_IESwitchLayer());
        }
    }
}