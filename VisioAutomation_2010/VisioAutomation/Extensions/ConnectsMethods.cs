using System.Collections.Generic;
using IVisio = Microsoft.Office.Interop.Visio;
using VisioAutomation.Shapes.Connectors;

namespace VisioAutomation.Extensions
{
    public static class ConnectsMethods
    {
        public static IEnumerable<IVisio.Connect> ToEnumerable(this IVisio.Connects connects)
        {
            int count = connects.Count;
            for (int i = 0; i < count; i++)
            {
                yield return connects[i + 1];
            }
        }
    }
}
