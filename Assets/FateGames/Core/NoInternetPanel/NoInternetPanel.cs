
using UnityEngine;
using UnityEngine.Events;

public class NoInternetPanel : MonoBehaviour
{
    [SerializeField] UnityEvent onAppear, onHide;
    [SerializeField] private UIElement uiElement;
    bool hidden = true;
    private void Start()
    {
        InvokeRepeating(nameof(CheckInternet), 10, 10);
    }

    public void Retry()
    {
        if (hidden) return;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            uiElement.Hide();
            hidden = true;
            onHide.Invoke();
        }
    }

    public void CheckInternet()
    {
        if (!hidden || AdManager.Disabled) return;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            uiElement.Show();
            hidden = false;
            onAppear.Invoke();
        }
    }
}
