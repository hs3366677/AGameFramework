/********************************************************************
	created:	2014/11/27
	created:	27:11:2014   10:16
	filename: 	\CommonPlatform\Server\Framework\Utility\Protocol\Attributes\ProtoAttributes.cs
	file path:	\CommonPlatform\Server\Framework\Utility\Protocol\Attributes
	file base:	ProtoAttributes
	file ext:	cs
	author:		史耀力
	
	purpose:	协议所需特性实现
*********************************************************************/
using System;

namespace Protocols
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ProtoBodyAttribute : Attribute
    {
        private readonly Type bodyType;

        public ProtoBodyAttribute(Type inType)
        {
            bodyType = inType;
        }

        public Type BodyType
        {
            get { return bodyType; }
        }
    }
}
