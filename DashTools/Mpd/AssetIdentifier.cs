using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd
{
    /// <summary>
    /// The AssetIdentifier is used to identify the asset on Period level. If two different Periods contain 
    /// equivalent Asset Identifiers then the content in the two Periods belong to the same asset.
    /// </summary>
    public class AssetIdentifier : DescriptorType
    {
        public AssetIdentifier(XElement node) 
            : base(node)
        {
        }
    }
}