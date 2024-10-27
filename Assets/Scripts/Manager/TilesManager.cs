using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : Singleton<TilesManager>
{
    [SerializeField] private TileState[] tileStates;

    public int IndexOfTileStates(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    public int GetTileStatesLength()
    {
        return tileStates.Length;
    }

    public TileState GetTileState(int index)
    {
        return tileStates[index];
    }

    /// <summary>
    /// ��ȡ������ֵ��2�ļ��η���
    /// �����������ֵ����2���ݣ��򷵻�0��
    /// </summary>
    public int GetPowerOfTwo(int number)
    {
        if (number <= 0)
        {
            return 0;
        }

        int power = 0;
        while ((1 << power) < number)
        {
            power++;
        }

        if ((1 << power) == number)
        {
            return power;
        }

        return 0;
    }
}
