﻿using Entities.Enums;
using Entities.Helpers;
using Helpers.Extensions;
using Interfaces.Contracts;
using System.Collections.Generic;
using System.Net;

namespace Services.Implementations
{
    public class ResultService : IResultService
    {
        private readonly Dictionary<string, object> _properties;

        public ResultService()
        {
            if (_properties == null)
            {
                _properties = new Dictionary<string, object>();
            }
            else
            {
                _properties.Clear();
            }
        }

        public void AddValue(string key, object value)
        {
            _properties.Add(key, value);
        }

        public void AddValue(Resultado resultado, string mensaje)
        {
            Respuesta respuesta = new Respuesta()
            {
                Resultado = resultado.ToString(),
                Mensaje = mensaje
            };

            _properties.Add("respuesta", respuesta);
        }

        public Dictionary<string, object> GetProperties()
        {
            if (!_properties.ContainsKey("StatusCode"))
            {
                AddValue("StatusCode", HttpStatusCode.OK);
                AddValue(Resultado.Success, string.Empty);
            }

            return _properties;
        }

        public string GetJsonProperties()
        {
            return _properties.ToJsonString();
        }

        public bool TryGetProperties(out Dictionary<string, object> properties)
        {
            properties = default;

            if (_properties.Count > 0)
            {
                properties = _properties;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetMember(string key, out object result)
        {
            if (_properties.ContainsKey(key))
            {
                result = _properties[key];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public bool TrySetMember(string key, object value)
        {
            if (_properties.ContainsKey(key))
            {
                _properties[key] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExistsMember(string key)
        {
            return _properties.ContainsKey(key);
        }
    }
}