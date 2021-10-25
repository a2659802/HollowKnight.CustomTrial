using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomTrial.Utils
{
    internal static class ReflectionHelper
    {
        private static Dictionary<string, Type> behaviours;
        internal static List<Type> NamespaceAllTypes(string ns)
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == ns
                    select t;
            return q.ToList();
        }
        internal static List<(T[], Type)> AllTypesWithAttribues<T>(List<Type> types,bool inherit = false) where T:Attribute
        {
            List<(T[],Type)> result = new();
            foreach(var t in types)
            {
                //var att = t.GetCustomAttribute<T>(inherit);
                var atts = t.GetCustomAttributes<T>(inherit);
                if(!atts.Any())
                    continue;
                result.Add((atts.ToArray(),t));
            }
            return result;
        }
        private static void init_behaviours()
        {
            behaviours = new();
            var b_with_name = AllTypesWithAttribues<MatchNameAttribue>(NamespaceAllTypes("CustomTrial.Behaviours"));
            foreach(var i in b_with_name)
            {
                var atts = i.Item1;
                foreach(var a in atts)
                {
                    behaviours.Add(a.GoName, i.Item2);
                }
            }
        }
        internal static Type MatchBehaviourWithName(string go_name)
        {
            if(behaviours == null)
            {
                init_behaviours();
            }
            if(behaviours.TryGetValue(go_name,out var t))
                return t;
            return null;
        }
    }
}
