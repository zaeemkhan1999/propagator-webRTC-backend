namespace Apsy.App.Propagator.Application.Services;

public class VerificationRequestService : ServiceBase<VerificationRequest, VerificationRequestInput>, IVerificationRequestService
{
    public VerificationRequestService(IVerificationRequestRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;

    }
    private readonly IVerificationRequestRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public override ResponseBase<VerificationRequest> Add(VerificationRequestInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        var exist = repository.GetVerificationRequest().Any(c =>
                                    c.UserId == currentUser.Id &&
                                    c.VerificationRequestAcceptStatus != VerificationRequestAcceptStatus.Rejected);
        if (exist)
            return ResponseStatus.AlreadyExists;

        var verificationRequest = input.Adapt<VerificationRequest>();
        return repository.Add(verificationRequest);

    }

    public async Task<ResponseBase<VerificationRequest>> AcceptedVerificationRequest(int requestId)
    {
        var verificationRequest = await repository.GetVerificationRequestById(requestId);
        if (verificationRequest == null)
            return ResponseStatus.NotFound;

        if (verificationRequest.VerificationRequestAcceptStatus != VerificationRequestAcceptStatus.Pending)
            return ResponseStatus.NotEnoghData;
        verificationRequest.VerificationRequestAcceptStatus = VerificationRequestAcceptStatus.Accepted;

        var user =await repository.GetUser().Where(d => d.Id == verificationRequest.UserId).FirstOrDefaultAsync();
        user.IsVerified = true;
        var result = await repository.UpdateAsync(verificationRequest);

        await repository.UpdateAsync<User>(user);

        return result;
    }   
    
    public async Task<ResponseBase<VerificationRequest>> RejectVerificationRequest(int requestId,string reason)
    {
        var verificationRequest = await repository.GetVerificationRequestById(requestId);
        if (verificationRequest == null)
            return ResponseStatus.NotFound;

        if (verificationRequest.VerificationRequestAcceptStatus != VerificationRequestAcceptStatus.Pending)
            return ResponseStatus.NotEnoghData;

        verificationRequest.VerificationRequestAcceptStatus = VerificationRequestAcceptStatus.Rejected;
        verificationRequest.ReasonReject=reason;

        return await repository.UpdateAsync(verificationRequest);
    }


    public async Task<ResponseBase> RemoveVerificationRequest(int requestId)
    {
        var query = repository.GetVerificationRequest().Where(d => d.Id == requestId);

        if(requestId==0 ||  !query.Any())
            return ResponseStatus.NotFound;
        var result =await query.FirstOrDefaultAsync();
        await repository.RemoveAsync(result);

        return ResponseBase.Success();
    }
    public virtual User GetCurrentUser()
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user").Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }
}
