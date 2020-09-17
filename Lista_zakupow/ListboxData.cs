using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lista_zakupow
{
    class ListboxData
    {
        /// <summary>
        /// Lista, która przechowuje przedmioty z listy zakupów.
        /// </summary>
        public static List<String> listboxValues1 = new List<string>();
        public static List<String> listboxValues2 = new List<string>();
        public static List<String> listboxValues3 = new List<string>();
        public static bool list1Created, list2Created, list3Created;
        public static bool list1Edited, list2Edited, list3Edited;
    }
}
