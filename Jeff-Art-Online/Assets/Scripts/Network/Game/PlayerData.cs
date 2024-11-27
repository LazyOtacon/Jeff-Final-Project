using Fusion;

public struct PlayerData : INetworkInput
{
    public float horizontalInput;
    public float gunRotation;

    public NetworkButtons networkButtons;
}