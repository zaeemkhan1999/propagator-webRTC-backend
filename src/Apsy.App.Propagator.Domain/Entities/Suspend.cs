namespace Apsy.App.Propagator.Domain.Entities
{
    public class Suspend : UserKindDef<User>
    {
        public int DayCount { get; set; }
        public DateTime SuspensionLiftingDate { get; set; }
        public SuspendType SuspendType { get; set; }
        [GraphQLIgnore]
        public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser, CrudType crudType)
        {
            if (crudType == CrudType.UsersSuspendedEvent)
            {
                var usersSuspendedEvent = new UsersSuspendedEvent()
                {
                    AdminId = currrentUser.Id,
                    DayCount = DayCount,
                    SuspendType = SuspendType,
                    UserEmail = User.Email,
                    SuspensionLiftingDate = SuspensionLiftingDate,
                    UserId = UserId,
                    UserImageAddress = User?.ImageAddress,
                    UserCover = User?.Cover,
                    UserDisplayName = User?.DisplayName,
                    UserName = User?.Username
                };
                events.Add(usersSuspendedEvent);
            }
            return events;
        }

    }
}