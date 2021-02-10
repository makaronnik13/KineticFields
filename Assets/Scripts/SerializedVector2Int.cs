using System;

[Serializable]
public class SerializedVector2Int
{
    public int x, y;

    public SerializedVector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}