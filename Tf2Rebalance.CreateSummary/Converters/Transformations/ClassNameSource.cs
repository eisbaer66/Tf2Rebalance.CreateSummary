using System.Collections.Generic;

namespace Tf2Rebalance.CreateSummary.Converters.Transformations
{
    public interface IClassNameSource
    {
        string TryGet(string                className);
        string NormalizeClassName(string className);
    }

    public class ClassNameSource : IClassNameSource
    {
        private readonly IDictionary<string, string> _classNames = new Dictionary<string, string>
                                                                   {
                                                                       { "scout", "Scout" },
                                                                       { "soldier", "Soldier" },
                                                                       { "pyro", "Pyro" },
                                                                       { "demoman", "Demoman" },
                                                                       { "heavy", "Heavy" },
                                                                       { "engineer", "Engineer" },
                                                                       { "medic", "Medic" },
                                                                       { "sniper", "Sniper" },
                                                                       { "spy", "Spy" },
                                                                   };

        public string TryGet(string className)
        {
            if (className == null)
                return null;

            if (!_classNames.ContainsKey(className.ToLowerInvariant()))
                return null;

            return _classNames[className.ToLowerInvariant()];
        }

        public string NormalizeClassName(string className)
        {
            if (className == null)
                return null;

            if (!_classNames.ContainsKey(className.ToLowerInvariant()))
                return className;

            return _classNames[className.ToLowerInvariant()];
        }
    }
}