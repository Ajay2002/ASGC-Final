using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;


public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("Images")]
    public Texture2D warningImage;
    public Texture2D errorImage;
    public Texture2D costImage;
    public Texture2D messageImage;

    [Header("UI Elements")]
    public Transform panel;
    public Image image;
    public TextMeshProUGUI text;
    public Button button;

    [Header("positions")]
    public Vector3 openPosition;
    public Vector3 closePosition;
    Vector3 initialPosition;

    [Header("Queue")]
    public Queue<Notification> notifications = new Queue<Notification>();
    // Start is called before the first frame update
    void Awake()
    {   
        initialPosition = panel.position;
        panel.gameObject.SetActive(false);
        currentState=ProcessState.Closing;
        if (Instance == null) Instance = this;
    }

    public void Test() {
        print ("Button Pressed");
        currentState=ProcessState.Closing;
    }

    public enum ProcessState {Finished, Opening, Closing, Waiting};
    public ProcessState currentState = ProcessState.Finished;

    public Notification currentNotification;

    float t=0f;
    void Update() {
 
        if (currentState == ProcessState.Finished) {
            if (notifications.Count > 0) {
                panel.gameObject.SetActive(true);
                currentNotification = notifications.Dequeue();
                LoadNotification(currentNotification);
                t = currentNotification.duration;
                currentState = ProcessState.Opening;
            }
        }

        if (currentState == ProcessState.Opening) {

            float d = Vector3.Distance(panel.position, initialPosition+openPosition);

            if (d >= 0.1f) {
                panel.position = Vector3.Lerp(panel.position, initialPosition+openPosition, 0.2f);
            }
            else {

                //Begin the wait
                currentState = ProcessState.Waiting;
                
            }

        }

        if (currentState == ProcessState.Waiting) {

            if (t >= 0) {
                t -= Time.unscaledDeltaTime;
            }
            else {
                currentState=ProcessState.Closing;
            }

        }

        if (currentState == ProcessState.Closing) {

            float d = Vector3.Distance(panel.position, initialPosition+closePosition);

            if (d >= 0.1f) {
                panel.position = Vector3.Lerp(panel.position, initialPosition+closePosition, 0.2f);
            }
            else {

                //Begin the wait
                currentState = ProcessState.Finished;
                
            }

        }

    }

    private void LoadNotification (Notification load) {
        
        switch (load.type) {

            case NotificationType.Cost:
                image.color = Color.green;
                break;
            
            case NotificationType.Error:
                image.color = Color.red;
                break;

            case NotificationType.Warning:
                image.color = Color.yellow;
                break;

            case NotificationType.Message:
                image.color = Color.grey;
                break;

        }

        text.text = load.text;

        if (load.containsAction) {
            button.gameObject.SetActive(true);
            button.onClick.AddListener(load.returnMethod);
        }
        else {
            button.gameObject.SetActive(false);
        }
    }

    public void CreateNotification (NotificationType type, string text, float duration, bool highPriority, UnityAction returnMethod) {
        
        Notification n = new Notification();
        n.type = type;
        n.text = text;
        n.duration = duration;
        n.highPriority = highPriority;
        n.containsAction = true;
        n.returnMethod = returnMethod;
        notifications.Enqueue(n);
    }

    public void CreateNotification (NotificationType type, string text, bool highPriority, float duration) {
        Notification n = new Notification();
        n.type = type;
        n.text = text;
        n.duration = duration;
        n.highPriority = highPriority;
        n.containsAction = false;
        notifications.Enqueue(n);
    }

    public void CreateNotification (Notification notification) {

        notifications.Enqueue(notification);

    }

}

public enum NotificationType {Error, Message, Warning, Cost};

[Serializable]
public struct Notification {
    public NotificationType type;
    public string text;
    public float duration;
    public bool highPriority;
    public bool containsAction;
    public UnityAction returnMethod;
}