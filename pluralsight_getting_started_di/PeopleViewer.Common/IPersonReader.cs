using System.Collections.Generic;
using PeopleViewer.Common;

namespace PersonDataReader.Service
{
    public interface IPersonReader
    {
        IEnumerable<Person> GetPeople();
        Person GetPerson(int id);
    }
}