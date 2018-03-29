using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace AssetManager
{
    /// <summary>
    /// An extended Resource Manager that provides cached instances of objects.
    /// This differs from the stock ResourceManager class which always
    /// deserializes and creates new instances of every object.
    /// After the first time an object is requested, it will be cached
    /// for all future requests.
    /// </summary>
    public class CachedResourceManager : System.Resources.ResourceManager
    {
        /// <summary>
        /// A hashtable is used to store the objects.
        /// </summary>
        private Hashtable objectCache = new Hashtable();

        public CachedResourceManager(Type resourceSource) : base(resourceSource)
        {
        }

        public CachedResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
        {
        }

        public CachedResourceManager(string baseName, Assembly assembly, Type usingResourceSet) : base(baseName, assembly, usingResourceSet)
        {
        }

        public CachedResourceManager() : base()
        {
        }

        /// <summary>
        /// Returns a cached instance of the specified resource.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetObject(string name)
        {
            return GetObject(name, null);
        }

        /// <summary>
        /// Returns a cached instance of the specified resource.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object GetObject(string name, CultureInfo culture)
        {
            var obj = objectCache[name];

            if (obj == null)
            {
                objectCache[name] = base.GetObject(name, culture);
                obj = objectCache[name];
            }

            return obj;
        }
    }
}