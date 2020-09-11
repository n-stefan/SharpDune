
namespace SharpDune
{
    class CArray<T>
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

        internal CArray() {}

        internal CArray(CArray<T> old)
        {
            Arr = old.Arr;
            Ptr = old.Ptr;
        }

        internal CArray(T[] array, int pointer)
        {
            Arr = array;
            Ptr = pointer;
        }

        public static CArray<T> operator +(CArray<T> array, int amount)
        {
            array.Ptr += amount;
            return array;
        }

        public static CArray<T> operator ++(CArray<T> array)
        {
            array.Ptr++;
            return array;
        }
    }
}
