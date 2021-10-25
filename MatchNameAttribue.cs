using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTrial
{
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = true, Inherited = false)]
    public sealed class MatchNameAttribue : Attribute
    {
        public string GoName;
        public MatchNameAttribue(string go_name)
        {
            GoName = go_name;
        }
    }
}
