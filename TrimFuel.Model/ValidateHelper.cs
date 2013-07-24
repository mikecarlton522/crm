using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ValidateHelper : IDisposable
    {
        private const string INVALID_PROPERTY_TEMPLATE = "Invalid Property({0}): {1}";
        private const string NULL_EXCEPTION = "Value can not be NULL";
        private const string STRING_OUT_OF_RANGE_EXCEPTION = "String is out of range({0})";

        private IList<KeyValuePair<string, string>> exceptions = null;

        private void AppendException(string propName, string exceptionString)
        {
            if (exceptions == null)
            {
                exceptions = new List<KeyValuePair<string, string>>();
            }

            exceptions.Add(new KeyValuePair<string, string>(propName, exceptionString));
        }

        public bool IsValid
        {
            get { return (exceptions == null); }
        }

        public string FullExceptionString
        {
            get
            {
                if (IsValid)
                {
                    return string.Empty;
                }

                StringBuilder b = new StringBuilder();
                foreach (KeyValuePair<string, string> item in exceptions)
                {
                    b.AppendLine(string.Format(INVALID_PROPERTY_TEMPLATE, item.Key, item.Value));
                }

                return b.ToString();
            }
        }

        public void AssertNotNull(string propName, object propValue)
        {
            if (propValue == null)
            {
                AppendException(propName, NULL_EXCEPTION);
            }
        }

        public void AssertString(string propName, string propValue, int maxLength)
        {
            if (propValue != null && propValue.Length > maxLength)
            {
                AppendException(propName, string.Format(STRING_OUT_OF_RANGE_EXCEPTION, maxLength));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!IsValid)
            {
                throw new Exception(FullExceptionString);
            }
        }

        #endregion
    }
}
