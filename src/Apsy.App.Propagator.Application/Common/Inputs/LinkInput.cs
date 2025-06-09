using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class LinkInput : InputDef, IEquatable<LinkInput>
{

    public bool Equals(LinkInput other)
    {
        if (
            Id == other.Id &&
            Text == other.Text &&
            Url == other.Url &&
            PostId == other.PostId &&
            ArticleId == other.ArticleId &&
            LinkType == other.LinkType)
            return true;

        return false;
    }


    public override int GetHashCode()
    {
        int hashId = Id == null ? 0 : Id.GetHashCode();
        int hashText = Text == null ? 0 : Text.GetHashCode();
        int hashUrl = Url == null ? 0 : Url.GetHashCode();
        int hashPostId = PostId == null ? 0 : PostId.GetHashCode();
        int hashArticleId = ArticleId == null ? 0 : ArticleId.GetHashCode();
        int hashLinkType = LinkType.GetHashCode();

        return hashId ^ hashText ^ hashUrl ^ hashPostId ^ hashArticleId ^ hashLinkType;
    }


    public int? Id { get; set; }


    [Required(ErrorMessage = "{0} is required")]
    public string Text { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Url { get; set; }

    public int? PostId { get; set; }

    public int? ArticleId { get; set; }

    public LinkType LinkType { get; set; }



}