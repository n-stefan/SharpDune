/* Generic pool */

namespace SharpDune.Pool
{
    /*
    * To find a pool item of a given type/house, this class is used. The result
    * is also written back in this class.
    */
    class PoolFindStruct
    {
	    internal byte houseID; /*!< House to search for, or HOUSE_INVALID for all. */
	    internal ushort type;  /*!< Type to search for, or -1 for all. */
	    internal ushort index; /*!< Last index of search, or -1 to start from begin. */
    }
}
