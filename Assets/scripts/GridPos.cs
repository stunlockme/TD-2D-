public struct GridPos
{
    public int X { get; set; }
    public int Y { get; set; }

    public GridPos(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// equals operator definition
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>true if objects are equal</returns>
    public static bool operator ==(GridPos a, GridPos b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    /// <summary>
    /// not equal operator definition
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>true if objects are not equal</returns>
    public static bool operator !=(GridPos a, GridPos b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    /// <summary>
    /// overrides the standard equals definition
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        GridPos gridPos = (GridPos)obj;
        if ((System.Object)gridPos == null)
        {
            return false;
        }
        return (X == gridPos.X) && (Y == gridPos.Y);
    }

    /// <summary>
    /// overrides the hash code
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return 0;
    }
}
