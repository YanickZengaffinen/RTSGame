using Unity.Entities;

public struct Unit : IComponentData
{
    public long id;

    public int type;

    public Unit(long id, int type)
    {
        this.id = id;
        this.type = type;
    }
}
