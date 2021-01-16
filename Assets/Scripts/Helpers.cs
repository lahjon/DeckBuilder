using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static string ToLowerFirstChar(this string input)
    {
        if(string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }
}
