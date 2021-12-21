using Resco.Cloud.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Tasks.Code
{
    public class HAEntity
    {
        public Dictionary<string, object> attributes { get; set; } = new Dictionary<string, object>();
        public HAEntity()
        {
        }
        public HAEntity(string entityName)
        {
            PrimaryEntity = entityName;
        }
        public HAEntity(string entityName, Guid Id)
        {
            PrimaryEntity = entityName;
            this.Id = Id;
        }
        public object this[string name]
        {
            get
            {
                TryGetValue(name, out object value);
                return value;
            }
            set
            {
                this.Add(name, value);
            }
        }
        public Guid Id { get; set; }
        public T GetPropertyValue<T>(string name)
        {
            return this.HasValue(name) ? (T)attributes[name] : default(T);
        }
        public void Add(string name, object value)
        {
            if (HasAttribute(name))
            {
                attributes[name] = value;
                if (iEntity != null)
                {
                    iEntity[name] = value;
                }
            }
            else
            {
                attributes.Add(name, value);
                if (iEntity != null)
                {
                    iEntity.Add(name, value);
                }
            }
        }
        public bool HasAttribute(string name)
        {
            return attributes.TryGetValue(name, out object value);
        }
        public bool HasValue(string name)
        {
            attributes.TryGetValue(name, out object value);
            return value != null;
        }
        public IEntityReference ToEntityReference()
        {
            return new EntityReference
            {
                EntityName = PrimaryEntity,
                Id = Id,
            };
        }
        public bool TryGetValue(string name, out object value)
        {
            return attributes.TryGetValue(name, out value);
        }
        public string PrimaryEntity { get; set; }
        public string[] PrimaryKey
        {
            get
            {
                return new string[] { "id" };
            }
        }
        public int Count => attributes.Count;
        public string[] Properties => attributes.Keys.ToArray();
        public IEntity iEntity { get; set; }
    }
}
