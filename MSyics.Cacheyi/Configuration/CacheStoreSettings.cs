/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    internal class CacheStoreSettings : ICacheStoreSettings
    {
        public Type CenterType { get; set; }
        public string StoreName { get; set; }
        public int? MaxCapacity { get; set; }
        public TimeSpan? Timeout { get; set; }
    }
}
