using System;
using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd
{
    public class DescriptorType : MpdElement
    {
        public DescriptorType(XElement node) 
            : base(node)
        {
        }

        /// <summary>
        /// Mandatory.
        /// 
        /// Specifies a URI to identify the scheme. The semantics of this 
        /// element are specific to the scheme specified by this attribute. 
        /// The @schemeIdUri may be a URN or URL.When a URL is 
        /// used, it should also contain a month-date in the form 
        /// mmyyyy; the assignment of the URL must have been 
        /// authorized by the owner of the domain name in that URL on 
        /// or very close to that date, to avoid problems when domain 
        /// names change ownership.
        /// </summary>
        public string SchemeIdUri
        {
            get { return helper.ParseMandatoryString("schemeIdUri"); }
        }

        /// <summary>
        /// Optional
        /// 
        /// Specifies the value for the descriptor element. The value 
        /// space and semantics must be defined by the owners of the 
        /// scheme identified in the @schemeIdUri attribute.
        /// </summary>
        public string Value
        {
            get { return helper.ParseOptionalString("value"); }
        }

        /// <summary>
        /// Optional
        /// 
        /// specifies an identifier for the descriptor. Descriptors with 
        /// identical values for this attribute shall be synonymous, i.e. 
        /// the processing of one of the descriptors with an identical 
        /// value is sufficient.
        /// </summary>
        public string Id
        {
            get { return helper.ParseOptionalString("id"); }
        }
    }
}