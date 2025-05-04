using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class StateHandlersManager : MonoBehaviour
{
    public static StateHandlersManager instance;


    [SerializeField] List<StateHandler> stateHandlers;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }


    [Button(size: ButtonSizes.Large)]
    public void UpdateList()
    {
        stateHandlers = FindObjectsOfType<StateHandler>().ToList();
    }


    public static void SetActiveFreeze(bool active)
    {
        instance.stateHandlers.ForEach(sh => sh.SetFreeze(active: active).Forget());
    }


    public static void AddStateHandler(StateHandler handler)
    {
        if (!instance.stateHandlers.Contains(handler))
            return;

        instance.stateHandlers.Add(handler);
    }

    public static void RemoveStateHandler(StateHandler handler) {
        if (!instance.stateHandlers.Contains(handler))
            return;

        instance.stateHandlers.Remove(handler);
    }
}
