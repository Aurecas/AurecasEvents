using UnityEngine;
using System;
using Firebase.Analytics;
using System.Linq;

namespace AurecasLib.Analytics {
    public class AnalyticsManager : MonoBehaviour {
        bool analyticsDebugText;
        public static AnalyticsManager Instance;

        public Action OnUpdateUserProperties;
        public Func<Parameter[]> GetDefaultParameters;

        Action delayedAction;
        float lastUserPropertiesUpdatedTime;

        bool eventsDisabled;

        public void DisableEventSending() {
            eventsDisabled = true;
        }

        public void EnableEventSending() {
            eventsDisabled = false;
        }

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                if (Instance != this) {
                    Destroy(gameObject);
                    return;
                }
            }
            DontDestroyOnLoad(gameObject);
            analyticsDebugText = Debug.isDebugBuild;
        }

        public void UpdateUserProperties() {
            if (eventsDisabled) return;
            if (Time.time - lastUserPropertiesUpdatedTime > 5f) {
                OnUpdateUserProperties?.Invoke();
                lastUserPropertiesUpdatedTime = Time.time;
            }
        }

        private void Update() {
            if (delayedAction != null) {
                delayedAction.Invoke();
                delayedAction = null;
            }
        }


        public void SendEvent(string eventName, params Parameter[] parameters) {
            if (eventsDisabled) return;
            if (analyticsDebugText) {
                string t = "Event " + eventName;
                Debug.Log(t);
            }
            UpdateUserProperties();
            Parameter[] concat = parameters;
            
            if(GetDefaultParameters != null) {
                concat.Concat(GetDefaultParameters());
            }

            FirebaseAnalytics.LogEvent(eventName, concat.ToArray());
        }
    }
}