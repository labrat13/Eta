using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace InventoryModuleManager2
{
    /// <summary>
    /// Базовый класс переноса данных Хранилища
    /// </summary>
    public class RecordBase
    {
        /// <summary>
        /// Словарь для хранения данных в системе ключ-значение
        /// </summary>
        protected Dictionary<String, String> m_dictionary;
    
        public RecordBase()
        {
            m_dictionary = new Dictionary<string, string>(16);
        }

        ~RecordBase()
        {
            m_dictionary.Clear();
        }

        /// <summary>
        /// Получить внутренний словарь для прямого чтения
        /// </summary>
        /// <returns></returns>
        internal Dictionary<String, String> getBaseDictionary()
        {
            return m_dictionary;
        }

        protected void setValue(string name, long val)
        {
            this.m_dictionary[name] = val.ToString(CultureInfo.InvariantCulture);
        }

        protected void setValue(string name, int val)
        {
            this.m_dictionary[name] = val.ToString(CultureInfo.InvariantCulture);
        }

        protected void setValue(string name, UInt32 val)
        {
            this.m_dictionary[name] = val.ToString(CultureInfo.InvariantCulture);
        }

        internal void setValue(string name, string val)
        {
            this.m_dictionary[name] = val;
        }

        protected void setValue(string name, bool val)
        {
            this.m_dictionary[name] = val.ToString();
        }


        protected UInt32 getValueAsUInt32(string name)
        {
            return UInt32.Parse(this.m_dictionary[name], CultureInfo.InvariantCulture);
        }
        protected Int64 getValueAsInt64(string name)
        {
            return Int64.Parse(this.m_dictionary[name], CultureInfo.InvariantCulture);
        }
        protected Int32 getValueAsInt32(string name)
        {
            return Int32.Parse(this.m_dictionary[name], CultureInfo.InvariantCulture);
        }
        protected string getValueAsString(string name)
        {
            return this.m_dictionary[name];
        }
        protected bool getValueAsBoolean(string name)
        {
            return Boolean.Parse(this.m_dictionary[name]);
        }

    }
}
