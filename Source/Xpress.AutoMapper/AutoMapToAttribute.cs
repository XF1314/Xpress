using System;
using AutoMapper;

namespace Xpress.AutoMapper
{
    /// <summary>
    /// Tag mapping to target object
    /// </summary>
    public class AutoMapToAttribute : AutoMapAttributeBase
    {
        /// <inheritdoc />
        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            CreateMap(configuration, type, MemberList.Source);
        }
    }
}

