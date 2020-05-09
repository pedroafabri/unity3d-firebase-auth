using Firebase;
using UnityEngine.Events;

[System.Serializable]
public class LoginErrorEvent : UnityEvent<FirebaseException>
{
    // Simply makes the "UnityEvent<T0>" serializable.
}