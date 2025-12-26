using System;

namespace CmrGameFramework
{
    internal struct TypeNamePair: IEquatable<TypeNamePair>
    {
        private readonly Type m_Type;

        private readonly string m_Name;

        public Type Type => m_Type;

        public string Name => m_Name;

        public TypeNamePair(Type type): this(type, string.Empty) { }

        public TypeNamePair(Type type, string name)
        {
            if ((object)type == null)
            {
                throw Utility.Exception.ThrowException("Type is invalid.");
            }

            m_Type = type;
            m_Name = name ?? string.Empty;
        }

        public override string ToString()
        {
            if ((object)m_Type == null)
            {
                throw Utility.Exception.ThrowException("Type is invalid.");
            }

            string fullName = m_Type.FullName;
            if (!string.IsNullOrEmpty(m_Name))
            {
                return Utility.Text.Format("{0}.{1}", fullName, m_Name);
            }

            return fullName;
        }

        public override int GetHashCode()
        {
            return m_Type.GetHashCode() ^ m_Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeNamePair)
            {
                return Equals((TypeNamePair)obj);
            }

            return false;
        }

        public bool Equals(TypeNamePair value)
        {
            if ((object)m_Type == value.m_Type)
            {
                return m_Name == value.m_Name;
            }

            return false;
        }

        public static bool operator ==(TypeNamePair a, TypeNamePair b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TypeNamePair a, TypeNamePair b)
        {
            return !(a == b);
        }
    }
}
