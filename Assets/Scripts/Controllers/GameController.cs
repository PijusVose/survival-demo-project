using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Private fields
    
    private List<IControllerPlugin> controllerPlugins;

    // Events
    
    public event Action OnAwake;
    public event Action OnStart; 

    // GameController
    
    private void Awake()
    {
        GetPlugins();
        
        OnAwake?.Invoke();
    }

    private void Start()
    {
        InitPlugins();
        
        OnStart?.Invoke();
    }

    private void GetPlugins()
    {
        controllerPlugins = new List<IControllerPlugin>();
        
        foreach (var childComponent in gameObject.GetComponentsInChildren<MonoBehaviour>())
        {
            if (childComponent is IControllerPlugin plugin)
            {
                controllerPlugins.Add(plugin);
            }
        }
    }

    private void InitPlugins()
    {
        foreach (var plugin in controllerPlugins)         
        {
            plugin.Init();
        }
    }
}
