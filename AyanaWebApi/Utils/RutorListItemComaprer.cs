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
        const int _multiplier = 89;

        public bool Equals(RutorListItem x, RutorListItem y)
        {
            return x.AddedDate == y.AddedDate && x.HrefNumber == y.HrefNumber && x.Name == y.Name;
        }

        public int GetHashCode(RutorListItem obj)
        {
            int result = 0;

            if (obj == null)
            {
                return 0;
            }

            string main = obj.AddedDate + obj.HrefNumber + obj.Name;
            int length = main.Length;

            if (length > 0)
            {
                char let1 = main[0];
                char let2 = main[length - 1];

                int part1 = let1 + length;
                result = (_multiplier * part1) + let2 + length;
            }
            return result;
        }
    }
}
