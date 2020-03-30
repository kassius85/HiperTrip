using System.Collections.Generic;

namespace HiperTrip.Interfaces
{
    public interface IResultService
    {
        void AddValue(string key, object value);

        void AddValue(bool resultadp, string mensaje);

        Dictionary<string, object> GetProperties();

        string GetJsonProperties();

        bool TryGetProperties(out Dictionary<string, object> properties);

        bool TryGetMember(string key, out object result);

        bool TrySetMember(string key, object value);
    }
}