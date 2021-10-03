using System.ComponentModel.DataAnnotations;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Commands
{
    public abstract class Cmd : IXmlElement
    {
        [Required] public int? CmdID { set; get; } = null!;
        public string? Data { set; get; } = null!;
        public int? IsNoResp { set; get; } = null!;

        public Item[]? Item { set; get; } = null!;

        public virtual void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteElementString(WbxmlElement.CMDID, CmdID.ToString()!);
            if (Data != null)
            {
                writer.WriteElementString(WbxmlElement.DATA, Data!);
            }

            if (IsNoResp is > 0)
            {
                writer.WriteSelfClosingElement(WbxmlElement.NORESP);
            }

            if (Item != null)
            {
                foreach (var item in Item)
                {
                    item.Write(writer);
                }
            }
        }

        public abstract IXmlElement Parse(SyncMlParser parser, object? _ = null);
    }
}