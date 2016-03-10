/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Linq;
using System.Configuration;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// Center 要素を格納する構成要素を表します。
    /// </summary>
    internal class CenterConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CenterConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CenterConfigurationElement)element).Name;
        }

        public CenterConfigurationElement FindOrDefault(Func<CenterConfigurationElement, bool> predicate)
        {
            return this.Cast<CenterConfigurationElement>().FirstOrDefault(predicate);
        }
    }
}
