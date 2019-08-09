using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tokenio.Sdk.Api.Service;
using static Io.Token.Proto.Bankapi.SetValueRequest.Types;

namespace Tokenio.BankSample.Services
{
    /// <summary>
    /// In-memory implementation of the {@link StorageService}. This needs to be
    /// backed by a durable store to work in production.
    /// </summary>
    public class StorageServiceImpl : IStorageService
    {
        private readonly IDictionary<string, byte[]> storage = new Dictionary<string, byte[]>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] GetValue(string key) {
            return storage.ContainsKey(key) ? storage[key] : null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] SetValue(
            string key,
            ContentCategory category,
            byte[] value) {

            if (storage.ContainsKey(key))
            {
                var temp = storage[key];
                storage.Add(key, value);
                return temp;

            }
            storage.Add(key, value);
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveValue(string key)
        {
            storage.Remove(key);
        }

    }
}
