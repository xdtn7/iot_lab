using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace IoTDashboard
{
    public class SensorData
    {
        public string temperature { get; set; }
        public string humidity { get; set; }
    }

    public class ControlData
    {
        public string device { get; set; }
        public string status { get; set; }

    }

    public class MQTTManager : M2MqttUnityClient
    {
        private int serverPort = 1883;
        public List<string> topics = new List<string>();
        private List<string> eventMessages = new List<string>();
        public UIManager UIManager;

        [SerializeField]
        public SensorData sensorData;
        [SerializeField]
        public ControlData controlLed;
        [SerializeField]
        public ControlData controlPump;

        public void PublishControlLed() {
            controlLed = UIManager.GetControlLed();
            string pkt = JsonConvert.SerializeObject(controlLed);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(pkt), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public void PublishControlPump() {
            controlPump = UIManager.GetControlPump();
            string pkt = JsonConvert.SerializeObject(controlPump);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(pkt), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

	    public void PreConnect() {
            this.brokerAddress = UIManager.addressInputField.text;
            this.mqttUserName = UIManager.usrnameInputField.text;
            this.mqttPassword = UIManager.pwdInputField.text;
            this.brokerPort = serverPort;

            this.Connect();
        }
       
        public void SetEncrypted(bool isEncrypted) {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting() {
            base.OnConnecting();
            UIManager.displayConnStatus("Connecting to " + this.brokerAddress);
        }

        protected override void OnConnected() {
            base.OnConnected();
            UIManager.displayConnStatus("Server connected");
            SubscribeTopics();
            UIManager.SwitchLayer();
        }

        protected override void SubscribeTopics() {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        protected override void UnsubscribeTopics() {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage) {
            UIManager.displayConnStatus("Connection failed");
        }

        protected override void OnDisconnected() {

        }

        protected override void OnConnectionLost() {
            UIManager.displayConnStatus("Connection lost");
        }

        protected override void Start() {
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message) {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            if (topic == topics[0])
                ProcessMessageStatus(msg);
            else if (topic == topics[1])
                ProcessMessageLed(msg);
            else if (topic == topics[2])
                ProcessMessagePump(msg);
        }

        private void ProcessMessageStatus(string msg) {
            Debug.Log(msg);
            sensorData = JsonConvert.DeserializeObject<SensorData>(msg);
            UIManager.displayStatus(sensorData);
        }

        private void ProcessMessageLed(string msg) {
            controlLed = JsonConvert.DeserializeObject<ControlData>(msg);
            UIManager.UpdateLedToggle(controlLed);
        }

        private void ProcessMessagePump(string msg) {
            controlPump = JsonConvert.DeserializeObject<ControlData>(msg);
            UIManager.UpdatePumpToggle(controlPump);
        }

        protected override void Update() {
            base.Update();
        }

        private void OnDestroy() {
            Disconnect();
        }

        public void ServerDisconnect() {
            Disconnect();
            UIManager.displayConnStatus("");
            UIManager.SwitchLayer();
        }
    }
}
