using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallFunction : MonoBehaviour
{
    public UnityEvent callAtStart;
    public UnityEvent callOnUpdate;
    public UnityEvent callOnFixedUpdate;
    
    void Start()
    {
        callAtStart.Invoke();
    }
    
    void Update()
    {
        callOnUpdate.Invoke();
    }

    private void FixedUpdate()
    {
        callOnFixedUpdate.Invoke();
    }
}
