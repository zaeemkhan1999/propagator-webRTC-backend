namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class CurrentUserDto : DtoDef
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public string ExternalId { get; set; }
        public string LinkBio { get; set; }

        public string Email { get; set; }


        public string StripeAccountId { get; set; }
        public string StripeCustomerId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeletedAccount { get; set; }

        public UserTypes UserTypes { get; set; }

        public string Bio { get; set; }

        public string DisplayName { get; set; }

        public string Username { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool IsVerified { get; set; }

        public string ImageAddress { get; set; }

        public string Cover { get; set; }

        public Gender Gender { get; set; }

        public string Location { get; set; }

        public bool DirectNotification { get; set; }

        public bool FolloweBacknotification { get; set; }

        public bool LikeNotification { get; set; }

        public bool CommentNotification { get; set; }

        public bool EnableTwoFactorAuthentication { get; set; }

        public bool PrivateAccount { get; set; }

        public bool ProfessionalAccount { get; set; }
        public int FollwingCount { get; set; }
        public int FollowerCount { get; set; }
           public int PostCount { get; set; }
    }
}
