using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectParallelSort
{
    internal class Name
    {
        public Name(string fname, string lname)
        {
            this.firstName = fname; 
            this.lastName = lname;
        }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public override string ToString()
        {
            return firstName + " " + firstName;
        }

        public int CompareTo(Name name)
        {
            // compared by lastname, then by firstname
            int result = lastName.CompareTo(name.lastName);
            if (result == 0)
            {
                return firstName.CompareTo(name.firstName);
            }
            return result;
        }
    }
}
