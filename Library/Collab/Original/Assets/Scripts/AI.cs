using System.Collections.Generic;
using UnityEngine;

public class AI {

    int GuessLength;
    List<string> all = new List<string>(0);

    double[,] prob = new double[10, 9];
    double[,] mem = new double[10, 1025];
    bool[,] vis = new bool[10, 1025];

    public AI(int length)
    {
        GuessLength = length;
        fillAll(0, "");
    }

    public string whatNext(List<Guess> list, int type, string number) {
        if (type == 1)
            return nextZerx(list);
        else if (type == 2)
            return nextProto(list);
        else if (type == 3)
            return nextTurbo(list);
        else if (type == 4)
            return nextStella(list);
        else if (type == 5)
            return nextCubic(list);
        return "no such AI yet";
    }

    string nextZerx(List<Guess> list) {

        /*
        List<char> newNum = notUsed(list);

        if(newNum.Count >= GuessLength)
        {
            newNum.Shuffle();
            string ans = "";
            for (int i = 0; i < GuessLength; i++)
                ans += newNum[i];
            return ans;
        }
        */

        List<string> allowed = filter(list);

        fillProb(allowed);

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 1025; j++)
                vis[i, j] = false;
        dp(0, 0);
        Debug.Log(dp(0, 0));
        string ans = trace(0, 0);
        return ans;
    }

    string nextProto(List<Guess> list)
    {
        List<char> newNum = notUsed(list);

        if (newNum.Count >= GuessLength)
        {
            newNum.Shuffle();
            string ans = "";
            for (int i = 0; i < GuessLength; i++)
                ans += newNum[i];
            return ans;
        }

        List<string> allowed = filter(list);

        int id = Random.Range(0, allowed.Count);

        return allowed[id];
    }

    string nextTurbo(List<Guess> list)
    {
        List<string> allowed = filter(list);

        int id = Random.Range(0, allowed.Count);

        return allowed[id];
    }

    string nextStella(List<Guess> list) {
        List<int> avoid = new List<int>();
        foreach (Guess guess in list)
        {
            int total = guess.gBulls + guess.gCows;
            if (total == 0)
            {
                foreach (char ch in guess.gGuess)
                    avoid.Add((int)System.Char.GetNumericValue(ch));
            }
            else if (total >= GuessLength-1)
            {
                return nextTurbo(list);
            }
        }
        return getRandomGuess(avoid);
    }

    string nextCubic(List<Guess> list)
    {
        List<int> avoid = new List<int>();
        foreach (Guess guess in list)
        {
            int total = guess.gBulls + guess.gCows;
            if (total == 0)
            {
                foreach (char ch in guess.gGuess)
                    avoid.Add((int)System.Char.GetNumericValue(ch));
            }
            else if (total == GuessLength)
            {
                return nextTurbo(list);
            }
        }
        return getRandomGuess(avoid);
    }

    List<string> filter(List<Guess> list)
    {
        List<string> allowed = new List<string>(0);
        foreach(string number in all)
        {
            bool f = true;
            foreach (Guess guess in list)
                if (!likeness(number, guess))
                {
                    f = false;
                    break;
                }
            if (f)
                allowed.Add(number);
        }
        return allowed;
    }

    List<char> notUsed(List<Guess> list)
    {
        //get unused numbers so far
        List<char> nfound = new List<char>(0);
        HashSet<char> found = new HashSet<char>();

        foreach (Guess g in list)
            foreach (char c in g.gGuess)
                found.Add(c);

        for (int i = 0; i < 10; i++)
            if (!found.Contains((char)(i + '0')))
                nfound.Add((char)(i + '0'));
        return nfound;
    }

    string getRandomGuess(List<int> avoid)
    {
        List<string> nums = new List<string>();
        for (int i = 0; i < 10; i++)
            if(!avoid.Contains(i))
            {
                nums.Add(i.ToString());
            }
        string guess = "";
        for (int i = 0; i < LobbyManager.LM.guess_length; i++)
        {
            int r = UnityEngine.Random.Range(0, nums.Count);
            guess += nums[r];
            nums.RemoveAt(r);

        }
        return guess;
    }

    bool likeness(string x, Guess y)
    {
        int Circle = 0;
        int Square = 0;
        compareNumbers(ref Square, ref Circle, x, y.gGuess);
        return Circle == y.gBulls && Square == y.gCows;
    }

    void compareNumbers(ref int cows, ref int bulls, string number, string guess)
    {
        cows = bulls = 0;
        foreach (char a in number)
            foreach (char b in guess)
                if (a == b)
                    cows++;
        for (int i = 0; i < GuessLength; i++)
            if (number[i] == guess[i])
                bulls++;
        cows -= bulls;
    }

    void fillAll(int lvl, string num)
    {
        if (lvl == GuessLength)
        {
            all.Add(num);
            return;
        }
        for (int i = 0; i < 10; i++)
            if (!num.Contains(i.ToString()))
                fillAll(lvl + 1, num + i.ToString());
    }

    void fillProb(List<string> list)
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 9; j++)
                prob[i, j] = 0;
        foreach(string s in list)
            for (int i = 0; i < s.Length; i++)
                prob[(int)System.Char.GetNumericValue(s[i]), i] += 1;

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 9; j++)
                prob[i, j] /= list.Count;
    }

    double dp(int lvl,int mask)
    {
        if (lvl == GuessLength)
            return 1;
        if (vis[lvl, mask] == true)
            return mem[lvl, mask];
        vis[lvl, mask] = true;
        double ret = 0;
        for(int i=0; i<10; i++)
            if((mask & (1<<i)) == 0)
            {
                if (ret < prob[i,lvl] * dp(lvl + 1, mask | (1 << i)))
                    ret = prob[i, lvl] * dp(lvl + 1, mask | (1 << i));
            }
        return mem[lvl, mask] = ret;
    }

    string trace(int lvl, int mask)
    {
        if (lvl == GuessLength)
            return "";
        List<int> pos = new List<int>();
        for (int i = 0; i < 10; i++)
            if ((mask & (1 << i)) == 0)
                if (ExtensionMethods.IsApproximatelyEqualTo(mem[lvl, mask], prob[i, lvl] * dp(lvl + 1, mask | (1 << i))))
                    pos.Add(i);
        int c = pos[Random.Range(0,pos.Count)];
        return c.ToString() + trace(lvl + 1, mask | (1 << c));
    }

    public Pair<int,double> test(int type, int number)
    {
        int mx = 0;
        double sum = 0;
        while (number-- > 0)
        {
            List<Guess> gu = new List<Guess>();
            Guess guess = new Guess();
            guess.gBulls = guess.gCows = 0;
            string num = getRandomGuess(new List<int>());
            while(guess.gBulls < GuessLength)
            {
                guess.gGuess = whatNext(gu, type,num);
                int cows=0,bulls = 0;
                compareNumbers(ref cows, ref bulls, num, guess.gGuess);
                guess.gCows = cows;
                guess.gBulls = bulls;
                gu.Add(guess);
            }
            mx = System.Math.Max(mx,gu.Count);
            sum += gu.Count;
        }
        return new Pair<int,double> (mx, sum/number);
    }
}

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};

public static class ExtensionMethods
{
    public static bool IsApproximatelyEqualTo(this double initialValue, double value)
    {
        return IsApproximatelyEqualTo(initialValue, value, 0.00001);
    }

    public static bool IsApproximatelyEqualTo(this double initialValue, double value, double maximumDifferenceAllowed)
    {
        // Handle comparisons of floating point values that may not be exactly the same
        return (System.Math.Abs(initialValue - value) < maximumDifferenceAllowed);
    }
}
