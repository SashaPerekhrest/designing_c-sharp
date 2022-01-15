using System;

namespace Inheritance.DataStructure
{ 
    public class Category : IComparable
    { 
        public string Message { get; }
        public MessageType Type { get; }
        public MessageTopic Topic { get; }
        
        public Category(string message, MessageType type, MessageTopic topic)
        {
            Message = message;
            Type = type;
            Topic = topic;
        }

        public int CompareTo(object o)
        {
            if (o == null) return -1;
            Category c = o as Category;

            var result = String.Compare(Message, c.Message);
            if (result != 0) return result;

            result = Type.CompareTo(c.Type);
            if (result != 0) return result;
            
            result = Topic.CompareTo(c.Topic);
            return result;
        }

        public override bool Equals(object o) => o == null ? false : this == o as Category;

        public override string ToString() => String.Format("{0}.{1}.{2}",Message,Type,Topic);

        public static bool operator >(Category a, Category b) => a.CompareTo(b) == 1;

        public static bool operator <(Category a, Category b) => a.CompareTo(b) == -1;

        public static bool operator >=(Category a, Category b) => (a.CompareTo(b) == 1 || a.CompareTo(b) == 0);

        public static bool operator <=(Category a, Category b) => (a.CompareTo(b) == -1 || a.CompareTo(b) == 0);

        public static bool operator ==(Category a, Category b) => a.CompareTo(b) == 0;

        public static bool operator !=(Category a, Category b) => a.CompareTo(b) != 0;
    }
}