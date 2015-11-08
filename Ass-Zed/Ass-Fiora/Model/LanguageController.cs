using System.Collections.Generic;
using Ass_Zed.Model.Enum;

namespace Ass_Zed.Model
{
    abstract class LanguageController
    {
        public Dictionary<EnumContext, string> Dictionary = new Dictionary<EnumContext, string>();
    }
}
