using UnityEngine;

[System.Serializable]
public class Heart
{
    public enum HeartState
    {
        Empty,
        Half,
        Full,
    }

    public enum HeartType
    {
        Red,
        Black,
    }

    public Heart(HeartType type, HeartState state)
    {
        this.state = state;
        this.type = type;
    }

    [Header("Heart")]
    [SerializeField] private HeartState state = HeartState.Full;
    [SerializeField] private HeartType type = HeartType.Red;

    public HeartState State { get => state; set => state = value; }
    public HeartType Type { get => type; set => type = value; }
}