using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// https://www.dotnetperls.com/fisher-yates-shuffle
// https://gist.github.com/mikedugan/8249637
// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
// https://www.youtube.com/watch?v=u03XdDYIOkc

public static class GameMananger  {

    public static T[] MengArray<T>(T[] array, int seed)
    {
        System.Random randomNumberGenerator = new System.Random(seed);
        for (int i= 0; i< array.Length; i++)
        {
            int randomIndex = randomNumberGenerator.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    } 
	
}
