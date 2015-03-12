using System;
using System.Runtime;

namespace Scintilla.Configuration.Legacy
{
    [SerializableAttribute()]
    public class ConfigItem
    {
        [NonSerializedAttribute()]
        protected ConfigFile _parent;

        public virtual void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            if (theParent == null)
            {
                _parent = (ConfigFile)this;
            }
            else
            {
                _parent = theParent;
            }
        }
    }

}
