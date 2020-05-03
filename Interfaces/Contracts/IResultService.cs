using Entities.Enums;
using System.Collections.Generic;

namespace Interfaces.Contracts
{
    public interface IResultService
    {
        void AddValue(string key, object value);

        void AddValue(Resultado resultado, string mensaje);

        Dictionary<string, object> GetProperties();

        string GetJsonProperties();

        bool TryGetProperties(out Dictionary<string, object> properties);

        bool TryGetMember(string key, out object result);

        bool TrySetMember(string key, object value);
    }
}