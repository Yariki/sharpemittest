using System;
using System.Collections.Generic;

namespace Library1
{
    public delegate void PersonChanged(Person person);

    public delegate void PersonsChanged(Guid id, Person[] persons, DataDvcStatus[] status);
    
    public class DataSvc
    {
        private PersonChanged _personChanged;
        private PersonsChanged _personsChanged;

        private int count = 0;

        public DataSvc()
        {
        }

        public void Subscribe(PersonChanged personChanged)
        {
            var svc = this;
            this._personChanged = (PersonChanged) Delegate.Combine(this._personChanged, personChanged);
        }
            
        public void Subscribe(PersonsChanged personsChanged)
        {
            var svc = this;
            this._personsChanged = (PersonsChanged) Delegate.Combine(this._personsChanged, personsChanged);
        }

        public void InvokePerson()
        {
            if (this._personChanged == null)
            {
                return;
            }
            
            count++;
            var person = new Person(){Age = count, Name = $"Person{count}"};
            this._personChanged(person);
        }

        public void InvokePersons()
        {
            if (this._personsChanged == null)
            {
                return;
            }
            
            var list = new List<Person>();
            count++;
            list.Add(new Person(){Age = count, Name = $"Person{count}"});
            count++;
            list.Add(new Person(){Age = count, Name = $"Person{count}"});
            var listStatus = new List<DataDvcStatus>()
            {
                new DataDvcStatus(1,DateTime.Now),
                new DataDvcStatus(0,DateTime.Now)
            };
            
            this._personsChanged(Guid.NewGuid(),list.ToArray(),listStatus.ToArray());
        }
        
        

    }
}