namespace Modules.Base.RoguelikeScreen.Scripts.ObjectPool
{
    public interface IObjectPool
    {
        int ActiveCount { get; }
        int InactiveCount { get; }
        int TotalCount { get; }

        void Prewarm(int count);
        void Clear();
        void Resize(int size);
        void DespawnAll();
    }

    public interface IObjectPool<TValue> : IObjectPool
    {
        TValue Spawn();
        void Despawn(TValue obj);
    }

    public interface IObjectPool<TParam1, TValue> : IObjectPool
    {
        TValue Spawn(TParam1 param1);
        void Despawn(TValue obj);
    }

    public interface IObjectPool<TParam1, TParam2, TValue> : IObjectPool
    {
        TValue Spawn(TParam1 param1, TParam2 param2);
        void Despawn(TValue obj);
    }
    
    public interface IObjectPool<TParam1, TParam2, TParam3, TValue> : IObjectPool
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3);
        void Despawn(TValue obj);
    }
}