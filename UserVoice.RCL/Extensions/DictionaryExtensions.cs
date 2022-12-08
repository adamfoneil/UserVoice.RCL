namespace UserVoice.RCL.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static bool IsSet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue, bool> filter) where TKey : struct where TValue : struct
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return filter.Invoke(value);
            }

            return false;
        }

        internal static bool IsTrue<TKey>(this Dictionary<TKey, bool> dictionary, TKey key) where TKey : struct => IsSet(dictionary, key, (val) => val);
    }
}
