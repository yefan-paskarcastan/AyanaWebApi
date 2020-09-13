using System.Collections.Generic;
using AyanaWebApi.Models;

namespace AyanaWebApi.Utils
{
    public class NnmclubListItemComparer : IEqualityComparer<NnmclubListItem>
    {
        public bool Equals(NnmclubListItem x, NnmclubListItem y)
        {
            return x.Added == y.Added && x.Href == y.Href && x.Name == y.Name;
        }

        public int GetHashCode(NnmclubListItem obj)
        {
            if (obj == null)
            {
                return 0;
            }

            int date = obj.Added.GetHashCode();
            int href = obj.Href.GetHashCode();
            int name = obj.Name.GetHashCode();
            return date ^ href ^ name;
        }
    }
}
