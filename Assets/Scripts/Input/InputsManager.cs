using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsManager : MonoBehaviour
{
    #region Singleton
    private static InputsManager instance;
    public static InputsManager Instance { get => instance; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        inputActions = new PlayerInputs();
        inputActions.Enable();
    }
    #endregion

    public enum StateAction
    {
        AddAction,
        RemoveAction,
    }

    private PlayerInputs inputActions;

    #region Main
    public void DesactivateInputs()
    {
        inputActions.Disable();
    }
    public void ActivateInputs()
    {
        inputActions.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }
    #endregion

    #region Movement
    public Vector2 Movement()
    {
        return inputActions.Deplacement.Movement.ReadValue<Vector2>();
    }
    #endregion

    #region Menuing
    public void Map(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Menuing.Map.performed += function;
        }
        else
        {
            inputActions.Menuing.Map.performed -= function;
        }
    }

    public void PassiveItem(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Menuing.PassiveItem.performed += function;
        }
        else
        {
            inputActions.Menuing.PassiveItem.performed -= function;
        }
    }
    #endregion

    #region Action
    public void UseItem(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Action.UseItem.performed += function;
        }
        else
        {
            inputActions.Action.UseItem.performed -= function;
        }
    }
    public void Fire(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Action.Fire.performed += function;
        }
        else
        {
            inputActions.Action.Fire.performed -= function;
        }
    }

    public void RecupItem(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Action.RecupItem.performed += function;
        }
        else
        {
            inputActions.Action.RecupItem.performed -= function;
        }
    }
    #endregion

    #region Debug
    public void TakeDamage(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Debug.TakeDamage.performed += function;
        }
        else
        {
            inputActions.Debug.TakeDamage.performed -= function;
        }
    }
    public void UpdateHeartHUD(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Debug.UpdateHeartHUD.performed += function;
        }
        else
        {
            inputActions.Debug.UpdateHeartHUD.performed -= function;
        }
    }
    public void Heal(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Debug.Heal.performed += function;
        }
        else
        {
            inputActions.Debug.Heal.performed -= function;
        }
    }
    public void AddHeart(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Debug.AddHeart.performed += function;
        }
        else
        {
            inputActions.Debug.AddHeart.performed -= function;
        }
    }
    public void RemoveHeart(Action<InputAction.CallbackContext> function, StateAction stateAction)
    {
        if (stateAction == StateAction.AddAction)
        {
            inputActions.Debug.RemoveHeart.performed += function;
        }
        else
        {
            inputActions.Debug.RemoveHeart.performed -= function;
        }

    }
    #endregion
}