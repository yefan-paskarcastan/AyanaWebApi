using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AyanaWebApi.Models;

namespace AyanaWebApi.Utils
{
    public class RutorListItemComaprer : IEqualityComparer<RutorListItem>
    {
        public bool Equals(RutorListItem x, RutorListItem y)
        {
            return x.AddedDate == y.AddedDate && x.HrefNumber == y.HrefNumber && x.Name == y.Name;
        }

        public int GetHashCode(RutorListItem obj)
        {
            if (obj == null)
            {
                return 0;
            }

            int date = obj.AddedDate.GetHashCode();
            int href = obj.HrefNumber.GetHashCode();
            int name = obj.Name.GetHashCode();
            return date ^ href ^ name;
        }
    }
}
