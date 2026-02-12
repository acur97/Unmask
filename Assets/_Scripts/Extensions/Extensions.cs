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
}