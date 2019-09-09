public struct BlittableBool
{
    private readonly byte value;

    public BlittableBool(bool value)
    {
        this.value = (byte)(value ? 1 : 0);
    }

    public static implicit operator BlittableBool(bool value)
    {
        return new BlittableBool(value);
    }

    public static implicit operator bool(BlittableBool value)
    {
        return value.value != 0;
    }
}
