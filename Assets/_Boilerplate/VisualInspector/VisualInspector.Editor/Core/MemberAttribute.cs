using System;
using System.Linq;
using System.Reflection;

namespace VisualInspector.Editor.Core
{
    /// <summary>
    ///     Container for <see cref="MemberInfo"/> with cached <see cref="Attributes"/>
    /// </summary>
    public interface IMemberAttribute
    {
        MemberInfo MemberInfo { get; }
        Attribute[] Attributes { get; }
    }
    
    /// <summary>
    ///     Container for <see cref="MemberInfo"/> with cached <see cref="Attributes"/>
    /// </summary>
    public class MemberAttribute : IMemberAttribute
    {
        public MemberAttribute(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            Attributes = memberInfo.GetCustomAttributes<Attribute>().ToArray();
        }

        /// <summary>
        ///     Underlying <see cref="MemberInfo"/>
        /// </summary>
        public MemberInfo MemberInfo { get; }
        
        /// <summary>
        ///     Cached <see cref="Attribute"/>s
        /// </summary>
        public Attribute[] Attributes { get; }
    }
    
    /// <summary>
    ///     Container for <see cref="MemberInfo"/> with cached <see cref="Attributes"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemberAttribute<T> : MemberAttribute where T : MemberInfo
    {
        public MemberAttribute(T memberInfo) : base(memberInfo)
        {
            MemberInfo = memberInfo;
            Attributes = memberInfo.GetCustomAttributes<Attribute>().ToArray();
        }

        /// <summary>
        ///     Underlying <see cref="MemberInfo"/>
        /// </summary>
        public new T MemberInfo { get; }
        
        /// <summary>
        ///     Cached <see cref="Attribute"/>s
        /// </summary>
        public new Attribute[] Attributes { get; }
    }
}