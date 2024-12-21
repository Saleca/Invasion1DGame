using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Helpers
{
    public static class ResourcesInterop
    {
        public static bool TryGetResource<T>(string key, out T? resource) 
        {
            resource = default;
            if (App.Current!.Resources.TryGetValue(key, out object? resourceObject))
            {
                resource = (T)resourceObject;
                return true;
            }
            return false;
        }
    }
}
