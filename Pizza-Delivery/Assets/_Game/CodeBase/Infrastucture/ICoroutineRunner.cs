using System.Collections;
using UnityEngine;

namespace _Game.CodeBase.Infrastucture
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}