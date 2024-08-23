using System.Collections;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class CoroutineWrapper
{
    public bool IsCompleted { get; private set; }
    public float ElapsedTime
    {
        get
        {
            return Time.time - _startTime;
        }
    }

    // 원래 코루틴
    protected IEnumerator _coroutine;
    protected MonoBehaviour _owner;
    protected float _startTime;
    protected Coroutine _runningCoroutine;

    public CoroutineWrapper(IEnumerator coroutine, MonoBehaviour owner)
    {
        _coroutine = coroutine;
        _owner = owner;
        IsCompleted = false;
    }

    public Coroutine Start()
    {
        _startTime = Time.time;
        _runningCoroutine = _owner.StartCoroutine(Run());
        return _runningCoroutine;
    }

    private IEnumerator Run()
    {
        while (_coroutine.MoveNext())
        {
            yield return _coroutine.Current;
        }

        IsCompleted = true;
    }

    public void Stop()
    {
        if (_runningCoroutine != null)
        {
            _owner.StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
            IsCompleted = true;
        }
    }

}
