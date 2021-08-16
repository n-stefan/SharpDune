namespace SharpDune.Include;

class Array<T>
{
    internal T[] Arr { get; set; }

    internal int Ptr { get; set; }

    internal T Curr
    {
        get => Arr[Ptr];
        set => Arr[Ptr] = value;
    }

    //internal T CurrInc
    //{
    //    get => Arr[Ptr++];
    //    //set => Arr[Ptr++] = value;
    //}

    internal Array() { }

    internal Array(Array<T> old)
    {
        Arr = old.Arr;
        Ptr = old.Ptr;
    }

    //internal Array(T[] array, int pointer)
    //{
    //    Arr = array;
    //    Ptr = pointer;
    //}

    public static Array<T> operator +(Array<T> array, int amount)
    {
        array.Ptr += amount;
        return array;
    }

    public static Array<T> operator ++(Array<T> array)
    {
        array.Ptr++;
        return array;
    }
}
