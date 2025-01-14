using UnityEngine;

class RandomUtil
{
    public static int getRandom(int start, int end)
    {
        return Random.Range(start, end + 1);
    }
}
