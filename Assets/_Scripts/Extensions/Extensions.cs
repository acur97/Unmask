using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static async UniTask<T> AsyncInstantiate<T>(T ob, Transform parent = null) where T : Object
    {
        return (await Object.InstantiateAsync(
            ob,
            new InstantiateParameters { parent = parent, worldSpace = false }))[0];
    }

    public static async UniTask<T[]> AsyncInstantiate<T>(T ob, int count, Transform parent = null) where T : Object
    {
        return (await Object.InstantiateAsync(
            ob,
            count,
            new InstantiateParameters { parent = parent, worldSpace = false }));
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}