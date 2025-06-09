namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class SettingsQueries
{
    [GraphQLName("settings_getSettings")]
    public ResponseBase<SettingsDto> GetSettings(
                   [Service(ServiceKind.Default)] IConfiguration configuration)
    {
        var initialPrice = configuration.GetSection("InitialPrice").Value;
        var numberOfPeoplePerUnit = configuration.GetSection("NumberOfPeoplePerUnit").Value;
        var settingsDto = new SettingsDto
        {
            InitialPrice = Convert.ToDouble(initialPrice),
            NumberOfPeoplePerUnit = Convert.ToDouble(numberOfPeoplePerUnit)
        };

        return new(settingsDto);
    }
}
