using System.Collections.Generic;
using UnityEngine;

class RandomUtil
{
    public static int getRandom(int start, int end)
    {
        return Random.Range(start, end + 1);
    }

    public static int SelectProbability(List<int> probabilities)
    {
        // 生成一个1到100之间的随机数
        int randomNumber = getRandom(1,100);

        // 遍历概率列表，确定随机数落在哪个区间
        int cumulativeProbability = 1;
        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomNumber < cumulativeProbability)
            {
                return i;
            }
        }

        // 如果所有概率都不匹配，返回最后一个索引
        return probabilities.Count - 1;
    }
}
