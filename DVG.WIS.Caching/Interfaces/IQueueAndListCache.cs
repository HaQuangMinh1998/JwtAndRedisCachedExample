namespace DVG.WIS.Caching.Interfaces
{
    public interface IQueueAndListCached
    {
        void EndQueue(string key, string item, long score);
        string DeQueue(string key);
        void EndQueue<T>(string key, T item, long score);
        T DeQueue<T>(string key);
        long GetSortedSetCount(string key);
        void Push(string key, string item);
        string Pop(string key);
        void SetEntryOrIncrementValueInHash(string hashKey, string key);
        System.Collections.Generic.Dictionary<string, string> GetAllEntriesAndRemoveFromHash(string hashkey);
    }
}
