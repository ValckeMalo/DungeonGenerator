using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour, IDamageable, IHealable
{
    private enum State
    {
        Idle,
        Run,
        Death,
    }

    [Header("UI")]
    [SerializeField] private bool isMapOpen = false;
    [SerializeField] private bool isPassiveItemOpen = false;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rigidBody2D;

    [Header("Collectible")]
    [SerializeField] private int gold;
    [SerializeField] private int bomb;
    [SerializeField] private int key;

    [Header("Stats")]
    [SerializeField] private State state = State.Idle;
    [SerializeField] private List<Heart> life;
    [SerializeField] private float speed;

    [Header("Item")]
    [SerializeField] private IRecuperable recuperableItem = null;
    [SerializeField] private EItem.ItemType typeItem;

    [Header("Active Item")]
    [SerializeField] private Transform containerActiveItem;
    [SerializeField] private ActiveItem activeItem = null;

    [Header("Passive Item")]
    [SerializeField] private List<SOPassiveItem> allPassiveItem;

    private void Start()
    {
        LevelManager.Instance.onTeleportPlayer += SetCoordinate;
        InitInputButton();
    }

    private void SetCoordinate(Vector2 newPos)
    {
        transform.parent.position = newPos;
    }

    #region Input Action
    private void InitInputButton()
    {
        //menuing
        InputsManager.Instance.Map(MapAction, InputsManager.StateAction.AddAction);
        InputsManager.Instance.PassiveItem(PassiveItem, InputsManager.StateAction.AddAction);

        //action
        InputsManager.Instance.Fire(Fire, InputsManager.StateAction.AddAction);
        InputsManager.Instance.UseItem(UseItem, InputsManager.StateAction.AddAction);
        InputsManager.Instance.RecupItem(RecupItem, InputsManager.StateAction.AddAction);

        //debug
        InputsManager.Instance.TakeDamage(CallTakeDamage, InputsManager.StateAction.AddAction);
        InputsManager.Instance.UpdateHeartHUD(CallOnLifeChangedHUD, InputsManager.StateAction.AddAction);
        InputsManager.Instance.Heal(CallHeal, InputsManager.StateAction.AddAction);
        InputsManager.Instance.AddHeart(CallAddHeart, InputsManager.StateAction.AddAction);
        InputsManager.Instance.RemoveHeart(CallRemoveHeart, InputsManager.StateAction.AddAction);
    }

    private void MapAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isMapOpen)
            {
                MapGenerator.Instance.OpenMap();
                isMapOpen = true;
            }
            else
            {
                MapGenerator.Instance.CloseMap();
                isMapOpen = false;
            }
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }

    private void UseItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (activeItem != null)
            {
                if (activeItem.CanActiveEffect())
                {
                    LevelManager.Instance.onActiveItemChanged?.Invoke(activeItem);
                }
            }
        }
    }

    private void RecupItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (recuperableItem != null)
            {
                if (typeItem == EItem.ItemType.ActiveItem)
                {
                    ActiveItemOnGround itemOnGround = recuperableItem as ActiveItemOnGround;
                    itemOnGround.ChangeParentActiveItem(containerActiveItem);
                    itemOnGround.Recuperate();

                    if (activeItem != null)
                    {
                        Destroy(containerActiveItem.GetChild(0).gameObject);
                    }

                    activeItem = containerActiveItem.GetChild(0).GetComponent<ActiveItem>();

                    LevelManager.Instance.onActiveItemChanged?.Invoke(activeItem);
                }
                else if (typeItem == EItem.ItemType.PassiveItem)
                {
                    PassiveItemOnGround itemOnGround = recuperableItem as PassiveItemOnGround;
                    SOPassiveItem newPassiveItem = itemOnGround.PassiveItem;
                    allPassiveItem.Add(newPassiveItem);
                    itemOnGround.Recuperate();

                    LevelManager.Instance.onPassiveItemChanged?.Invoke(newPassiveItem);
                }
                recuperableItem = null;
            }
        }
    }

    private void PassiveItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isPassiveItemOpen)
            {
                PassiveItemHud.Instance.HidePassiveitem();
                isPassiveItemOpen = false;
            }
            else
            {
                PassiveItemHud.Instance.ShowPassiveItem();
                isPassiveItemOpen = true;
            }
        }
    }
    #endregion

    #region State
    private void FixedUpdate()
    {
        if (state != State.Death)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 axis = InputsManager.Instance.Movement();
        rigidBody2D.velocity = axis * speed;

        if (axis != Vector2.zero)
        {
            state = State.Run;
        }
        else
        {
            state = State.Idle;
        }
    }
    #endregion

    #region IDagameable
    public void TakeDamage(int nbDamage, Status.EStatus statusApplied = Status.EStatus.None)
    {
        InflictDamage(nbDamage);
        InflictStatus(statusApplied);
        OnDeath();
    }

    private void InflictDamage(int nbDamage)
    {
        for (int i = life.Count - 1; i >= 0; i--)
        {
            Heart.HeartState currentState = life[i].State;
            if (currentState != Heart.HeartState.Empty)
            {
                if ((int)currentState < nbDamage)//if the current heart have less life than the number damage to inflict
                {
                    nbDamage -= (int)currentState;
                    life[i].State = Heart.HeartState.Empty;
                    continue;
                }
                else
                {
                    life[i].State -= nbDamage;
                    break;
                }
            }
        }

        LevelManager.Instance.onLifeChanged?.Invoke(life);
    }

    private void InflictStatus(Status.EStatus statusApplied)
    {
        if (statusApplied != Status.EStatus.None)
        {

        }
    }

    private void OnDeath()
    {
        if (life.Where(heart => heart.State != Heart.HeartState.Empty).Select(heart => heart).ToList().Count == 0) //no more heart
        {
            state = State.Death;
            LevelManager.Instance.onPlayerDeath?.Invoke();
        }
    }
    #endregion

    #region IHealable
    public bool HealHeart(int nbHeal, Heart.HeartType type)
    {
        List<Heart> possibleHeart = life.Where(heart => heart.Type == type && heart.State != Heart.HeartState.Full).Select(heart => heart).ToList();
        for (int i = 0; i < possibleHeart.Count; i++)
        {
            if ((int)possibleHeart[i].State + nbHeal <= 2)
            {
                possibleHeart[i].State += nbHeal;
                LevelManager.Instance.onLifeChanged?.Invoke(life);
                return true;
            }
        }

        return false;
    }

    public void AddHeart(Heart.HeartType type)
    {
        if (life.Count == 20)//max heart
        {
            life.RemoveAt(19);
        }
        life.Add(new Heart(type, Heart.HeartState.Full));

        LevelManager.Instance.onLifeChanged?.Invoke(life);
    }

    public void RemoveHeart(Heart.HeartType type)
    {
        life.Remove(life.Last(heart => heart.Type == type));

        if (life.Count <= 0)
        {
            state = State.Death;
            LevelManager.Instance.onPlayerDeath?.Invoke();
        }
        LevelManager.Instance.onLifeChanged?.Invoke(life);
    }
    #endregion

    #region IRecuperable
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IRecuperable item = collision.GetComponent<IRecuperable>();
        if (item != null)
        {
            recuperableItem = item;
            RecuperateItem();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (typeItem == EItem.ItemType.ActiveItem || typeItem == EItem.ItemType.PassiveItem)
        {
            if (recuperableItem != null)
            {
                (recuperableItem as IDisplayable).DesactiveDescription();
                recuperableItem = null;
            }
        }
    }

    private void RecuperateItem()
    {
        typeItem = recuperableItem.GetTypeObject();
        if (typeItem != EItem.ItemType.ActiveItem && typeItem != EItem.ItemType.PassiveItem)
        {
            AddColectible(typeItem);
            recuperableItem.Recuperate();
            recuperableItem = null;
        }
        else
        {
            ShowDescriptionItem();
        }
    }

    private void AddColectible(EItem.ItemType typeItem)
    {
        switch (typeItem)
        {
            case EItem.ItemType.Gold:
                gold++;
                break;
            case EItem.ItemType.Bomb:
                bomb++;
                break;
            case EItem.ItemType.Key:
                key++;
                break;

            default:
                Debug.LogError("don't recognize collectible");
                break;
        }

        LevelManager.Instance.onCollectibleChanged?.Invoke(gold, bomb, key);
    }

    private void ShowDescriptionItem()
    {
        (recuperableItem as IDisplayable).ActiveDescription();
    }
    #endregion

    #region Debug
    private void CallTakeDamage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TakeDamage(1);
        }
    }
    private void CallOnLifeChangedHUD(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LevelManager.Instance.onLifeChanged?.Invoke(life);
        }
    }
    private void CallHeal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HealHeart(1, Heart.HeartType.Red);
        }
    }
    private void CallAddHeart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AddHeart(Heart.HeartType.Red);
        }
    }
    private void CallRemoveHeart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RemoveHeart(Heart.HeartType.Red);
        }
    }
    #endregion
}